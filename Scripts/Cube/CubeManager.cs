using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;


class CubeFace
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
	[Export] SubViewportContainer[] cubeSideViewports;
	[Export] Player player;
	[Export] Player fakePlayer;
	CubeFace[] cubeFaces = new CubeFace[6];
    CubeFace currentFace;
    Quaternion targetQuaternion = Quaternion.Identity;
    public override void _Ready()
	{
        cubeFaces[0] = new CubeFace("Front", Vector3.Forward, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[0]));
        cubeFaces[1] = new CubeFace("Right", Vector3.Right, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[1]));
        cubeFaces[2] = new CubeFace("Back", Vector3.Back, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[2]));
        cubeFaces[3] = new CubeFace("Left", Vector3.Left, Vector3.Up, getCubeSubViewportContainerChildWorld(cubeSideViewports[3]));
        cubeFaces[4] = new CubeFace("Top", Vector3.Up, Vector3.Back, getCubeSubViewportContainerChildWorld(cubeSideViewports[4]));
        cubeFaces[5] = new CubeFace("Bottom", Vector3.Down, Vector3.Forward, getCubeSubViewportContainerChildWorld(cubeSideViewports[5]));

        currentFace = cubeFaces[0];
    }
    Node2D getCubeSubViewportContainerChildWorld(Node subViewportContaienr)
    {
        Node subViewport = subViewportContaienr.GetChild(0);
        Node2D world = (Node2D)subViewport.GetChild(0);
        return world;
    }
    public override void _Process(double delta)
	{
        currentFaceText.Text = currentFace.Name;
        HandleFaceTransitions();
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
        Basis faceBasis = Basis.LookingAt(currentFace.Normal, currentFace.UpDirection);
        Basis targetCubeBasis = cameraBasis * faceBasis;
        Vector2 playerUp2D = -player.GravityDirection;

        // twist so the player always visually falls down the screen.
        float twistAngle = Vector2.Up.AngleTo(playerUp2D);
        return targetCubeBasis.Rotated(Vector3.Forward, -twistAngle).GetRotationQuaternion();
    }
    void HandleFaceTransitions()
    {
        Vector2 position = player.Position;
        Vector2 screenMoveDirection = Vector2.Zero;

        // 1. Detect if the player crossed a boundary edge
        if (position.X < 0) screenMoveDirection = Vector2.Left;
        else if (position.X > CUBE_SIDE_LENGTH) screenMoveDirection = Vector2.Right;
        else if (position.Y < 0) screenMoveDirection = Vector2.Up;
        else if (position.Y > CUBE_SIDE_LENGTH) screenMoveDirection = Vector2.Down;

        if (screenMoveDirection != Vector2.Zero)
        {
            TransitionToFace(screenMoveDirection);
        }
    }
    private void TransitionToFace(Vector2 screenMoveDirection)
    {
        //Get the face and position to move to 
        Vector3 _3dMoveDirection = Convert2DMovementTo3D(screenMoveDirection, currentFace);
        CubeFace nextFace = FindFaceByNormal(_3dMoveDirection);
        if (nextFace == null) return;

        Vector3 exitWorldPos = Convert2DPositionTo3D(player.Position, currentFace);

        // Applying the things
        UpdatePlayerGravity(nextFace);

        Vector2 newPlayerPosition = Convert3DPositionTo2D(exitWorldPos, nextFace);
        MovePlayerToFace(exitWorldPos, nextFace);
        TweenCubeRotation(GetQuaternionThatFacesCamera());
     }
    void MovePlayerToFace(Vector3 facePosition, CubeFace face)
    {
        Vector2 newPlayerPosition = Convert3DPositionTo2D(facePosition, face);

        // Nudge the player slightly onto the new face so they don't instantly re-trigger a transition
        int edgeOffset = 1;
        if (newPlayerPosition.X <= 0) newPlayerPosition.X = edgeOffset;
        if (newPlayerPosition.X >= CUBE_SIDE_LENGTH) newPlayerPosition.X = CUBE_SIDE_LENGTH - edgeOffset;
        if (newPlayerPosition.Y <= 0) newPlayerPosition.Y = edgeOffset;
        if (newPlayerPosition.Y >= CUBE_SIDE_LENGTH) newPlayerPosition.Y = CUBE_SIDE_LENGTH - edgeOffset;

        //Update the things to the player node
        currentFace = face;
        player.Reparent(currentFace.World);
        player.Position = newPlayerPosition;
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

        private void UpdatePlayerGravity(CubeFace nextFace)
    {
        // Changes the players gravity to the correct one when moving between faces.

        Vector3 gravity3D = Convert2DMovementTo3D(player.GravityDirection, currentFace);
        Quaternion faceRotation = new Quaternion(currentFace.Normal, nextFace.Normal);

        Vector3 currentGravity3D = faceRotation * gravity3D;

        Vector3 nextRight3D = nextFace.Normal.Cross(nextFace.UpDirection).Normalized();

        float nextGravity3DX = currentGravity3D.Dot(nextRight3D);
        float nextGravity3DY = -currentGravity3D.Dot(nextFace.UpDirection);

        Vector2 calculatedGravity2D = new Vector2(nextGravity3DX, nextGravity3DY);


        player.ChangeGravityDirection(calculatedGravity2D.Normalized());
    }
}
