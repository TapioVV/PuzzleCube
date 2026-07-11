using Godot;
using System;

public partial class CubeRotation : Node3D
{
	public override void _Ready()
	{
        startRotation = Rotation;
	}

    Vector3 startRotation;
    float rotationSpeed = 4f;
	public override void _Process(double delta)
	{
		Vector2 stickInput = Input.GetVector("camera_right", "camera_left", "camera_down", "camera_up");
        if (Input.IsActionJustPressed("camera_reset"))
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "rotation", startRotation, 0.1f);
        }
        if (stickInput.Length() > 0.1f)
        {
            float dt = (float)delta;

            Rotate(Vector3.Up, -stickInput.X * rotationSpeed * dt);

            // 3. Rotate around the local X-axis (Pitch / Up-Down)
            // Moving stick down (positive Y) tilts the object down around its local X
            Rotate(Vector3.Right, -stickInput.Y * rotationSpeed * dt);
            //RotateObjectLocal(Vector3.Right, -stickInput.Y * rotationSpeed * dt);
        }
    }
}
