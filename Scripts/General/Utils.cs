using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils 
{
	public static void AnimatedSprite2DCopyValuesFromAToB(AnimatedSprite2D a, AnimatedSprite2D b)
	{
		b.FlipH = a.FlipH;
		b.FlipV = a.FlipV;
		b.SpriteFrames = a.SpriteFrames;
		b.Animation = a.Animation;
		b.Frame = a.Frame;
		b.FrameProgress = a.FrameProgress;
		b.Modulate = a.Modulate;
		b.SelfModulate = a.SelfModulate;
	}
	public static void CollisionShape2DCopyValuesFromAToB(CollisionShape2D a, CollisionShape2D b)
	{
		b.Shape = (Shape2D)a.Shape.Duplicate();
	}
}
public static class NodeExtensions
{
    public static List<T> GetNodesInChildren<T>(this Node node) where T : Node
    {
        return node.FindChildren("*", "", true, false)
            .OfType<T>()
            .ToList();
    }
}