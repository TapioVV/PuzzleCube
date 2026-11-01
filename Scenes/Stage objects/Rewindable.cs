using Godot;
using System;

public struct RewindData
{
	public Vector2 Position;
	public Vector2 GravityDirection;
	public float Rotation;
	public Node2D Parent;
	public CubeFace CurrentFace;
}
