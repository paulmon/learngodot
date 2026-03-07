# Grid Shift (C# / Godot 4)

This is the C# implementation of the Sokoban-like learning project.

## Goal
Ship a complete playable version in Godot C# first, then rebuild with Rust at feature parity.

## Immediate Bootstrap
1. Open this folder in Godot 4 .NET build.
2. Create `project.godot` in this folder (new project).
3. Set input actions in Project Settings:
   - `move_up`
   - `move_down`
   - `move_left`
   - `move_right`
   - `undo`
   - `restart`
4. Create baseline scene: `scenes/Main.tscn`.
5. Add first C# script and confirm build succeeds.

## Folder Layout
- `scenes/` scene files
- `scripts/` C# scripts
- `levels/` level data/resources
- `audio/` sound effects and music
- `art/` sprites and tiles
