using Godot;
using System;

public partial class WalkState : State
{
    public override void Enter()
    {
        player.CharacterSprite.Play("walk");
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
        if (player.IsBoxOnHead())
        {
            if(player.inputAxis != 0)
            {
                player.ChangeState(new BoxOnHeadWalkState());
            }
            else
            {
                player.ChangeState(new BoxOnHeadIdleState());
            }
            return;
        }
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
                    float pushSpeed = player.velocity.X * player.PushingSpeedMultiplier;
                    pushableBody.Push(pushDirection * Mathf.Abs(pushSpeed));
                }
            }

        }
    }
}
