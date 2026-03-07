# Playtest Notes

Date: 2026-03-06
Build: `dotnet build ./grid-shift-csharp/grid-shift.sln` (success)
Levels tested: `level01.txt`, `level02.txt`

## Test Pass Summary
- Movement input (`W/A/S/D`) moves one grid tile per input.
- Walls block movement as expected.
- Crate push works only for one crate and only into free space.
- Pushing a crate into wall/crate is correctly blocked.
- `undo` (`Z`) reverts successful moves in sequence.
- `restart` (`R`) resets level state.
- Win condition triggers when all goals are filled.
- Level progression advances from level01 to level02 on completion.

## Observations
- Current flow is clear for a prototype vertical slice.
- Validation warnings should be checked in Godot output when testing malformed level files.

## Follow-up
- Add more level content and repeat pass with 6+ levels.
- Add a lightweight regression checklist for each content change.
