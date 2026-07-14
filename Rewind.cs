using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Rewind : Node
{
	// Called when the node enters the scene tree for the first time.

	List<Node2D> rewindables = new List<Node2D>();
	List<Vector2> previousPositions = new List<Vector2>();
	
	List<List<Vector2>> allPreviousPositions = new List<List<Vector2>>();
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
		rewindables = GetTree().GetNodesInGroup("Rewindable").OfType<Node2D>().ToList();
        Callable.From(TakeSnapShot).CallDeferred();
    }
    public override void _PhysicsProcess(double delta)
    {
		if (Input.IsActionJustPressed("undo"))
		{
			Undo();
		}
	}
	public void OnRewindSnapshotTimerTimeOut()
	{
		TakeSnapShot();
	}
	public void TakeSnapShot()
	{
		List<Vector2> positions = new List<Vector2>();
		for(int i = 0; i < rewindables.Count; i++)
		{
			positions.Add(rewindables[i].Position);
		}

		previousPositions = positions;
		allPreviousPositions.Add(positions);
		GD.Print(allPreviousPositions.Count);
	}
	public void Undo()
	{
        if (allPreviousPositions.Count == 0)
        {
            GD.Print("Reached the beginning of history!");
            return;
        }

        // 2. Get the very last saved snapshot (the top of our stack)
        List<Vector2> targetPositions = allPreviousPositions[allPreviousPositions.Count - 1];

        // 3. Apply those positions to your rewindable nodes
        for (int i = 0; i < rewindables.Count; i++)
        {
            // Safety check: make sure we don't index out of bounds if nodes changed
            if (i < targetPositions.Count)
            {
                rewindables[i].Position = targetPositions[i];
            }
        }

        // 4. Remove this state from our history so the next undo goes back further
        allPreviousPositions.RemoveAt(allPreviousPositions.Count - 1);

        // 5. Update previousPositions to point to the new "top" of our history stack
        if (allPreviousPositions.Count > 0)
        {
            previousPositions = allPreviousPositions[allPreviousPositions.Count - 1];
        }
        else
        {
            previousPositions = new List<Vector2>(); // History is completely empty now
        }
        if(allPreviousPositions.Count == 0)
        {
            TakeSnapShot();
        }
    }
}
