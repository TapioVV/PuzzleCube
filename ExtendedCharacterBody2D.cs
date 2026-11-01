using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class ExtendedCharacterBody2D : CharacterBody2D
{
    public Vector2 GravityDirection = Vector2.Down;

    public CubeFace CurrentFace;
    public CubeFace PreviousFace;
    public ulong LastFaceChangeFrame;

    public List<CollisionShape2D> collisionShape2Ds = new List<CollisionShape2D>();

    public AnimatedSprite2D AnimatedSprite;

    public ExtendedCharacterBody2D SecondaryBody;
    public ExtendedCharacterBody2D MainBody;

    public override void _EnterTree()
    {
        AnimatedSprite = GetChildren().OfType<AnimatedSprite2D>().FirstOrDefault();

        // if(MainSprite == null)
        // {
        //     GD.PushError("Couldn't find AnimatedSprite as a child of " + Name);
        // }
    }
    public void MirrorSpriteToSecondaryBodySprite()
    {
        Utils.AnimatedSprite2DCopyValuesFromAToB(AnimatedSprite, SecondaryBody.AnimatedSprite);
        for(int i = 0; i < collisionShape2Ds.Count; i++)
        {
            // Utils.CollisionShape2DCopyValuesFromAToB(collisionShape2Ds[i], SecondaryBody.collisionShape2Ds[i]);
        }
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
        if (AnimatedSprite == null || AnimatedSprite.SpriteFrames == null)
            return new Rect2();

        string anim = AnimatedSprite.Animation;
        int frame = AnimatedSprite.Frame;
        Texture2D currentFrameTexture = AnimatedSprite.SpriteFrames.GetFrameTexture(anim, frame);

        if (currentFrameTexture == null)
            return new Rect2();

        Vector2 globalScale = AnimatedSprite.GlobalScale;
        Vector2 frameSize = currentFrameTexture.GetSize() * globalScale;
        Vector2 offset = AnimatedSprite.Offset * globalScale;

        Vector2 topLeft;
        if (AnimatedSprite.Centered)
        {
            topLeft = AnimatedSprite.GlobalPosition + offset - (frameSize / 2f);
        }
        else
        {
            topLeft = AnimatedSprite.GlobalPosition + offset;
        }

        return new Rect2(topLeft, frameSize);
    }
}