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
    public override void AfterMoveAndSlideUpdate(float deltaf)
    {
        if (player.pushRayCast2D.IsColliding())
        {
            var collider = player.pushRayCast2D.GetCollider();
            var collision = player.pushRayCast2D.GetCollisionNormal();
            if (collider is PushableBody2D)
            {
                PushableBody2D pushableBody = (PushableBody2D)collider;
                Vector2 collisionNormal = player.pushRayCast2D.GetCollisionNormal();
                if (Mathf.Abs(collisionNormal.X) > 0.9f)
                {
                    if (collisionNormal.X > 0f)
                    {
                        // Normal points Right -> Player is on the Left, pushing Right
                        GD.Print("Player is touching from the LEFT");
                    }
                    else
                    {
                        // Normal points Left -> Player is on the Right, pushing Left
                        GD.Print("Player is touching from the RIGHT");
                    }
                    if (pushableBody.IsOnFloor())
                    {
                        int pushDirection = (int)collisionNormal.Sign().X;
                        pushDirection = -Mathf.Sign(pushDirection);
                        float pushSpeed = player.velocity.X * 0.6f;
                        pushableBody.PushVelocity.X = (pushDirection * Mathf.Abs(pushSpeed));
                    }
                }
            }

        }
        //for (int i = 0; i < player.GetSlideCollisionCount(); i++)
        //{
        //    KinematicCollision2D collision = player.GetSlideCollision(i);
        //    var collider = collision.GetCollider();

        //           //}
    }
}
