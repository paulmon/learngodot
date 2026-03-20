# B1 Porting Strategy (C# -> Rust)

Date: 2026-03-19
Project: Grid Shift

## 1. Feature Freeze (C# Baseline Lock)

- C# implementation in `grid-shift-csharp/` is now the parity baseline.
- No new gameplay mechanics should be added to C# until Rust reaches parity.
- Allowed C# changes before parity:
  - Critical bug fixes (crash, blocker, corrupt save).
  - Build/export fixes.
  - Clarifying docs and tests.
- Any behavior change in C# must also update this document and the parity checklist below.

## 2. Architecture Map

### Core Model and Rules

- `scripts/GameState.cs` -> `rust/src/game_state.rs`
  - Owns canonical grid state: player position, wall set, goal set, crate set.
  - Applies movement/push rules (`TryApplyMove`).
  - Evaluates win condition (`IsComplete`).

- `scripts/PlayerController.cs` -> `rust/src/player_controller.rs`
  - Input boundary (`move_*`, `undo`, `restart`).
  - Parses level text and validates constraints.
  - Builds scene nodes for walls/crates/goals.
  - Owns undo stack of full snapshots.

### Progression and Session

- `scripts/GameManager.cs` -> `rust/src/game_manager.rs`
  - Loads requested level index from session + save cap.
  - Subscribes to level completion and advances progression.
  - Handles restart/level-select/main-menu scene transitions.

- `scripts/LevelCatalog.cs` -> `rust/src/level_catalog.rs`
  - Source of truth for ordered level file list.

- `scripts/LevelSession.cs` -> `rust/src/level_session.rs`
  - Carries selected start level index across scenes.

- `scripts/SaveData.cs` -> `rust/src/save_data.rs`
  - Persists `highest_unlocked_level_index` in `user://save_data.json`.

### UI and Menus

- `scripts/MainMenuController.cs` -> `rust/src/ui/main_menu_controller.rs`
- `scripts/LevelSelectController.cs` -> `rust/src/ui/level_select_controller.rs`
- `scripts/PauseMenuController.cs` -> `rust/src/ui/pause_menu_controller.rs`
  - Keep same scene flow and pause behavior.

### Audio and Bootstrap

- `scripts/AudioManager.cs` -> `rust/src/audio_manager.rs`
  - BGM playback and SFX methods (`move/push/goal/win/fail`).

- `scripts/GameBootstrap.cs` -> `rust/src/game_bootstrap.rs`
  - Ensures input map actions exist and applies fade-in tween.

## 3. Signals/Events Wiring Map

- C# signal: `PlayerController.LevelCompleted` -> Rust signal on `PlayerController` class.
- C# subscription: `GameManager._Ready()` subscribes to player completion event -> Rust `GameManager` must subscribe during ready/init lifecycle.
- UI button `Pressed` callbacks in menu/pause controllers -> Rust equivalents on the same scene node paths.
- Pause input (`pause` action) currently handled in `PauseMenuController._UnhandledInput` with `ProcessModeEnum.Always` -> preserve this behavior.

## 4. Shared Data and Resource Formats

- Level files remain text in `levels/level01.txt` .. `level12.txt`.
- Symbols and meaning must stay identical:
  - `#` wall
  - `P` player start
  - `C` crate
  - `G` goal
  - `*` crate on goal
  - `.` or space floor
- Level validation invariants to preserve:
  - Rectangular map.
  - Fully walled border.
  - Exactly one `P`.
  - Crate count equals goal count.
  - Reject unsupported symbols.
- Save format remains JSON key `highest_unlocked_level_index`.
- Input action names remain:
  - `move_up`, `move_down`, `move_left`, `move_right`, `undo`, `restart`, `pause`.

## 5. Parity Checklist (Rust vs C#)

### System Parity

- [ ] Grid movement blocked by walls.
- [ ] Push-only crate movement (no pulling, no chain push).
- [ ] Failed move/push does not mutate state and does not push undo snapshot.
- [ ] Successful move pushes one full-state undo snapshot.
- [ ] Undo restores player + all crate positions.
- [ ] Restart restores initial level state and clears undo history.
- [ ] Win triggers only when every goal has exactly one crate.
- [ ] Level completion advances progression and unlocks next level.
- [ ] Save/load highest unlocked level works across restarts.
- [ ] Main menu, level select, pause menu behavior matches C#.
- [ ] BGM and SFX hooks fire on matching events.

### Content Parity

- [x] Rust loader accepts all 12 existing level files without format changes.
- [ ] Rust run-through completed for levels 1-12 with no progression blockers.

### Scene/Flow Parity

- [ ] Main menu -> Start launches level flow at allowed index.
- [ ] Level select disables locked levels.
- [ ] Pause menu toggles with `Esc` and resumes correctly.
- [ ] Main -> Level Select -> Main transitions preserve expected session index.

## 6. Execution Order for B2

1. Port `GameState` and deterministic movement/push logic.
2. Port `PlayerController` level parse/load + undo/restart.
3. Port `GameManager` progression + save/load integration.
4. Port menu and pause controllers.
5. Reconnect audio manager.
6. Run parity checklist and resolve deviations.
