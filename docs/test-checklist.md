# Test Checklist (A6)

Date: 2026-03-06
Project: Grid Shift C# (Godot 4)

## Core Controls
- [ ] `W/A/S/D` moves exactly one tile per input
- [ ] `Esc` toggles pause menu in every level
- [ ] `Z` undo restores the immediate previous successful state
- [ ] `R` restart restores level start state

## Sokoban Rules
- [ ] Player cannot walk through walls
- [ ] Player can push exactly one crate at a time
- [ ] Push fails when target tile is wall or crate
- [ ] No pulling behavior exists

## Win/Progression
- [ ] Win triggers only when all goals are occupied
- [ ] Current level label updates correctly (`Level X/12`)
- [ ] Completing a level advances to next level
- [ ] Completing level N unlocks level N+1 in level select

## Save/Load
- [ ] Highest unlocked level persists after app restart
- [ ] Locked levels remain disabled in level select
- [ ] Start from main menu always begins at level 1

## Menus and Flow
- [ ] Main menu buttons (`Start`, `Level Select`, `Quit`) work
- [ ] Pause menu buttons (`Resume`, `Restart`, `Level Select`, `Main Menu`) work
- [ ] Level select `Back` returns to main menu

## Audio
- [ ] BGM starts when gameplay scene loads
- [ ] Move/push/goal/win/fail SFX trigger at expected events
- [ ] Audio levels are audible but not clipping

## Build and Stability
- [ ] `dotnet build ./grid-shift-csharp/grid-shift.sln` succeeds
- [ ] No compile/lint errors in workspace
- [ ] No progression blockers in level sequence
