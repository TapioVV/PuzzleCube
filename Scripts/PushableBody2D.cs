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

     

        MoveAndSlide();
        _Velocity = Vector2.Zero;
        PushVelocity = Vector2.Zero;
    }
    public void Push(float dx)
    {
        // PushVelocity.X = pusherVelocity; 
        // if(pusherVelocity < 0)
        // {
        //     pushRayCast2D.TargetPosition = new Vector2(-pushRayCast2DLength, 0);
        // }
        // if(pusherVelocity > 0)
        // {
        //     pushRayCast2D.TargetPosition = new Vector2(pushRayCast2DLength, 0);
        // }
        // pushRayCast2D.ForceRaycastUpdate();
        // if (pushRayCast2D.IsColliding())
        // {
        //     var collider = pushRayCast2D.GetCollider();
        //     if (collider is PushableBody2D)
        //     {
        //         PushableBody2D pushableBody = (PushableBody2D)collider;
        //         if (pushableBody.IsOnFloor())
        //         {
        //             pushableBody.Push(pusherVelocity);
        //         }
        //     }
        // }
        if (Mathf.IsZeroApprox(dx)) return;

        // 1. Whatever is in front of us moves first, so chains move front-to-back
        pushRayCast2D.TargetPosition = new Vector2(Mathf.Sign(dx) * pushRayCast2DLength, 0);
        pushRayCast2D.ForceRaycastUpdate();
        if (pushRayCast2D.GetCollider() is PushableBody2D next && next.IsOnFloor())
            next.Push(dx);

        // 2. Then we move, this same tick
        MoveAndCollide(new Vector2(dx, 0));
    }
}



