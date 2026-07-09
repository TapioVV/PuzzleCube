using Godot;
using System;

public partial class JumpState : State
{
    float timer = 0.1f;
    public override void Enter()
    {
        timer = 0.1f;
        player.velocity.Y = player.jumpSpeed;
    }
    public override void Exit()
    {
    }
    public override void Update(float deltaf)
    {
        //animator.CrossFade("character_jump_animation", 0, 0);
        if (player.inputAxis == 0)
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, 0, player.horizontalDeacceleration / player.jumpControlDeacceleration * deltaf);
        }
        else
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * player.maxHorizontalSpeed, player.horizontalAcceleration / player.jumpControlAcceleration * deltaf);
        }

        timer -= deltaf;
        if( timer < 0)
        {
            if (player.IsOnFloor())
            {
                if (player.velocity.X >= -0.1f && player.velocity.X <= 0.1f)
                {
                    player.velocity.X = 0;
                    player.ChangeState(new IdleState());
                }
                else
                {
                    player.ChangeState(new WalkState());
                }
            }
        }
        player.velocity.Y -= player.gravity * deltaf;
    }

    public override void JumpInput()
    {

    }

}
