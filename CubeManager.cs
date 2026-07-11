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
	const int CUBE_SIDE_WIDTH = 240;
	const int CUBE_SIDE_LENGTH = 240;


    public enum Direction { Left, Right, Up, Down }


    [Export] Node3D cubeRotationPoint; 
	[Export] Node2D[] cubeSideWorlds;
	[Export] Player player;
	[Export] Player fakePlayer;
	CubeFace[] cubeFaces = new CubeFace[6];
    CubeFace currentFace;
    Quaternion targetQuaternion = Quaternion.Identity;
    public override void _Ready()
	{
        cubeFaces[0] = new CubeFace("Front", Vector3.Forward, Vector3.Up, cubeSideWorlds[0]);
        cubeFaces[1] = new CubeFace("Right", Vector3.Right, Vector3.Up, cubeSideWorlds[1]);
        cubeFaces[2] = new CubeFace("Back", Vector3.Back, Vector3.Up, cubeSideWorlds[2]);
        cubeFaces[3] = new CubeFace("Left", Vector3.Left, Vector3.Up, cubeSideWorlds[3]);
        cubeFaces[4] = new CubeFace("Top", Vector3.Up, Vector3.Back, cubeSideWorlds[4]);
        cubeFaces[5] = new CubeFace("Bottom", Vector3.Down, Vector3.Forward, cubeSideWorlds[5]);

        currentFace = cubeFaces[0];
    }
    public override void _Process(double delta)
	{
        //if (Input.IsActionPressed("down"))
        //{
        //    if (Input.IsActionJustPressed("jump"))
        //    {
        //        ChangePlayerCurrentCubeSide(currentFace.);
        //        //player.ChangeGravityDirection(Vector2.Right);

        //        CubeRotationTween(new Vector2(-90, 0));
        //        player.Position = new Vector2(player.Position.X, 1);
        //        return;
        //    }
        //}
        if (Input.IsActionJustPressed("gravity_right"))
        {
            player.ChangeGravityDirection(Vector2.Right);
        }
        if (Input.IsActionJustPressed("gravity_left"))
        {
            player.ChangeGravityDirection(Vector2.Left);
        }
        if (Input.IsActionJustPressed("gravity_up"))
        {
            player.ChangeGravityDirection(Vector2.Up);
        }
        if (Input.IsActionJustPressed("gravity_down"))
        {
            player.ChangeGravityDirection(Vector2.Down);
        }
        HandeFaceTransitions();
	}

    //void ChangePlayerCurrentCubeSide(CubeFace face)
    //{
    //    currentFace = face;

    //    player.Reparent(currentFace.World);
    //}
    private void TweenCubeRotation()
    {
        float rotationSpeed = 1f;
        Tween tween = CreateTween();
        tween.TweenProperty(cubeRotationPoint, "quaternion", targetQuaternion, rotationSpeed)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.InOut);
    }
    void HandeFaceTransitions()
    {
        Vector2 position = player.Position;
        Vector2 moveDirection = Vector2.Zero;
        Vector2 newPlayerPosition = position;
        int newPositionOffset = 1;
        if(position.X < 0)
        {
            moveDirection = Vector2.Left;
            newPlayerPosition = new Vector2(CUBE_SIDE_LENGTH - newPositionOffset, newPlayerPosition.Y);
        }
        if(position.X > CUBE_SIDE_LENGTH)
        {
            moveDirection = Vector2.Right;
            newPlayerPosition = new Vector2(newPositionOffset, newPlayerPosition.Y);
        }
        if(position.Y < 0)
        {
            moveDirection = Vector2.Up;
            newPlayerPosition = new Vector2(newPlayerPosition.X, CUBE_SIDE_LENGTH + newPlayerPosition.Y);
        }
        if(position.Y > CUBE_SIDE_LENGTH)
        {

            moveDirection = Vector2.Down;
            newPlayerPosition = new Vector2(newPlayerPosition.X, CUBE_SIDE_LENGTH);
        }
        if(moveDirection != Vector2.Zero)
        {
            TransitionToFace(moveDirection, newPlayerPosition);
        }
    }
    private void TransitionToFace(Vector2 screenMovement, Vector2 newPosition)
    {
        // 1. Calculate the 3D direction vector of the player's screen movement relative to the current face
        // 2. The target face's normal will be matching our local 3D movement path off the edge
        Vector3 _3dMoveDirection = Convert2DMovementTo3D(screenMovement, currentFace);
        CubeFace nextFace = FindFaceByNormal(_3dMoveDirection);

        if (nextFace == null) return;

        // 3. Compute the structural 3D rotation step required to center the new face
        Quaternion faceRotation = new Quaternion(currentFace.Normal, nextFace.Normal);

        // Accumulate rotation smoothly globally
        targetQuaternion = faceRotation * targetQuaternion;

        TweenCubeRotation();

        // 4. Transform gravity direction smoothly based on the surface shift
        UpdatePlayerGravity(nextFace);

        // 5. Update viewport parents
        currentFace = nextFace;
        player.Reparent(currentFace.World);
        player.Position = newPosition;
    }

    private Vector3 Convert2DMovementTo3D(Vector2 screenMovement, CubeFace face)
    {
        // Derive standard basis vectors for the local 2D screen coordinate planes inside 3D space
        Vector3 screenUp3D = face.UpDirection;
        Vector3 screenRight3D = face.Normal.Cross(screenUp3D).Normalized();

        // Combine inputs to scale our 3D vector accurately
        return (screenRight3D * screenMovement.X) + (screenUp3D * -screenMovement.Y);
    }

    private CubeFace FindFaceByNormal(Vector3 normal)
    {
        foreach (var face in cubeFaces)
        {
            // Allow minor floating point tolerances
            if (face.Normal.DistanceSquaredTo(normal.Normalized()) < 0.01f)
            {
                return face;
            }
        }
        return null;
    }

    private void UpdatePlayerGravity(CubeFace nextFace)
    {
        // Find the absolute difference in world orientation between the two viewports
        // project the world down vector into the new screen space layout
        Vector3 gravity3D = Vector3.Down; // Base gravity world context

        // Find out what direction local "Down" is in terms of the new viewport layout
        Vector3 localUp3D = nextFace.UpDirection;
        Vector3 localRight3D = nextFace.Normal.Cross(localUp3D).Normalized();

        // Project the world down vector to a flat 2D vector relative to the viewport surface maps
        float gravity2DX = gravity3D.Dot(localRight3D);
        float gravity2DY = -gravity3D.Dot(localUp3D);

        Vector2 calculatedGravity2D = new Vector2(gravity2DX, gravity2DY).Normalized();

        player.ChangeGravityDirection(calculatedGravity2D);
    }
}
