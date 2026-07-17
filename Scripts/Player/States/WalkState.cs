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
            player.ChangeState(new IdleState());
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
        player.pushRayCast2D.ForceRaycastUpdate();
        if (player.pushRayCast2D.IsColliding())
        {
            var collider = player.pushRayCast2D.GetCollider();
            if (collider is PushableBody2D)
            {
                PushableBody2D pushableBody = (PushableBody2D)collider;
                if (pushableBody.IsOnFloor())
                {
                    //int pushDirection = (int)player.pushRayCast2DLength.Sign();
                    int pushDirection = Mathf.Sign(player.pushRayCast2D.TargetPosition.X);
                    float pushSpeed = player.velocity.X * 0.5f;
                    pushableBody.Push(pushDirection * Mathf.Abs(pushSpeed));
                }
            }

        }
    }
}
