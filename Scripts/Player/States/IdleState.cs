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

        player.velocity.Y = 0;
        if (player.jumpBufferTimer > 0)
        {
            player.ChangeState(new JumpState());
            return;
        }

        if (player.inputAxis != 0)
        {
            player.ChangeState(new WalkState());
            return;
        }
        if (!player.IsOnFloor())
        {
            player.ChangeState(new FallState());
            return;
        }
    }
}