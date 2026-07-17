using Godot;
using System;

[GlobalClass]
public partial class PushableBody2D : ExtendedCharacterBody2D
{
    public Vector2 _Velocity;
    public Vector2 PushVelocity = Vector2.Zero;
    Vector2 gravity;
    public RayCast2D pushRayCast2D;
    public float pushRayCast2DLength;
    public override void _Ready()
    {
        pushRayCast2D = GetNode<RayCast2D>("%PushRayCast");
        pushRayCast2DLength = pushRayCast2D.TargetPosition.X;
    }
    public override void _PhysicsProcess(double delta)
    {
        // Extremely simple gravity and pushing

        gravity = GetGravity();
        if (!IsOnFloor())
        {
            _Velocity.Y = gravity.Y;
        }




        _Velocity += PushVelocity;
        Velocity = _Velocity;

        //if (pushRayCast2D.IsColliding())
        //{
        //    var collider = pushRayCast2D.GetCollider();
        //    var collision = pushRayCast2D.GetCollisionNormal();
        //    if (collider is PushableBody2D)
        //    {
        //        PushableBody2D pushableBody = (PushableBody2D)collider;
        //        Vector2 collisionNormal = pushRayCast2D.GetCollisionNormal();
        //        if (Mathf.Abs(collisionNormal.X) > 0.9f)
        //        {
        //            //if (collisionNormal.X > 0f)
        //            //{
        //            //    // Normal points Right -> Player is on the Left, pushing Right
        //            //    GD.Print("Player is touching from the LEFT");
        //            //}
        //            //else
        //            //{
        //            //    // Normal points Left -> Player is on the Right, pushing Left
        //            //    GD.Print("Player is touching from the RIGHT");
        //            //}
        //            if (pushableBody.IsOnFloor())
        //            {
        //                int pushDirection = (int)collisionNormal.Sign().X;
        //                pushDirection = -Mathf.Sign(pushDirection);
        //                float pushSpeed = Velocity.X;
        //                pushableBody.PushVelocity.X = (pushDirection * Mathf.Abs(pushSpeed));
        //            }
        //        }
        //    }
        //}
        //if (Mathf.Sign(_Velocity.X) > 0)
        //{
        //    pushRayCast2D.TargetPosition = new Vector2(pushRayCast2DLength, 0);
        //}
        //if (Mathf.Sign(_Velocity.X) < 0)
        //{
        //    pushRayCast2D.TargetPosition = new Vector2(-pushRayCast2DLength, 0);
        //}

        MoveAndSlide();
        //Velocity = new Vector2(Velocity.X * (float)delta, Velocity.Y * (float)delta);
        //MoveAndCollide(Velocity);

        // Had to do this rounding because the physics were so imprecise that you couldn't walk over the same size boxes

        _Velocity = Vector2.Zero;
        PushVelocity = Vector2.Zero;
        GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Round(GlobalPosition.Y));
        //GlobalPosition = new Vector2(Mathf.Round(GlobalPosition.X), Mathf.Round(GlobalPosition.Y));
    }
    public void Push(float pusherVelocity)
    {
        PushVelocity.X = pusherVelocity; 
        if(pusherVelocity < 0)
        {
            pushRayCast2D.TargetPosition = new Vector2(-pushRayCast2DLength, 0);
        }
        if(pusherVelocity > 0)
        {
            pushRayCast2D.TargetPosition = new Vector2(pushRayCast2DLength, 0);
        }
        pushRayCast2D.ForceRaycastUpdate();
        if (pushRayCast2D.IsColliding())
        {
            var collider = pushRayCast2D.GetCollider();
            if (collider is PushableBody2D)
            {
                PushableBody2D pushableBody = (PushableBody2D)collider;
                if (pushableBody.IsOnFloor())
                {
                    pushableBody.Push(pusherVelocity- 2);
                }
            }
        }
    }
}



