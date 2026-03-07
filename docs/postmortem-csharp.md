# Postmortem: C# + Godot

Date: 2026-03-06
Project: Grid Shift (Phase A)

## What Was Easy
- Rapid iteration on gameplay rules with C# scripts and immediate scene feedback
- Input mapping and event handling were straightforward to wire
- Data-driven levels made content iteration faster once the loader existed
- Building/checking with `dotnet build` fit naturally into the workflow

## What Was Hard
- Keeping scene node state and game state synchronized during undo/restart
- Preventing regressions while adding menus, saves, and progression in parallel
- Tuning level layouts to reduce ambiguous/accidental dead-end pushes
- Verifying export setup from CLI without preset scaffolding

## What Helped Most
- An explicit `GameState` model as source of truth
- Snapshot-based undo (simple and reliable first)
- Incremental docs (`dev-notes`, `playtest-notes`, `level-catalog`)
- Frequent build checks after each behavior change

## Lessons for Phase B (Rust)
- Preserve model-first architecture from day one
- Port behavior in strict parity order (movement -> push -> undo -> progression)
- Keep level data format unchanged to simplify parity testing
- Add parity checklist before optimization work

## Next Improvements
- Finalize export preset and produce desktop artifact
- Add recorded reference solution paths for each level
- Tune audio mix and replace placeholder SFX where needed
