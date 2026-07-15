using Godot;
using System;

[GlobalClass]
public partial class Rewindable : Node2D
{
	// Called when the node enters the scene tree for the first time.
	Node2D parent;
	public Vector2 PreviousPosition;
	public override void _Ready()
	{
		parent = (Node2D)GetParent();
	}
}
