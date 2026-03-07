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

## Docs
- `docs/dev-notes.md`: implementation notes on scene tree, signals, and level resources.
- `docs/playtest-notes.md`: current prototype playtest results and follow-up checks.

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

## Development Workflow

### Daily Loop
1. Open Godot editor for the C# project:

```powershell
./open-csharp-project.ps1
```

2. Make gameplay/content changes in `grid-shift-csharp/`.
3. Run and validate behavior in Godot editor.
4. Build from terminal to catch compile errors early:

```powershell
dotnet build ./grid-shift-csharp/grid-shift.sln
```

### Useful Commands

Check .NET SDK:

```powershell
dotnet --version
```

Build C# project directly:

```powershell
dotnet build ./grid-shift-csharp/grid-shift.csproj
```

### Pre-Commit Checklist
- C# project builds without errors.
- Core controls (`move_*`, `undo`, `restart`) still work.
- No regression in crate push, undo, restart, and win condition.
- Progression/save behavior still works for tested levels.

## Core Constraints
- Tile/grid logic uses integer grid coordinates.
- Crates are push-only, one at a time.
- Undo uses full state snapshots for successful moves only.
- Win condition: every goal tile has exactly one crate.

## Roadmap
- Phase A: ship the full C# version with 12 levels and complete UX loop.
- Phase B: rebuild in Rust with behavior parity.

See `PRD.md` for the complete checklist and milestones.
