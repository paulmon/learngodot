using Godot;
using System.Collections.Generic;

public sealed class GameState
{
    public Vector2I PlayerGridPos { get; set; }
    public HashSet<Vector2I> WallPositions { get; }
    public HashSet<Vector2I> GoalPositions { get; }
    public HashSet<Vector2I> CratePositions { get; }

    public GameState(
        Vector2I playerGridPos,
        IEnumerable<Vector2I> wallPositions,
        IEnumerable<Vector2I> goalPositions,
        IEnumerable<Vector2I> cratePositions)
    {
        PlayerGridPos = playerGridPos;
        WallPositions = new HashSet<Vector2I>(wallPositions);
        GoalPositions = new HashSet<Vector2I>(goalPositions);
        CratePositions = new HashSet<Vector2I>(cratePositions);
    }

    public GameState Clone()
    {
        return new GameState(PlayerGridPos, WallPositions, GoalPositions, CratePositions);
    }

    public bool TryApplyMove(Vector2I delta, out bool movedCrate, out Vector2I crateFrom, out Vector2I crateTo)
    {
        movedCrate = false;
        crateFrom = Vector2I.Zero;
        crateTo = Vector2I.Zero;

        var target = PlayerGridPos + delta;
        if (WallPositions.Contains(target))
            return false;

        if (CratePositions.Contains(target))
        {
            var pushTarget = target + delta;
            if (WallPositions.Contains(pushTarget) || CratePositions.Contains(pushTarget))
                return false;

            CratePositions.Remove(target);
            CratePositions.Add(pushTarget);
            movedCrate = true;
            crateFrom = target;
            crateTo = pushTarget;
        }

        PlayerGridPos = target;
        return true;
    }

    public bool IsComplete()
    {
        if (GoalPositions.Count == 0 || CratePositions.Count != GoalPositions.Count)
            return false;

        foreach (var goal in GoalPositions)
        {
            if (!CratePositions.Contains(goal))
                return false;
        }

        return true;
    }
}
