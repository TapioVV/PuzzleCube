using Godot;
using System;

public partial class IdleState : State
{
    public override void Enter()
    {
        player.velocity.X = 0;
        return;
        
    }
    public override void Exit() 
    {
        return;
    }
    public override void Update(float deltaf)
    {
        //animator.CrossFade("character_idle_animation", 0, 0);


        if(Input.IsActionPressed("down") && Input.IsActionJustPressed("jump"))
        {
            player.SetCollisionMaskValue(4, false);
            player.passablePlatformTimer = player.passablePlatformTime;
            player.ChangeState(new FallState());
            return;
        }

        player.velocity.Y = 0;
        
        if(player.velocity.X != 0)
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, 0, player.horizontalDeacceleration * deltaf);
        }
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