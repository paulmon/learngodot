# Copilot Instructions

## Project Overview

**Grid Shift** — a 2D Sokoban-like tile puzzle game built twice:
1. **Phase A**: `grid-shift-csharp/` — Godot 4.x with C# (.NET)
2. **Phase B**: `grid-shift-rust/` — Same game rebuilt with Rust via `gdext` (godot-rust bindings for Godot 4)

The Rust version must reach full feature parity with the C# version before any new mechanics are added to either.

## Architecture

### Core Game Model
All game state lives in an explicit, serializable model — not scattered across nodes:
- Player position (grid coordinates)
- Crate positions (array/list of grid coordinates)
- Goal tile positions (static per level)
- Undo stack: array of full state snapshots (not delta diffs — snapshots first, optimize later)

### Scene / Node Structure (Godot 4)
- One scene per level, loaded dynamically by a **Game Manager** autoload/singleton
- `TileMapLayer` handles the static grid (walls, floors, goal tiles)
- Player and crates are separate nodes positioned at grid coordinates (not physics-driven)
- Signals wire UI events (win, pause, undo, restart) to the game manager

### Level Format
Levels are defined as data resources (not hard-coded in scenes). The loader validates that crate count == goal count before play begins.

### Progression
Save data tracks only the highest unlocked level index. No per-level scores or timers.

## Key Conventions

### Grid Coordinate System
Movement is tile-based. Convert between world position and grid coords at the boundary (input/render); all game logic operates on integer grid coordinates only.

### Crate Push Rules
- Player pushes one crate at a time (no chain pushes)
- A push is only valid if the cell behind the crate is empty (not a wall, not another crate)
- No pulling

### Undo
Use explicit full-state snapshots. Each player move (including failed pushes that don't move the player) should **not** be recorded — only successful state-changing moves are pushed to the undo stack.

### Win Condition
Checked after every move: all goal tiles must have exactly one crate on them. No partial credit; the exit only opens when all goals are satisfied.

### Phase B Parity Rule
During Rust re-implementation, **freeze the C# version** (no new mechanics). Verify behavior level-by-level against the C# version before adding any Rust-specific improvements.

## Godot 4 .NET Setup

- Use the **standard .NET build** of Godot 4.x (not the non-.NET build)
- Compatible .NET SDK version must match the Godot version's requirements
- Input actions defined in Project Settings: `move_up`, `move_down`, `move_left`, `move_right`, `undo`, `restart`

## Rust / gdext Setup

- Crate: `gdext` (the `godot` crate on crates.io — Godot 4 Rust bindings)
- Lock working toolchain versions early (`rust-toolchain.toml`) to avoid gdext/Godot API breakage
- Validate the extension loads a simple scripted node before porting any game logic

## Minimum Feature Set (Both Versions)

These must all exist before either version is considered done:
- 12 levels with gradual difficulty
- Undo (single-step and multi-step)
- Restart level
- Main menu, level select, pause menu
- Save/load (highest unlocked level)
- Sound effects + background music
- Win/lose feedback and scene transitions
