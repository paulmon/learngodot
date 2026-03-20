use std::collections::HashSet;

use crate::grid_pos::GridPos;

#[derive(Debug, Clone)]
pub struct GameState {
    pub player_grid_pos: GridPos,
    pub wall_positions: HashSet<GridPos>,
    pub goal_positions: HashSet<GridPos>,
    pub crate_positions: HashSet<GridPos>,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct MoveResult {
    pub moved_crate: bool,
    pub crate_from: GridPos,
    pub crate_to: GridPos,
}

impl MoveResult {
    pub const fn no_crate() -> Self {
        Self {
            moved_crate: false,
            crate_from: GridPos::ZERO,
            crate_to: GridPos::ZERO,
        }
    }
}

impl GameState {
    pub fn new(
        player_grid_pos: GridPos,
        wall_positions: impl IntoIterator<Item = GridPos>,
        goal_positions: impl IntoIterator<Item = GridPos>,
        crate_positions: impl IntoIterator<Item = GridPos>,
    ) -> Self {
        Self {
            player_grid_pos,
            wall_positions: wall_positions.into_iter().collect(),
            goal_positions: goal_positions.into_iter().collect(),
            crate_positions: crate_positions.into_iter().collect(),
        }
    }

    pub fn try_apply_move(&mut self, delta: GridPos) -> Option<MoveResult> {
        let target = self.player_grid_pos + delta;
        if self.wall_positions.contains(&target) {
            return None;
        }

        let mut move_result = MoveResult::no_crate();

        if self.crate_positions.contains(&target) {
            let push_target = target + delta;
            if self.wall_positions.contains(&push_target)
                || self.crate_positions.contains(&push_target)
            {
                return None;
            }

            self.crate_positions.remove(&target);
            self.crate_positions.insert(push_target);
            move_result = MoveResult {
                moved_crate: true,
                crate_from: target,
                crate_to: push_target,
            };
        }

        self.player_grid_pos = target;
        Some(move_result)
    }

    pub fn is_complete(&self) -> bool {
        if self.goal_positions.is_empty() || self.crate_positions.len() != self.goal_positions.len()
        {
            return false;
        }

        self.goal_positions
            .iter()
            .all(|goal| self.crate_positions.contains(goal))
    }
}
