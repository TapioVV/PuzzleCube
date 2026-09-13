using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class ExtendedCharacterBody2D : CharacterBody2D
{
    public Vector2 GravityDirection = Vector2.Down;

    public CubeFace CurrentFace;

    public AnimatedSprite2D MainSprite;

    public AnimatedSprite2D FakeSprite;

    public ExtendedCharacterBody2D FakeBody;

    public override void _EnterTree()
    {
        MainSprite = GetChildren().OfType<AnimatedSprite2D>().FirstOrDefault();
        if(MainSprite == null)
        {
            GD.PushError("Couldn't find AnimatedSprite as a child of " + Name);
        }
    }
    public void MirrorSpriteToFakeSprite()
    {
        FakeBody.MainSprite.FlipH = MainSprite.FlipH;
        FakeBody.MainSprite.FlipV = MainSprite.FlipV;
        FakeBody.MainSprite.SpriteFrames = MainSprite.SpriteFrames;
        FakeBody.MainSprite.Animation = MainSprite.Animation;
        FakeBody.MainSprite.Animation = MainSprite.Animation;
        FakeBody.MainSprite.Frame = MainSprite.Frame;
        FakeBody.MainSprite.FrameProgress = MainSprite.FrameProgress;



        // FakeSprite.FlipH = MainSprite.FlipH;
        // FakeSprite.FlipV = MainSprite.FlipV;
        // FakeSprite.SpriteFrames = MainSprite.SpriteFrames;
        // FakeSprite.Animation = MainSprite.Animation;
        // FakeSprite.Animation = MainSprite.Animation;
        // FakeSprite.Frame = MainSprite.Frame;
        // FakeSprite.FrameProgress = MainSprite.FrameProgress;
    }


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
    public Rect2 GetSpriteLocalBounds()
    {
        if (MainSprite == null || MainSprite.SpriteFrames == null)
            return new Rect2();

        string anim = MainSprite.Animation;
        int frame = MainSprite.Frame;
        Texture2D currentFrameTexture = MainSprite.SpriteFrames.GetFrameTexture(anim, frame);

        if (currentFrameTexture == null)
            return new Rect2();

        Vector2 globalScale = MainSprite.GlobalScale;
        Vector2 frameSize = currentFrameTexture.GetSize() * globalScale;
        Vector2 offset = MainSprite.Offset * globalScale;

        Vector2 topLeft;
        if (MainSprite.Centered)
        {
            topLeft = MainSprite.GlobalPosition + offset - (frameSize / 2f);
        }
        else
        {
            topLeft = MainSprite.GlobalPosition + offset;
        }

        return new Rect2(topLeft, frameSize);
    }
}