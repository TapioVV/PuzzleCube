using Godot;
using System;

[GlobalClass]
public partial class PushableBody2D : ExtendedCharacterBody2D
{
    public Vector2 _Velocity;
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
        Velocity = _Velocity;

        MoveAndSlide();
        _Velocity = Vector2.Zero;
    }

    public void Push(float dx)
    {
        if (Mathf.IsZeroApprox(dx)) return;

        if (MainBody is PushableBody2D main)
        {
            main.Push(dx);
            return;
        }

        // Look for boxes ahead on this face AND ahead of our clone on the other face.
        PushThroughRay(pushRayCast2D, pushRayCast2DLength, dx);
        if (SecondaryBody is PushableBody2D clone)
        {
            PushThroughRay(clone.pushRayCast2D, clone.pushRayCast2DLength, dx);
        }

        MoveAndCollide(Vector2.Right.Rotated(GlobalRotation) * dx);
    }

    void PushThroughRay(RayCast2D ray, float length, float dx)
    {
        ray.TargetPosition = new Vector2(Mathf.Sign(dx) * length, 0);
        ray.ForceRaycastUpdate();

        if (ray.GetCollider() is PushableBody2D hit)
        {
            // If we hit a clone, ask the real body about floor state.
            PushableBody2D real = hit.MainBody as PushableBody2D ?? hit;
            if (real != this && real.IsOnFloor())
                real.Push(dx);
        }
    }
}



