using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;


public class CubeFace
{
    public String Name;
    public Vector3 UpDirection;
    public Vector3 Normal;
	public Node2D World;
    public CubeFace(string name,  Vector3 normal, Vector3 upDirection, Node2D world)
    {
        Name = name;
        Normal = normal;
        UpDirection = upDirection;
        World = world;
    }
}



public partial class CubeManager : Node
{
	const int CUBE_SIDE_LENGTH = 240;

    [Export] Label currentFaceText;
    [Export] Node3D cubeRotationPoint; 
    Vector3 startRotation;
	[Export] SubViewportContainer[] cubeSideViewports;
	[Export] Player player;
	[Export] ExtendedCharacterBody2D fakePlayer;
    [Export] AnimatedSprite2D fakePlayerSprite;
	CubeFace[] cubeFaces = new CubeFace[6];
    List<ExtendedCharacterBody2D> dynamicBodies;
    Quaternion targetQuaternion = Quaternion.Identity;

    public override void _Ready()
	{
        dynamicBodies = new List<ExtendedCharacterBody2D>();

        startRotation = cubeRotationPoint.Rotation;
        foreach(Node node in GetTree().GetNodesInGroup("Dynamic"))
        {
            if(node is ExtendedCharacterBody2D body)
            {
                dynamicBodies.Add(body);
            }
        }

        InitializeFaces();

        // player.CurrentFace = cubeFaces[0];
        foreach(ExtendedCharacterBody2D body in dynamicBodies)
        {
            body.CurrentFace = cubeFaces[0];
        }
    }

    float rotationSpeed = 4f;

    public void InitializeFaces()
    {
        cubeFaces[0] = new CubeFace("Front", Vector3.Forward, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[0]));
        cubeFaces[1] = new CubeFace("Right", Vector3.Right, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[1]));
        cubeFaces[2] = new CubeFace("Back", Vector3.Back, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[2]));
        cubeFaces[3] = new CubeFace("Left", Vector3.Left, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[3]));
        cubeFaces[4] = new CubeFace("Top", Vector3.Up, Vector3.Back, getCubeSubViewportContainerChildWorld(cubeSideViewports[4]));
        cubeFaces[5] = new CubeFace("Bottom", Vector3.Down, Vector3.Forward, getCubeSubViewportContainerChildWorld(cubeSideViewports[5]));
    }
    Node2D getCubeSubViewportContainerChildWorld(Node subViewportContaienr)
    {
        Node subViewport = subViewportContaienr.GetChild(0);
        Node2D world = (Node2D)subViewport.GetChild(0);
        return world;
    }
    public override void _Process(double delta)
	{
        fakePlayerSprite.FlipH = player.CharacterSprite.FlipH;
        fakePlayerSprite.FlipV = player.CharacterSprite.FlipV;
        fakePlayerSprite.SpriteFrames = player.CharacterSprite.SpriteFrames;
        fakePlayerSprite.Animation = player.CharacterSprite.Animation;
        fakePlayerSprite.Animation = player.CharacterSprite.Animation;
        fakePlayerSprite.Frame = player.CharacterSprite.Frame;
        fakePlayerSprite.FrameProgress = player.CharacterSprite.FrameProgress;




        currentFaceText.Text = player.CurrentFace.Name;
        foreach(ExtendedCharacterBody2D body in dynamicBodies)
        {
            CheckForFaceTransition(body);
        }
        Rect2 spriteBounds = GetSpriteLocalBounds(player.CharacterSprite);
        Vector2 moveDirection = Vector2.Zero;
        fakePlayer.CurrentFace = player.CurrentFace;
        float fakePositionX = 0;
        float fakePositionY = 0;

        fakePlayer.GravityDirection = player.GravityDirection;
        if(spriteBounds.Position.X < 0)
        {
            // GD.Print("sprite is on the left of the screen");
            moveDirection = Vector2.Left;

            fakePositionX = CUBE_SIDE_LENGTH + player.Position.X;
            fakePositionY = player.Position.Y;
        }
        else if(spriteBounds.End.X > CUBE_SIDE_LENGTH)
        {
            moveDirection = Vector2.Right;

            fakePositionX = player.Position.X - CUBE_SIDE_LENGTH;
            // fakePositionX = CUBE_SIDE_LENGTH - player.Position.X;
            fakePositionY = player.Position.Y;
        }
        else if(spriteBounds.Position.Y < 0)
        {
            moveDirection = Vector2.Up;

            fakePositionX = player.Position.X;
            fakePositionY = CUBE_SIDE_LENGTH + player.Position.Y;
        }
        else if(spriteBounds.End.Y > CUBE_SIDE_LENGTH)
        {
            moveDirection = Vector2.Down;

            fakePositionX = player.Position.X;
            fakePositionY = player.Position.Y - CUBE_SIDE_LENGTH;
        }
        if(moveDirection != Vector2.Zero)
        {

            // This works but i dont understand it because i made it with ai
            // I need to figure this out
            // Tomorrow figure out notes in vscode
            Vector3 _3dMoveDirection = Convert2DMovementTo3D(moveDirection, player.CurrentFace);

            CubeFace nextFace = FindFaceByNormal(_3dMoveDirection);
            if (nextFace == null) return;

            Vector3 exitWorldPos = Convert2DPositionTo3D(player.Position, player.CurrentFace);

            // Applying the things
            UpdateBodyGravity(fakePlayer, nextFace);

            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(player.Position.X, 0, CUBE_SIDE_LENGTH),
                Mathf.Clamp(player.Position.Y, 0, CUBE_SIDE_LENGTH)
            );

            // 2. Get the 3D position of the seam
            Vector3 seamWorldPos = Convert2DPositionTo3D(clampedPos, player.CurrentFace);

            // 3. Calculate how far past the edge the player has moved
            float overflow = 0f;
            if (spriteBounds.Position.X < 0)
                overflow = -player.Position.X;
            else if (spriteBounds.End.X > CUBE_SIDE_LENGTH)
                overflow = player.Position.X - CUBE_SIDE_LENGTH;
            else if (spriteBounds.Position.Y < 0)
                overflow = -player.Position.Y;
            else if (spriteBounds.End.Y > CUBE_SIDE_LENGTH)
                overflow = player.Position.Y - CUBE_SIDE_LENGTH;

            // 4. Fold the overflow onto the adjacent face's surface
            float normalizedOverflow = overflow / CUBE_SIDE_LENGTH;
            Vector3 foldedWorldPos = seamWorldPos - (player.CurrentFace.Normal * normalizedOverflow);

            // 5. Project the folded 3D position into the next face's 2D space
            Vector3 screenUp3D = nextFace.UpDirection;
            Vector3 screenRight3D = nextFace.Normal.Cross(screenUp3D).Normalized();

            float pctX = foldedWorldPos.Dot(screenRight3D);
            float pctY = -foldedWorldPos.Dot(screenUp3D);

            Vector2 newPlayerPosition = new Vector2(
                (pctX + 0.5f) * CUBE_SIDE_LENGTH,
                (pctY + 0.5f) * CUBE_SIDE_LENGTH
            );

            fakePlayer.CurrentFace = nextFace;
            fakePlayer.Reparent(fakePlayer.CurrentFace.World);
            fakePlayer.Position = newPlayerPosition;
        }
	}





    public Rect2 GetSpriteLocalBounds(AnimatedSprite2D sprite)
    {
if (sprite == null || sprite.SpriteFrames == null)
        return new Rect2();

    string anim = sprite.Animation;
    int frame = sprite.Frame;
    Texture2D currentFrameTexture = sprite.SpriteFrames.GetFrameTexture(anim, frame);

    if (currentFrameTexture == null)
        return new Rect2();

    // 1. Get real texture size scaled by node's global scale
    Vector2 globalScale = sprite.GlobalScale;
    Vector2 frameSize = currentFrameTexture.GetSize() * globalScale;
    Vector2 offset = sprite.Offset * globalScale;

    // 2. Use GlobalPosition (world position inside SubViewport) instead of local Position
    Vector2 topLeft;
    if (sprite.Centered)
    {
        topLeft = sprite.GlobalPosition + offset - (frameSize / 2f);
    }
    else
    {
        topLeft = sprite.GlobalPosition + offset;
    }

    return new Rect2(topLeft, frameSize);

    }
    private void TweenCubeRotation(Quaternion target)
    {
        float rotationSpeed = 1f;
        Tween tween = CreateTween();
        tween.TweenProperty(cubeRotationPoint, "quaternion", target, rotationSpeed)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.InOut);
    }
    Quaternion GetQuaternionThatFacesCamera()
    {
        Basis cameraBasis = Basis.LookingAt(Vector3.Forward, Vector3.Up);
        Basis faceBasis = Basis.LookingAt(player.CurrentFace.Normal, player.CurrentFace.UpDirection);
        Basis targetCubeBasis = cameraBasis * faceBasis;
        Vector2 playerUp2D = -player.GravityDirection;

        // twist so the player always visually falls down the screen.
        float twistAngle = Vector2.Up.AngleTo(playerUp2D);
        return targetCubeBasis.Rotated(Vector3.Forward, -twistAngle).GetRotationQuaternion();
    }
    void CheckForFaceTransition(ExtendedCharacterBody2D body)
    {
        Vector2 position = body.Position;
        Vector2 screenMoveDirection = Vector2.Zero;

        // 1. Detect if the player crossed a boundary edge
        if (position.X < 0) screenMoveDirection = Vector2.Left;
        else if (position.X > CUBE_SIDE_LENGTH) screenMoveDirection = Vector2.Right;
        else if (position.Y < 0) screenMoveDirection = Vector2.Up;
        else if (position.Y > CUBE_SIDE_LENGTH) screenMoveDirection = Vector2.Down;

        if (screenMoveDirection != Vector2.Zero)
        {
            TransitionToFace(screenMoveDirection, body);
        }
    }
    private void TransitionToFace(Vector2 screenMoveDirection, ExtendedCharacterBody2D body)
    {
        //Get the face and position to move to 
        Vector3 _3dMoveDirection = Convert2DMovementTo3D(screenMoveDirection, body.CurrentFace);

        CubeFace nextFace = FindFaceByNormal(_3dMoveDirection);
        if (nextFace == null) return;

        Vector3 exitWorldPos = Convert2DPositionTo3D(body.Position, body.CurrentFace);

        // Applying the things
        UpdateBodyGravity(body, nextFace);

        MoveCharacterBodyToFace(body, exitWorldPos, nextFace);
        // if(body is player)
        // {
        //     TweenCubeRotation(GetQuaternionThatFacesCamera());
        // }
     }
    void MoveCharacterBodyToFace(ExtendedCharacterBody2D node, Vector3 positionOnFace, CubeFace face)
    {
        Vector2 newPlayerPosition = Convert3DPositionTo2D(positionOnFace, face);

        // // Nudge the player slightly onto the new face so they don't instantly re-trigger a transition
        // int edgeOffset = 1;
        // if (newPlayerPosition.X <= 0) newPlayerPosition.X = edgeOffset;
        // if (newPlayerPosition.X >= CUBE_SIDE_LENGTH) newPlayerPosition.X = CUBE_SIDE_LENGTH - edgeOffset;
        // if (newPlayerPosition.Y <= 0) newPlayerPosition.Y = edgeOffset;
        // if (newPlayerPosition.Y >= CUBE_SIDE_LENGTH) newPlayerPosition.Y = CUBE_SIDE_LENGTH - edgeOffset;

        // Update the things to the player node
        node.CurrentFace = face;
        node.Reparent(node.CurrentFace.World);
        node.Position = newPlayerPosition;
    }

    private Vector3 Convert2DPositionTo3D(Vector2 pos2D, CubeFace face)
    {
        // Normalize 2D position to range [-0.5, 0.5]
        float pctX = (pos2D.X / CUBE_SIDE_LENGTH) - 0.5f;
        float pctY = (pos2D.Y / CUBE_SIDE_LENGTH) - 0.5f;

        // Face surface sits at exactly 0.5 units along its normal vector
        Vector3 faceCenter = face.Normal * 0.5f;

        // 2D Y goes downwards on screen, so invert it relative to 3D Up
        return faceCenter + Convert2DMovementTo3D(new Vector2(pctX, pctY), face);

    }

    // Helper: Map a 3D boundary coordinate back down onto the new face's local 2D screen plane
    private Vector2 Convert3DPositionTo2D(Vector3 pos3D, CubeFace face)
    {
        Vector3 screenUp3D = face.UpDirection;
        Vector3 screenRight3D = face.Normal.Cross(screenUp3D).Normalized();
         // Project the 3D point onto our face basis vectors
        float pctX = pos3D.Dot(screenRight3D);
        float pctY = -pos3D.Dot(screenUp3D); // Invert back to screen space down

        // Convert back from [-0.5, 0.5] to [0, CUBE_SIDE_WIDTH/LENGTH]
        float x2D = (pctX + 0.5f) * CUBE_SIDE_LENGTH;
        float y2D = (pctY + 0.5f) * CUBE_SIDE_LENGTH;

        return new Vector2(x2D, y2D);
    }

    private Vector3 Convert2DMovementTo3D(Vector2 screenMoveDirection, CubeFace face)
    {
        // Derive standard basis vectors for the local 2D screen coordinate planes inside 3D space
        Vector3 screenUp3D = face.UpDirection;
        Vector3 screenRight3D = face.Normal.Cross(screenUp3D).Normalized();

        // Combine inputs to scale our 3D vector accurately
        return (screenRight3D * screenMoveDirection.X) + (screenUp3D * -screenMoveDirection.Y);
    }

    private CubeFace FindFaceByNormal(Vector3 normal)
    {
        foreach (var face in cubeFaces)
        {
            if (face.Normal.DistanceSquaredTo(normal.Normalized()) < 0.01f)
            {
                return face;
            }
        }
        return null;
    }

    private void UpdateBodyGravity(ExtendedCharacterBody2D body, CubeFace nextFace)
    {
        // Changes the players gravity to the correct one when moving between faces.
        Vector3 gravity3D = Convert2DMovementTo3D(body.GravityDirection, body.CurrentFace);
        Quaternion faceRotation = new Quaternion(body.CurrentFace.Normal, nextFace.Normal);

        Vector3 currentGravity3D = faceRotation * gravity3D;

        Vector3 nextRight3D = nextFace.Normal.Cross(nextFace.UpDirection).Normalized();

        float nextGravity3DX = currentGravity3D.Dot(nextRight3D);
        float nextGravity3DY = -currentGravity3D.Dot(nextFace.UpDirection);

        Vector2 calculatedGravity2D = new Vector2(nextGravity3DX, nextGravity3DY);


        body.ChangeGravityDirection(calculatedGravity2D.Normalized());
    }
}
