using Godot;
using System;

public partial class FpsCounter : Label
{
	public override void _Process(double delta)
	{
        int fps = (int)Engine.GetFramesPerSecond();

        // Update the Label's text
        Text = $"FPS: {fps}";
    }
}
