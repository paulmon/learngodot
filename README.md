# Learn Godot: Grid Shift (C# then Rust)

Build one complete Sokoban-like puzzle game twice:
1. `grid-shift-csharp/` using Godot 4 + C#
2. `grid-shift-rust/` using Godot 4 + Rust (`gdext`)

The Rust version should reach feature parity with the C# version before adding new mechanics.

## Project Status
- C# project exists in `grid-shift-csharp/`
- Rust project is planned for Phase B
- Product requirements are documented in `PRD.md`

## Repository Layout
- `PRD.md`: full product and milestone plan
- `open-csharp-project.ps1`: helper script to open the C# project
- `grid-shift-csharp/`: Godot C# implementation

## Quick Start (C#)
1. Install Godot 4.x .NET build.
2. Ensure .NET SDK is installed (`dotnet --version`).
3. From the repo root, run:

```powershell
./open-csharp-project.ps1
```

4. In Godot, verify input actions are configured:
- `move_up`
- `move_down`
- `move_left`
- `move_right`
- `undo`
- `restart`

## Core Constraints
- Tile/grid logic uses integer grid coordinates.
- Crates are push-only, one at a time.
- Undo uses full state snapshots for successful moves only.
- Win condition: every goal tile has exactly one crate.

## Roadmap
- Phase A: ship the full C# version with 12 levels and complete UX loop.
- Phase B: rebuild in Rust with behavior parity.

See `PRD.md` for the complete checklist and milestones.
