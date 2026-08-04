using Godot;
using System;

public partial class SignalBus : Node
{
	private static SignalBus instance;

    public override void _Ready()
    {
        if (instance != null && instance != this)
        {
            QueueFree();
            return;
        }
        instance = this;
    }
	public static Action OnPlayerJump;
	public static Action OnPlayerTouchedGround;
	public static void EmitPlayerJumped()
	{
		OnPlayerJump?.Invoke();
	}
}
