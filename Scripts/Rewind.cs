using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Rewind : Node
{
    public static event Action OnUndo;

    Timer snapshotTimer;
	List<Node2D> rewindables = new List<Node2D>();

    List<RewindData> previousRewinds = new List<RewindData>();
	List<List<RewindData>> allPreviousRewinds = new List<List<RewindData>>();
	
	List<List<Vector2>> allPreviousPositions = new List<List<Vector2>>();

    public override void _EnterTree()
    {
        Player.OnJumped += TakeSnapShot;
    }
    public override void _ExitTree()
    {
        Player.OnJumped -= TakeSnapShot;
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
	private void OnRewindSnapshotTimerTimeOut()
	{
		TakeSnapShot();
	}

	private void TakeSnapShot()
	{
        GD.Print("Took snapshot");
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
    //FIXME:

    private void Undo()
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
        OnUndo?.Invoke();
    }
}
