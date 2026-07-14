using Godot;
using System;

[GlobalClass]
public partial class PushableBody2D : CharacterBody2D
{
    public Vector2 _Velocity;
    public Vector2 PushVelocity = Vector2.Zero;
    Vector2 gravity;  
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


        // Had to do this rounding because the physics were so imprecise that you couldn't walk over the same size boxes
        GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Round(GlobalPosition.Y));
    }
    
}
