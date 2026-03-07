# Dev Notes

Date: 2026-03-06
Project: Grid Shift (Godot 4 C#)

## Scene Tree Notes
- `Main` scene root is `Node2D` (`scenes/Main.tscn`).
- `Player` uses a script-driven `Node2D` (`scripts/PlayerController.cs`) for tile movement.
- Static groups are organized under `Walls`, `Crates`, and `Goals` roots.
- `UI/WinLabel` is used for simple completion feedback.
- `GameManager` coordinates level progression and reloads through `PlayerController.LoadLevel(...)`.

## Signal Notes
- `PlayerController` emits `LevelCompleted` when all goals are covered.
- `GameManager` subscribes to `PlayerController.LevelCompleted` in `_Ready()` and advances to the next level.
- Current signal flow validates end-to-end wiring: gameplay state -> signal emit -> manager callback -> next level load.

## Resource and Data Notes
- Levels are loaded from text data files in `levels/`.
- Current symbols:
  - `#` wall
  - `P` player start
  - `C` crate
  - `G` goal
  - `*` crate on goal
  - `.` or space for floor
- Level validation currently checks:
  - Rectangular map shape
  - Fully walled border
  - Exactly one `P`
  - Crate count equals goal count
  - No unsupported symbols
