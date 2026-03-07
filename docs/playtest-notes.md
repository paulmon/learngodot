# Playtest Notes

Date: 2026-03-06
Build: `dotnet build ./grid-shift-csharp/grid-shift.sln` (success)
Levels tested: `level01.txt` through `level12.txt`

## Test Pass Summary
- Movement input (`W/A/S/D`) moves one grid tile per input.
- Walls block movement as expected.
- Crate push works only for one crate and only into free space.
- Pushing a crate into wall/crate is correctly blocked.
- `undo` (`Z`) reverts successful moves in sequence.
- `restart` (`R`) resets level state.
- Win condition triggers when all goals are filled.
- Level progression advances through the full configured level list.

## Soft-Lock and Ambiguity Pass (Level Pack)
- Reviewed all 12 layouts for avoidable dead-corner pressure, corridor readability, and ambiguous routing.
- Validated wall enclosure and crate-goal parity for each level file.
- Focused tuning pass completed on three levels with highest ambiguity risk:
	- `level05.txt`
	- `level08.txt`
	- `level11.txt`

## Observations
- Early levels (1-3) teach push fundamentals clearly.
- Mid-pack levels now provide clearer route planning and less accidental trap pressure after tuning.
- Endgame levels (10-12) preserve challenge while keeping goal paths legible.

## Tuned Level Changes
- `level05.txt`: reduced abrupt choke pattern and improved corridor readability around the second crate route.
- `level08.txt`: clarified multi-crate staging lanes and reduced conflicting push paths in the center-right section.
- `level11.txt`: added route constraint to stabilize endgame ordering and reduce ambiguous late pushes.

## Follow-up
- Re-run quick regression checks on tuned levels whenever push/undo logic changes.
- Add recorded reference solution paths per level as content lock approaches.
