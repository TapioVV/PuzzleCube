using Godot;
using System;

public partial class ExtendedCharacterBody2D : CharacterBody2D
{
    public Vector2 GravityDirection = Vector2.Down;
    public CubeFace CurrentFace;
    public void ChangeGravityDirection(Vector2 direction)
    {
        GravityDirection = direction.Normalized();
        UpDirection = -GravityDirection;
        switch (GravityDirection)
        {
            case (0, -1): // Up
                Rotation = Mathf.DegToRad(180);
                break;
            case (0, 1): // Down
                Rotation = 0;
                break;
            case (-1, 0): // Left
                Rotation = Mathf.DegToRad(90);
                break;
            case (1, 0): // Right
                Rotation = Mathf.DegToRad(-90);
                break;
        }
    }
}