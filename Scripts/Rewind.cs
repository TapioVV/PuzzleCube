using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Rewind : Node
{
    // Called when the node enters the scene tree for the first time.


    [Export] Player player;
    Timer snapshotTimer;
    //Rewindable[] rewindablesArray;
	List<Node2D> rewindables = new List<Node2D>();
	// List<Vector2> previousPositions = new List<Vector2>();

    List<RewindData> previousRewinds = new List<RewindData>();
	List<List<RewindData>> allPreviousRewinds = new List<List<RewindData>>();
	
	List<List<Vector2>> allPreviousPositions = new List<List<Vector2>>();
    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _EnterTree()
    {
        SignalBus.OnPlayerJump += TakeSnapShot;
    }
    public override void _ExitTree()
    {
        SignalBus.OnPlayerJump -= TakeSnapShot;
    }


    public override void _Ready()
    {
        snapshotTimer = GetNode<Timer>("%RewindSnapShotTimer");
		rewindables = GetTree().GetNodesInGroup("Rewindable").OfType<Node2D>().ToList();
        //rewindablesArray = new Rewindable[ GetTree().GetNodeCountInGroup("Rewindable")];
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
	// public void TakeSnapShot()
	// {
    //     GD.Print("took snapshot");
	// 	List<Vector2> positions = new List<Vector2>();
	// 	for(int i = 0; i < rewindables.Count; i++)
	// 	{
	// 		positions.Add(rewindables[i].GlobalPosition);
	// 	}

	// 	previousPositions = positions;
	// 	allPreviousPositions.Add(positions);
    //     snapshotTimer.Start();
	// }
	public void TakeSnapShot()
	{
		List<RewindData> rewindDatas = new List<RewindData>();
		for(int i = 0; i < rewindables.Count; i++)
		{
            RewindData data = new RewindData();
            Node2D rewindable = rewindables[i];
            data.Position = rewindable.GlobalPosition;
            data.Parent = rewindable.GetParent<Node2D>();
            data.Rotation = rewindable.Rotation;
            if(rewindables[i] is ExtendedCharacterBody2D body)
            {
                data.GravityDirection = body.GravityDirection;
                data.CurrentFace = body.CurrentFace;
            }
			rewindDatas.Add(data);
		}
        previousRewinds = rewindDatas;
        allPreviousRewinds.Add(rewindDatas);
        snapshotTimer.Start();
	}
    public void Undo()
	{
        if (allPreviousRewinds.Count == 0)
        {
            GD.Print("Reached the beginning of history!");
            return;
        }

        // 2. Get the very last saved snapshot (the top of our stack)
        List<RewindData> targetDatas = allPreviousRewinds[allPreviousRewinds.Count - 1];

        for (int i = 0; i < rewindables.Count; i++)
        {
            rewindables[i].ProcessMode = ProcessModeEnum.Disabled;
        }
        // 3. Apply those positions to your rewindable nodes
        for (int i = 0; i < rewindables.Count; i++)
        {
            // Safety check: make sure we don't index out of bounds if nodes changed
            if (i < targetDatas.Count)
            {
                RewindData data = targetDatas[i];
                Node2D rewindable = rewindables[i];
                rewindable.GlobalPosition = data.Position;
                rewindable.Reparent(data.Parent);
                rewindable.Rotation = data.Rotation;
                if(rewindable is ExtendedCharacterBody2D body)
                {
                    body.GravityDirection = data.GravityDirection;
                    body.CurrentFace = data.CurrentFace;
                }
            }
        }

        for (int i = 0; i < rewindables.Count; i++)
        {
            rewindables[i].ProcessMode = ProcessModeEnum.Inherit;
        }
        // 4. Remove this state from our history so the next undo goes back further
        allPreviousRewinds.RemoveAt(allPreviousRewinds.Count - 1);

        // 5. Update previousPositions to point to the new "top" of our history stack
        if (allPreviousRewinds.Count > 0)
        {
            previousRewinds = allPreviousRewinds[allPreviousRewinds.Count - 1];
        }
        else
        {
            previousRewinds = new List<RewindData>(); // History is completely empty now
        }
        if(allPreviousRewinds.Count == 0)
        {
            TakeSnapShot();
        }
    }
	// public void Undo()
	// {
    //     if (allPreviousPositions.Count == 0)
    //     {
    //         GD.Print("Reached the beginning of history!");
    //         return;
    //     }

    //     // 2. Get the very last saved snapshot (the top of our stack)
    //     List<Vector2> targetPositions = allPreviousPositions[allPreviousPositions.Count - 1];

    //     for (int i = 0; i < rewindables.Count; i++)
    //     {
    //         rewindables[i].ProcessMode = ProcessModeEnum.Disabled;
    //     }
    //     // 3. Apply those positions to your rewindable nodes
    //     for (int i = 0; i < rewindables.Count; i++)
    //     {
    //         // Safety check: make sure we don't index out of bounds if nodes changed
    //         if (i < targetPositions.Count)
    //         {
    //             rewindables[i].GlobalPosition = targetPositions[i];
    //         }
    //     }

    //     for (int i = 0; i < rewindables.Count; i++)
    //     {
    //         rewindables[i].ProcessMode = ProcessModeEnum.Inherit;
    //     }
    //     // 4. Remove this state from our history so the next undo goes back further
    //     allPreviousPositions.RemoveAt(allPreviousPositions.Count - 1);

    //     // 5. Update previousPositions to point to the new "top" of our history stack
    //     if (allPreviousPositions.Count > 0)
    //     {
    //         previousPositions = allPreviousPositions[allPreviousPositions.Count - 1];
    //     }
    //     else
    //     {
    //         previousPositions = new List<Vector2>(); // History is completely empty now
    //     }
    //     if(allPreviousPositions.Count == 0)
    //     {
    //         TakeSnapShot();
    //     }

    // }
}
