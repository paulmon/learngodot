# Learn Godot by Building One Puzzle Game Twice (C# then Rust)

## 1. Project Scope

- Project type: 2D Sokoban-like puzzle game
- Goal: Learn Godot by shipping one complete small game in C#, then rebuilding the same game in Rust with feature parity
- Experience baseline: Comfortable coder, new to game development
- Time budget: 5-8 hours/week
- Timeline: Flexible (progress by milestones, not calendar pressure)
- Plan format: Task list

## 2. Learning Outcomes

- Use Godot editor effectively (scenes, nodes, signals, resources, input map)
- Build core puzzle game systems end-to-end
- Ship a playable version with menus, save data, and basic polish
- Translate architecture from C# to Rust while preserving behavior
- Compare productivity, performance, and maintainability across both languages

## 3. Game Definition (Target for Both Versions)

- Working title: `Grid Shift`
- Genre: 2D single-screen Sokoban-like tile puzzle
- Core loop:
	- Move player on a grid
	- Push crates onto goal tiles
	- Avoid deadlocks and reposition carefully
	- Open exit door when all goals are satisfied
	- Complete level and unlock next
- Minimum feature set (must exist in both C# and Rust versions):
	- 12 levels in a level pack
	- Standard Sokoban crate/goal rules
	- Deadlock-aware level design (no impossible mandatory states in intended solutions)
	- Undo (single-step and multi-step)
	- Restart level
	- Main menu, level select, pause menu
	- Save progress (highest unlocked level)
	- Sound effects + background music
	- Basic win/lose feedback and transitions

## 4. Sokoban Level Design Rules

- [ ] Keep grid readability high: clear walls, floor, crate, goal, and player silhouettes
- [ ] Match counts: number of crates must equal number of required goal tiles
- [ ] Enforce push-only logic: no mechanic should effectively allow pulling unless intentionally introduced later
- [ ] Avoid unavoidable dead corners for mandatory crate paths unless clearly optional
- [ ] Guarantee player traversal space around critical interaction zones
- [ ] Design one teaching objective per level first, then add one secondary twist at most
- [ ] Escalate difficulty gradually:
	- Level 1-3: single-crate fundamentals
	- Level 4-6: corridor pressure and box ordering
	- Level 7-9: multi-crate dependency
	- Level 10-12: tight-space endgame planning
- [ ] Limit solution length early; keep first few levels solvable in under 20 moves
- [ ] Require at least one meaningful reversal to validate undo value in mid/late levels
- [ ] Test for soft locks and false complexity (annoying trial-and-error without insight)
- [ ] Verify every shipped level with a recorded reference solution path

## 5. Technical Boundaries

- Engine: Godot 4.x (stable)
- Rendering: 2D only
- Input: Keyboard first; optional gamepad later
- Art: Temporary placeholder assets allowed for first pass
- Scope control rule: No new mechanics until all baseline systems are complete

## 6. Task Plan A: Build the Full Game in C# First

### A0. Environment Setup

- [x] Install Godot 4.x (standard .NET build)
- [x] Install .NET SDK compatible with Godot version
- [x] Create project: `grid-shift-csharp`
- [x] Configure source control and `.gitignore`
- [x] Verify scripts compile and run in editor

### A1. Godot Fundamentals Through a Tiny Prototype

- [x] Create a test scene with `Node2D`, `TileMapLayer`, and `CharacterBody2D` (or equivalent movement node)
- [x] Implement grid movement with input actions (`move_up/down/left/right`)
- [x] Wire one signal end-to-end (button or area trigger)
- [x] Load one level scene and reset it
- [x] Write short dev notes on scene tree, signals, and resources

### A2. Vertical Slice (One Complete Puzzle Level)

- [x] Implement tile-based collision and movement rules
- [x] Add Sokoban crate push mechanic (push-only, no pulling)
- [x] Add goal tiles and "all crates on goals" win condition
- [x] Add victory trigger and level-complete screen
- [x] Add restart and undo (at least 1 step)
- [x] Playtest until the slice feels clear and reliable

### A3. Core Systems

- [x] Refactor to reusable game state model (player position, crate states, goal states)
- [x] Implement multi-step undo stack
- [x] Implement level loader from data files/resources
- [x] Implement simple level validation checks (crate count matches goal count, reachable layout sanity)
- [x] Add game manager for level progression

### A4. Content Production

- [x] Design 12 levels with gradual mechanic difficulty
- [x] Categorize levels by concept (intro push, corridor pressure, multi-crate planning, tight-space endgame)
- [x] Run playtesting pass for soft locks and ambiguity
- [x] Tune at least 3 levels based on test feedback

### A5. UX and Polish

- [x] Main menu + level select + pause menu
- [x] Save/load unlocked progress
- [x] Add SFX hooks for move, crate push, goal complete, win, fail
- [x] Add background music loop
- [x] Add simple transitions and visual feedback for interactions

### A6. Stability and Ship Readiness

- [x] Create test checklist (movement, crate push rules, undo, reset, completion, save/load)
- [x] Run full pass on all 12 levels
- [x] Fix high-severity bugs first (crashes, progression blockers, bad saves)
- [x] Build exportable desktop version
- [x] Write short postmortem: what was easy/hard in C# + Godot

## 7. Task Plan B: Rebuild the Same Game in Rust (Feature Parity)

### B0. Rust Tooling Setup

- [x] Create project: `grid-shift-rust`
- [x] Install Rust toolchain (`rustup`, stable toolchain)
- [x] Add and configure Godot Rust bindings for Godot 4
- [x] Verify Godot can load Rust extension and run a simple scripted node

### B1. Porting Strategy Before Coding

- [x] Freeze C# version features (no new mechanics)
- [x] Document architecture map:
	- C# class -> Rust struct/module equivalent
	- Signals/events -> Rust callback/event wiring
	- Resource/data formats shared across versions
- [x] Define parity checklist from the C# version

### B2. Re-implement Core Gameplay

- [ ] Rebuild grid movement and collision logic in Rust
- [ ] Rebuild crate pushing and goal-completion logic
- [ ] Rebuild victory conditions, restart, and undo stack
- [ ] Confirm behavior matches C# version level-by-level

### B3. Re-implement Systems and Content

- [ ] Rebuild level loading and progression logic
- [ ] Rebuild menus and pause flow
- [ ] Rebuild save/load progress
- [ ] Reconnect audio and transitions
- [ ] Run through all 12 levels for parity verification

### B4. Rust-Focused Refinement

- [ ] Improve ownership/borrowing design where code feels awkward
- [ ] Reduce unnecessary cloning and dynamic allocations
- [ ] Add lightweight profiling checkpoints (frame consistency in puzzle scenes)
- [ ] Document Rust-specific lessons and patterns for future Godot projects

## 8. Definition of Done

- [ ] C# version and Rust version both playable from start to finish
- [ ] Both versions include all minimum features listed in Section 3
- [ ] No progression blockers across all 12 levels
- [ ] Save/load works reliably in both versions
- [ ] Basic polish present (audio, feedback, menus)
- [ ] Final comparison notes completed (developer experience, speed, maintainability)

## 9. Suggested Weekly Cadence (Flexible)

- [ ] Week focus template:
	- 1 setup/learning block
	- 2 implementation blocks
	- 1 testing/refactor block
- [ ] End every week with:
	- Short demo recording (1-3 min)
	- Updated bug/parity checklist
	- One written lesson learned

## 10. Risk Management

- [ ] Risk: Scope creep from adding mechanics too early
	- Mitigation: Enforce baseline-only rule until both versions reach parity
- [ ] Risk: Rust interop/tooling friction
	- Mitigation: Build tiny Rust spike first and lock working toolchain versions
- [ ] Risk: Puzzle design consumes too much time
	- Mitigation: Start with 6 strong levels, then expand to 12 after systems stabilize
- [ ] Risk: Undo system complexity
	- Mitigation: Use explicit state snapshots first, optimize later

## 11. Immediate Next Actions

- [x] Install Godot 4.x .NET and verify C# script execution
- [x] Create `grid-shift-csharp` repo and baseline scene
- [x] Implement grid movement + restart in one test level
- [x] Confirm this baseline before touching Rust setup
- [x] Build exportable desktop version (close out A6)
- [x] Start Rust phase B0: create `grid-shift-rust` and validate a simple extension node in Godot
- [x] Start Rust phase B1: freeze C# features and document C# to Rust architecture mapping
- [ ] Start Rust phase B2: port `GameState` + movement/push + undo/restart behavior
