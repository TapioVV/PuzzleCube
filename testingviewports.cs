using Godot;
using System;

[Tool]
public partial class testingviewports : SubViewport
{
    [ExportToolButton("Reset viewports views")]
    public Callable ClickMeButton => Callable.From(ResetSubviewport);
    [Export] SubViewport secondViewport;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResetSubviewport();
	}
	public void ResetSubviewport()
	{
		secondViewport.World2D = this.World2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
