using Godot;
using System;

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
}
