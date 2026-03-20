#[cfg(test)]
mod tests {
    use std::path::PathBuf;

    use crate::game_state::GameState;
    use crate::grid_pos::GridPos;
    use crate::level_catalog::{LEVEL_COUNT, LEVEL_FILES};
    use crate::level_loader::{parse_level_file, parse_level_text, LevelParseError};
    use crate::level_runtime::LevelRuntime;

    fn pos(x: i32, y: i32) -> GridPos {
        GridPos::new(x, y)
    }

    #[test]
    fn blocked_by_wall_does_not_change_state_or_undo() {
        let initial = GameState::new(pos(1, 1), [pos(2, 1)], [], []);
        let mut runtime = LevelRuntime::new(initial.clone());

        let result = runtime.try_move(GridPos::RIGHT);
        assert!(result.is_none());
        assert_eq!(runtime.game_state.player_grid_pos, initial.player_grid_pos);
        assert!(runtime.undo_stack.is_empty());
    }

    #[test]
    fn pushing_crate_into_wall_fails_and_does_not_record_undo() {
        let initial = GameState::new(pos(1, 1), [pos(3, 1)], [], [pos(2, 1)]);
        let mut runtime = LevelRuntime::new(initial.clone());

        let result = runtime.try_move(GridPos::RIGHT);
        assert!(result.is_none());
        assert_eq!(runtime.game_state.player_grid_pos, initial.player_grid_pos);
        assert_eq!(runtime.game_state.crate_positions, initial.crate_positions);
        assert!(runtime.undo_stack.is_empty());
    }

    #[test]
    fn successful_push_moves_player_and_crate_and_records_snapshot() {
        let initial = GameState::new(pos(1, 1), [], [], [pos(2, 1)]);
        let mut runtime = LevelRuntime::new(initial);

        let result = runtime.try_move(GridPos::RIGHT).expect("move should succeed");
        assert!(result.moved_crate);
        assert_eq!(runtime.game_state.player_grid_pos, pos(2, 1));
        assert!(runtime.game_state.crate_positions.contains(&pos(3, 1)));
        assert_eq!(runtime.undo_stack.len(), 1);
    }

    #[test]
    fn undo_restores_previous_snapshot() {
        let initial = GameState::new(pos(1, 1), [], [], [pos(2, 1)]);
        let mut runtime = LevelRuntime::new(initial.clone());

        runtime.try_move(GridPos::RIGHT).expect("move should succeed");
        assert!(runtime.undo_last_move());

        assert_eq!(runtime.game_state.player_grid_pos, initial.player_grid_pos);
        assert_eq!(runtime.game_state.crate_positions, initial.crate_positions);
        assert!(runtime.undo_stack.is_empty());
    }

    #[test]
    fn restart_resets_state_and_clears_undo() {
        let initial = GameState::new(pos(1, 1), [], [], [pos(2, 1)]);
        let mut runtime = LevelRuntime::new(initial.clone());

        runtime.try_move(GridPos::RIGHT).expect("move should succeed");
        runtime.restart();

        assert_eq!(runtime.game_state.player_grid_pos, initial.player_grid_pos);
        assert_eq!(runtime.game_state.crate_positions, initial.crate_positions);
        assert!(runtime.undo_stack.is_empty());
    }

    #[test]
    fn is_complete_requires_exact_goal_coverage() {
        let mut state = GameState::new(pos(0, 0), [], [pos(1, 1)], [pos(1, 1)]);
        assert!(state.is_complete());

        state.crate_positions.clear();
        assert!(!state.is_complete());

        let state = GameState::new(pos(0, 0), [], [pos(1, 1)], [pos(2, 2)]);
        assert!(!state.is_complete());
    }

    #[test]
    fn parse_rejects_non_rectangular_level() {
        let text = "#####\n#P..#\n####";
        let parsed = parse_level_text(text);
        assert!(matches!(parsed, Err(LevelParseError::NonRectangular)));
    }

    #[test]
    fn parse_rejects_invalid_symbol() {
        let text = "#####\n#PX.#\n#####";
        let parsed = parse_level_text(text);
        assert!(matches!(
            parsed,
            Err(LevelParseError::InvalidSymbol {
                symbol: 'X',
                x: 2,
                y: 1
            })
        ));
    }

    #[test]
    fn parse_accepts_level_pack_files() {
        assert_eq!(LEVEL_COUNT, 12);

        for level_file in LEVEL_FILES {
            let path = csharp_level_path(level_file);
            let parsed = parse_level_file(&path)
                .unwrap_or_else(|err| panic!("{} ({})", err, path.display()));

            assert_eq!(parsed.crate_positions.len(), parsed.goal_positions.len());

            let state = parsed.into_game_state();
            assert!(!state.wall_positions.is_empty());
        }
    }

    fn csharp_level_path(level_file: &str) -> PathBuf {
        let manifest_dir = PathBuf::from(env!("CARGO_MANIFEST_DIR"));
        manifest_dir
            .parent()
            .expect("rust dir has parent")
            .parent()
            .expect("workspace root exists")
            .join("grid-shift-csharp")
            .join("levels")
            .join(level_file)
    }
}
