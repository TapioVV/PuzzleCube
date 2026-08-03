using Godot;
using System;

public partial class BoxOnHeadWalkState : State
{
    public override void Enter()
    {
        player.CharacterSprite.Play("box_on_head_walk");
    }
    public override void Exit()
    {
    }
    public override void Update(float deltaf)
    {
        //animator.CrossFade("character_run_animation", 0, 0);
        player.velocity.Y = 0;
		float boxOnHeadWalkSpeed = player.maxHorizontalSpeed / 2;

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
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * boxOnHeadWalkSpeed, player.horizontalAcceleration * 4f * deltaf);
            }
            else if (player.inputAxis < 0 && player.velocity.X > 0)
            {
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * boxOnHeadWalkSpeed, player.horizontalAcceleration * 4f * deltaf);
            }
            else
            {
                player.velocity.X = Mathf.MoveToward(player.velocity.X, player.inputAxis * boxOnHeadWalkSpeed, player.horizontalAcceleration * deltaf);
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
        if (!player.IsBoxOnHead())
		{
			if (player.velocity.X >= -0.1f && player.velocity.X <= 0.1f)
			{
				player.velocity.X = 0;
				player.ChangeState(new IdleState());
				return;
			}
			else
			{
				player.ChangeState(new WalkState());
			}
		}
        if (player.velocity.X >= -0.1f && player.velocity.X <= 0.1f)
        {
            player.velocity.X = 0;
            player.ChangeState(new BoxOnHeadIdleState());
            return;
        }
        if (!player.IsOnFloor())
        {
            player.ChangeState(new FallState());
            return;
        }
    }
}
