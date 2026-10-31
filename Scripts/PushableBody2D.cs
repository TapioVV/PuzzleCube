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
            _Velocity += GravityDirection * GetGravity().Length();
        }
        _Velocity += PushVelocity;
        Velocity = _Velocity;

        MoveAndSlide();
        _Velocity = Vector2.Zero;
        PushVelocity = Vector2.Zero;
    }
    public void Push(float dx)
    {
        if (Mathf.IsZeroApprox(dx)) return;
        if(MainBody != null)
        {
            PushableBody2D body = MainBody as PushableBody2D;
            body.Push(dx);
            return;
        }

        pushRayCast2D.TargetPosition = new Vector2(Mathf.Sign(dx) * pushRayCast2DLength, 0);
        pushRayCast2D.ForceRaycastUpdate();
        if (pushRayCast2D.GetCollider() is PushableBody2D nextPushableBody && nextPushableBody.IsOnFloor())
        {
            nextPushableBody.Push(dx);
        }
        MoveAndCollide(Vector2.Right.Rotated(GlobalRotation) * dx);
    }
}



