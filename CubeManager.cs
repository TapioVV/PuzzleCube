using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;


class CubeSide
{
    public CubeSide Left;
    public CubeSide Right;
    public CubeSide Top;
    public CubeSide Bottom;
    public Vector3 UpDirection;
    public Vector3 Normal;
	public Node2D world;
    public CubeSide( Vector3 upDirection, Vector3 normal, Node2D world)
    {
        UpDirection = upDirection;
        Normal = normal;
        this.world = world;
    }
}

public partial class CubeManager : Node
{
	const int CUBE_SIDE_WIDTH = 240;
	const int CUBE_SIDE_HEIGHT = 240;


    public enum Direction { Left, Right, Up, Down }


    [Export] Node3D cubeRotationPoint; 
	[Export] Node2D[] cubeSideWorlds;
	[Export] Player player;
	[Export] Player fakePlayer;
	CubeSide[] cubeSides = new CubeSide[6];
    CubeSide currentSide;
    public override void _Ready()
	{
        cubeSides[0] = new CubeSide();
        cubeSides[1] = new CubeSide();
        cubeSides[2] = new CubeSide();
        cubeSides[3] = new CubeSide();
        cubeSides[4] = new CubeSide();
        cubeSides[5] = new CubeSide();
        for (int i = 0; i < cubeSideWorlds.Length; i++)
        {
            CubeSide side = cubeSides[i];
            side.world = cubeSideWorlds[i];
        }

        cubeSides[0].Left = cubeSides[4 - 1]; // Left
        cubeSides[0].Right = cubeSides[2 - 1]; // Right
        cubeSides[0].Top = cubeSides[5 - 1]; // Top
        cubeSides[0].Bottom = cubeSides[6 - 1]; // Bottom

        // Right (2)
        cubeSides[1].Left = cubeSides[1 - 1]; // Front
        cubeSides[1].Right = cubeSides[3 - 1]; // Back
        cubeSides[1].Top = cubeSides[5 - 1]; // Top
        cubeSides[1].Bottom = cubeSides[6 - 1]; // Bottom

        // Back (3)
        cubeSides[2].Left = cubeSides[2- 1]; // Right
        cubeSides[2].Right = cubeSides[4 - 1]; // Left
        cubeSides[2].Top = cubeSides[5 - 1]; // Top
        cubeSides[2].Bottom = cubeSides[6 - 1]; // Bottom

        // Left (4)
        cubeSides[3].Left = cubeSides[3 - 1]; // Back
        cubeSides[3].Right = cubeSides[1 - 1]; // Front
        cubeSides[3].Top = cubeSides[5 - 1]; // Top
        cubeSides[3].Bottom = cubeSides[6 - 1]; // Bottom

        // Bottom (5) - Assuming Top of Bottom points to Front
        cubeSides[4].Left = cubeSides[4 - 1];
        cubeSides[4].Right = cubeSides[2 - 1];
        cubeSides[4].Top = cubeSides[3 - 1];
        cubeSides[4].Bottom = cubeSides[1 - 1];

        // Top (6) - Assuming Bottom of Top points to Front
        cubeSides[5].Left = cubeSides[4 - 1];
        cubeSides[5].Right = cubeSides[2 - 1];
        cubeSides[5].Top = cubeSides[1 - 1];
        cubeSides[5].Bottom = cubeSides[3 - 1];


        currentSide = cubeSides[0];
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	bool movedScreen = false;
    void ChangePlayerCurrentCubeSide(CubeSide side)
    {
        currentSide = side;

        player.Reparent(currentSide.world);
    }
    Vector2 targetRotation = Vector2.Zero;
    //public void CubeRotationTween(Vector2 addedRotation)
    //{
    //   float rotationSpeed = 1f; //Seconds
        
    //   targetRotation += new Vector2(Mathf.DegToRad(addedRotation.X), Mathf.DegToRad(addedRotation.Y));
    //   Tween tween = CreateTween().SetParallel();
    //   tween.TweenProperty(cubeRotationPoint, "rotation:x", targetRotation.X, rotationSpeed).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut); 
    //   tween.TweenProperty(cubeRotationPoint, "rotation:y", targetRotation.Y, rotationSpeed).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut); 
    //}
    private Quaternion targetQuaternion = Quaternion.Identity;

    public void CubeRotationTween(Vector2 addedRotation)
    {
        float rotationSpeed = 1f;

        // 1. Create rotations for X and Y separately using angles
        Quaternion xRotation = new Quaternion(Vector3.Right, Mathf.DegToRad(addedRotation.X));
        Quaternion yRotation = new Quaternion(Vector3.Up, Mathf.DegToRad(addedRotation.Y));

        // 2. Multiply them into your global target (Order matters! Y * X keeps it intuitive)
        targetQuaternion = yRotation * xRotation * targetQuaternion;

        // 3. Tween the entire basis or quaternion property at once
        Tween tween = CreateTween();
        tween.TweenProperty(cubeRotationPoint, "quaternion", targetQuaternion, rotationSpeed)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);
    }
    public override void _Process(double delta)
	{

		Rect2 playerSpriteRect = player.CharacterSprite.GetGlobalTransform() * player.CharacterSprite.GetRect();
		//if(movedScreen == true)
		//{
  // //         fakePlayer.Reparent(cubeSideWorlds[0]);
		//	//fakePlayer.Position = Vector2.Zero;
		//	//fakePlayer.Visible = true;
		//	//fakePlayer.Position = new Vector2(player.Position.X + 240, player.Position.Y);
  //          return;
		//}
		if(playerSpriteRect.Position.X < 0 || playerSpriteRect.End.X > CUBE_SIDE_WIDTH)
		{
			//fakePlayer.Reparent(cubeSideWorlds[1]);
			//fakePlayer.Position = Vector2.Zero;
			//fakePlayer.Visible = true;
			//fakePlayer.Position = new Vector2(player.Position.X - 240, player.Position.Y);
		}
        if (Input.IsActionPressed("down"))
        {
            if (Input.IsActionJustPressed("jump"))
            {
                ChangePlayerCurrentCubeSide(currentSide.Bottom);
                //player.ChangeGravityDirection(Vector2.Right);

                CubeRotationTween(new Vector2(-90, 0));
                player.Position = new Vector2(player.Position.X, 1);
                return;
            }
        }
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




        if (player.Position.X < 0)
        {
            //fakePlayer.Position = Vector2.Zero;
            //fakePlayer.Visible = false;
            //player.Reparent(cubeSideWorlds[0]);

            ChangePlayerCurrentCubeSide(currentSide.Left);

            CubeRotationTween(new Vector2(0, 90));

            player.Position = new Vector2(CUBE_SIDE_WIDTH - 1, player.Position.Y);

            movedScreen = false;
        }



        if(player.Position.X > CUBE_SIDE_WIDTH)
        {
            //fakePlayer.Position = Vector2.Zero;
            //fakePlayer.Visible = false;
            //player.Reparent(cubeSideWorlds[1]);
            ChangePlayerCurrentCubeSide(currentSide.Right);


            //testRotationY -= Mathf.DegToRad(90);

            //Tween tween = CreateTween();
            //float targetRotationY = cubeRotationPoint.Rotation.Y + Mathf.DegToRad(-90);
            //float targetRotationY = testRotationY + Mathf.DegToRad(-90);

            //tween.TweenProperty(cubeRotationPoint, "rotation:y", testRotationY, rotationSpeed).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
            CubeRotationTween(new Vector2(0, -90));

            player.Position = new Vector2(1, player.Position.Y);
            movedScreen = true;
        }







		else
		{
			fakePlayer.Visible = false;
		}
	}
}
