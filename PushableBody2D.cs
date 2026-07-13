using Godot;
using System;

[GlobalClass]
public partial class PushableBody2D : CharacterBody2D
{
    public Vector2 _Velocity;
    public Vector2 PushVelocity;
    [Export] Area2D area2D;
    Vector2 gravity;  
    public override void _PhysicsProcess(double delta)
    {
        gravity = GetGravity();
        if (!IsOnFloor())
        {
            _Velocity.Y = gravity.Y;
        }

        _Velocity += PushVelocity;
        //var overLappingBodies = area2D.GetOverlappingBodies();
        //for (int i = 0; i < overLappingBodies.Count; i++)
        //{
        //    var body = overLappingBodies[i];

        //    if (body is Player)
        //    {
        //        //CharacterBody2D pushableBody = (CharacterBody2D)collider;
        //        Vector2 collisionNormal = body.GlobalPosition - GlobalPosition;
        //        collisionNormal = collisionNormal.Normalized();
        //        float verticalAligment = collisionNormal.Dot(Vector2.Up);
        //        //GD.Print(verticalAligment);
        //        if (verticalAligment > 0.7f)
        //        {
        //            GD.Print("player is above");
        //        }
        //        if (verticalAligment < -0.7f)
        //        {

        //            GD.Print("player is below");
        //            _Velocity.Y = -1; 
        //        }
        //    }
        //}

        Velocity = _Velocity;
        MoveAndSlide();
        _Velocity = Vector2.Zero;
        PushVelocity = Vector2.Zero;
    }
    
}
