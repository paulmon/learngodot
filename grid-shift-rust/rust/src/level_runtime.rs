use crate::game_state::{GameState, MoveResult};
use crate::grid_pos::GridPos;

#[derive(Debug, Clone)]
pub struct LevelRuntime {
    pub start_state: GameState,
    pub game_state: GameState,
    pub undo_stack: Vec<GameState>,
    pub is_level_complete: bool,
}

impl LevelRuntime {
    pub fn new(initial_state: GameState) -> Self {
        let mut runtime = Self {
            start_state: initial_state.clone(),
            game_state: initial_state,
            undo_stack: Vec::new(),
            is_level_complete: false,
        };
        runtime.update_win_state();
        runtime
    }

    pub fn try_move(&mut self, delta: GridPos) -> Option<MoveResult> {
        let snapshot = self.game_state.clone();
        let result = self.game_state.try_apply_move(delta)?;

        // Match C# behavior: only successful state changes push undo snapshots.
        self.undo_stack.push(snapshot);
        self.update_win_state();

        Some(result)
    }

    pub fn undo_last_move(&mut self) -> bool {
        if let Some(previous) = self.undo_stack.pop() {
            self.game_state = previous;
            self.update_win_state();
            return true;
        }

        false
    }

    pub fn restart(&mut self) {
        self.game_state = self.start_state.clone();
        self.undo_stack.clear();
        self.update_win_state();
    }

    pub fn update_win_state(&mut self) {
        self.is_level_complete = self.game_state.is_complete();
    }
}
