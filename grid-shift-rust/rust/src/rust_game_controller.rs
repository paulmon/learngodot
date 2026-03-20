use std::path::Path;

use godot::classes::{INode, Node, ProjectSettings};
use godot::prelude::*;

use crate::grid_pos::GridPos;
use crate::level_loader::parse_level_file;
use crate::level_runtime::LevelRuntime;

#[derive(GodotClass)]
#[class(base = Node)]
pub struct RustGameController {
    base: Base<Node>,
    runtime: Option<LevelRuntime>,
}

#[godot_api]
impl INode for RustGameController {
    fn init(base: Base<Node>) -> Self {
        Self {
            base,
            runtime: None,
        }
    }

    fn ready(&mut self) {
        if let Err(err) = self.load_level_from_res_path("res://levels/level01.txt") {
            godot_error!("Failed to initialize level runtime: {err}");
            return;
        }

        godot_print!("RustGameController ready. Controls: WASD move, Z undo, R restart");
    }
}

#[godot_api]
impl RustGameController {
    #[func]
    fn handle_action(&mut self, action: GString) -> bool {
        let Some(runtime) = self.runtime.as_mut() else {
            return false;
        };

        let mut changed = false;
        let action_name = action.to_string();

        match action_name.as_str() {
            "move_up" => changed = runtime.try_move(GridPos::UP).is_some(),
            "move_down" => changed = runtime.try_move(GridPos::DOWN).is_some(),
            "move_left" => changed = runtime.try_move(GridPos::LEFT).is_some(),
            "move_right" => changed = runtime.try_move(GridPos::RIGHT).is_some(),
            "undo" => changed = runtime.undo_last_move(),
            "restart" => {
                runtime.restart();
                changed = true;
            }
            _ => {}
        }

        if changed {
            self.print_runtime_state();
        }

        changed
    }

    #[func]
    fn load_level(&mut self, level_res_path: GString) -> bool {
        match self.load_level_from_res_path(&level_res_path.to_string()) {
            Ok(()) => true,
            Err(err) => {
                godot_error!("{err}");
                false
            }
        }
    }

    fn load_level_from_res_path(&mut self, level_res_path: &str) -> Result<(), String> {
        let global_path = ProjectSettings::singleton()
            .globalize_path(level_res_path)
            .to_string();

        let parsed = parse_level_file(Path::new(&global_path))?;
        self.runtime = Some(LevelRuntime::new(parsed.into_game_state()));
        self.print_runtime_state();

        Ok(())
    }

    fn print_runtime_state(&self) {
        let Some(runtime) = self.runtime.as_ref() else {
            return;
        };

        godot_print!(
            "State p=({}, {}), crates={}, undo={}, complete={}",
            runtime.game_state.player_grid_pos.x,
            runtime.game_state.player_grid_pos.y,
            runtime.game_state.crate_positions.len(),
            runtime.undo_stack.len(),
            runtime.is_level_complete,
        );

        let _ = &self.base;
    }
}
