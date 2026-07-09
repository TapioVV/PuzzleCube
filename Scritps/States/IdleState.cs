using Godot;
using System;

public partial class IdleState : State
{
    public override void Enter()
    {
        return;
        
    }
    public override void Exit() 
    {
        return;
    }
    public override void Update(float deltaf)
    {
        //animator.CrossFade("character_idle_animation", 0, 0);

        player.velocity.Y = 2;
        if (Input.IsActionJustPressed("jump"))
        {
            player.ChangeState(new JumpState());
        }

        if (player.inputAxis != 0)
        {
            player.ChangeState(new WalkState());
        }
        if (!player.IsOnFloor())
        {
            player.ChangeState(new FallState());
        }
    }

    public override void JumpInput()
    {
        return;
        //player.Jump(1f);
        //player.OnPlayerNormalJump.Invoke();

        //player.CurrentState = player.jumpState;
    }
}