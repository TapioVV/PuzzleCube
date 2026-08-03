using Godot;
using System;

public partial class JumpState : State
{
    float timer = 0.1f;
    bool releasedJump = false;
    public override void Enter()
    {
        releasedJump = false;
        player.jumpBufferTimer = -1;
        timer = 0.1f;
        player.velocity.Y = 0;
        player.velocity.Y = -player.JumpForce;
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
        
        //Sprite flipping
        if (player.inputAxis > 0)
        {
            player.CharacterSprite.FlipH = false;
            player.pushRayCast2D.TargetPosition = new Vector2(player.pushRayCast2DLength, 0);
        }
        else if (player.inputAxis < 0)
        {
            player.CharacterSprite.FlipH = true;
            player.pushRayCast2D.TargetPosition = new Vector2(-player.pushRayCast2DLength, 0);
        }
        if (player.IsOnCeiling())
        {
            player.velocity.Y = 0;
        }

        player.velocity.Y += player.gravity * deltaf;
    }
}
