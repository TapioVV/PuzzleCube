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
	public Node2D world;
}

public partial class CubeManager : Node
{
	const int CUBE_SIDE_WIDTH = 240;
	const int CUBE_SIDE_HEIGHT = 240;

    public enum Direction { Left, Right, Up, Down }



    [Export] SubViewportContainer[] cubeSideViewports;
	[Export] Node2D[] cubeSideWorlds;
	[Export] Player player;
	[Export] Player fakePlayer;
	[Export] Sprite2D otherSprite;
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
                player.Position = new Vector2(player.Position.X, 1);
                return;
            }

        }





        if (player.Position.X < 0)
        {
            //fakePlayer.Position = Vector2.Zero;
            //fakePlayer.Visible = false;
            //player.Reparent(cubeSideWorlds[0]);

            ChangePlayerCurrentCubeSide(currentSide.Left);
            player.Position = new Vector2(CUBE_SIDE_WIDTH - 1, player.Position.Y);
            movedScreen = false;
        }



        if(player.Position.X > CUBE_SIDE_WIDTH)
        {
            //fakePlayer.Position = Vector2.Zero;
            //fakePlayer.Visible = false;
            //player.Reparent(cubeSideWorlds[1]);
            ChangePlayerCurrentCubeSide(currentSide.Right);
            player.Position = new Vector2(1, player.Position.Y);
            movedScreen = true;
        }







		else
		{
			fakePlayer.Visible = false;
		}
	}
}
