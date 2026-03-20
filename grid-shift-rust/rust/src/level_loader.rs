use std::collections::HashSet;
use std::fs;
use std::path::Path;

use crate::game_state::GameState;
use crate::grid_pos::GridPos;

#[derive(Debug, Clone)]
pub struct ParsedLevel {
    pub player_start: GridPos,
    pub wall_positions: HashSet<GridPos>,
    pub crate_positions: HashSet<GridPos>,
    pub goal_positions: HashSet<GridPos>,
}

impl ParsedLevel {
    pub fn into_game_state(self) -> GameState {
        GameState::new(
            self.player_start,
            self.wall_positions,
            self.goal_positions,
            self.crate_positions,
        )
    }
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum LevelParseError {
    Empty,
    EmptyFirstRow,
    NonRectangular,
    InvalidSymbol { symbol: char, x: i32, y: i32 },
    BorderNotWalled,
    InvalidPlayerCount { found: usize },
    CrateGoalCountMismatch { crates: usize, goals: usize },
}

pub fn parse_level_text(text: &str) -> Result<ParsedLevel, LevelParseError> {
    let normalized = text.replace('\r', "").trim_end_matches('\n').to_string();
    if normalized.trim().is_empty() {
        return Err(LevelParseError::Empty);
    }

    let lines: Vec<&str> = normalized.split('\n').collect();
    let width = lines[0].len();
    if width == 0 {
        return Err(LevelParseError::EmptyFirstRow);
    }

    for line in &lines {
        if line.len() != width {
            return Err(LevelParseError::NonRectangular);
        }
    }

    let max_y = lines.len() as i32 - 1;
    let max_x = width as i32 - 1;

    let mut player_start = GridPos::ZERO;
    let mut player_count = 0usize;
    let mut walls = HashSet::new();
    let mut crates = HashSet::new();
    let mut goals = HashSet::new();

    for (y, line) in lines.iter().enumerate() {
        for (x, symbol) in line.chars().enumerate() {
            let x = x as i32;
            let y = y as i32;
            let cell = GridPos::new(x, y);

            if !matches!(symbol, '#' | '.' | ' ' | 'P' | 'C' | 'G' | '*') {
                return Err(LevelParseError::InvalidSymbol { symbol, x, y });
            }

            if (x == 0 || y == 0 || x == max_x || y == max_y) && symbol != '#' {
                return Err(LevelParseError::BorderNotWalled);
            }

            match symbol {
                '#' => {
                    walls.insert(cell);
                }
                'P' => {
                    player_start = cell;
                    player_count += 1;
                }
                'C' => {
                    crates.insert(cell);
                }
                'G' => {
                    goals.insert(cell);
                }
                '*' => {
                    crates.insert(cell);
                    goals.insert(cell);
                }
                _ => {}
            }
        }
    }

    if player_count != 1 {
        return Err(LevelParseError::InvalidPlayerCount { found: player_count });
    }

    if crates.len() != goals.len() {
        return Err(LevelParseError::CrateGoalCountMismatch {
            crates: crates.len(),
            goals: goals.len(),
        });
    }

    Ok(ParsedLevel {
        player_start,
        wall_positions: walls,
        crate_positions: crates,
        goal_positions: goals,
    })
}

pub fn parse_level_file(path: &Path) -> Result<ParsedLevel, String> {
    let text = fs::read_to_string(path)
        .map_err(|err| format!("failed to read level '{}': {err}", path.display()))?;

    parse_level_text(&text)
        .map_err(|err| format!("failed to parse level '{}': {err:?}", path.display()))
}
