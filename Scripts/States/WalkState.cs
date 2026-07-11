using Godot;
using System;

public partial class WalkState : State
{
    public override void Enter()
    {
    }
    public override void Exit()
    {
    }
    public override void Update(float deltaf)
    {
        //animator.CrossFade("character_run_animation", 0, 0);
        player.velocity.Y = 2;
        if (Input.IsActionJustPressed("jump"))
        {
            player.ChangeState(new JumpState());
            return;
        }


        if (player.inputAxis == 0)
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, 0, player.horizontalDeacceleration * deltaf);
        }
        else
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * player.maxHorizontalSpeed, player.horizontalAcceleration * deltaf);
        }

        if (player.inputAxis > 0)
        {
            player.CharacterSprite.FlipH = false;
        }
        else if (player.inputAxis < 0)
        {
            player.CharacterSprite.FlipH = true;
        }

        if (player.velocity.X >= -0.1f && player.velocity.X <= 0.1f)
        {
            player.velocity.X = 0;
            player.ChangeState(new IdleState());
            return;
        }
        if (!player.IsOnFloor())
        {
            player.ChangeState(new FallState());
            return;
        }
    }
    public override void JumpInput()
    {
        //player.OnPlayerNormalJump.Invoke();

        //player.Jump(1f);
        //player.ChangeState(player.jumpState);

    }
}
