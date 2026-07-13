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
        player.velocity.Y = 0;
        //Deacceleration
        if (player.inputAxis == 0)
        {
            player.velocity.X = Mathf.MoveToward(player.velocity.X, 0, player.horizontalDeacceleration * deltaf);
        }
        //Acceleration
        else
        {
            //If turning around have increased acceleration
            if (player.inputAxis > 0 && player.velocity.X < 0) 
            { 
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * player.maxHorizontalSpeed, player.horizontalAcceleration * 4f * deltaf);
            }
            else if (player.inputAxis < 0 && player.velocity.X > 0) 
            {
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * player.maxHorizontalSpeed, player.horizontalAcceleration * 4f * deltaf);
            }
            else
            {
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * player.maxHorizontalSpeed, player.horizontalAcceleration * deltaf);
            }
        }


        //for(int i = 0; i < player.GetSlideCollisionCount(); i++)
        //{
        //    KinematicCollision2D collision = player.GetSlideCollision(i);
        //    var collider = collision.GetCollider();

        //    if(collider is PushableBody2D)
        //    {
        //        PushableBody2D pushableBody = (PushableBody2D)collider;
        //        Vector2 collisionNormal = collision.GetNormal();
        //        float verticalAligment = collisionNormal.Dot(Vector2.Up);
        //        GD.Print(verticalAligment);
        //        if(verticalAligment > 0)
        //        {
        //            GD.Print("box is below");
        //        }
        //        if(verticalAligment < 0)
        //        {
        //            GD.Print("box is above");
        //        }
        //        int pushDirection = (int)collision.GetNormal().Sign().X;
        //        pushDirection = -Mathf.Sign(pushDirection);
        //        float pushSpeed = player.velocity.X * 0.6f;
        //        pushableBody.PushVelocity.X = (pushDirection * Mathf.Abs(pushSpeed));
        //    }
        //}





        //Sprite flipping
        if (player.inputAxis > 0)
        {
            player.CharacterSprite.FlipH = false;
        }
        else if (player.inputAxis < 0)
        {
            player.CharacterSprite.FlipH = true;
        }


        //State changing
        if (player.jumpBufferTimer > 0)
        {
            player.ChangeState(new JumpState());
            return;
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
