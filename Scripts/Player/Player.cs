using Godot;
using System;
public partial class Player : ExtendedCharacterBody2D
{
    [Export] Label stateLabel;

    public State CurrentState;


    [ExportCategory("Movement variables")]
    [Export] public float horizontalAcceleration;
    [Export] public float horizontalDeacceleration;
    [Export] public float jumpControlAcceleration;
    [Export] public float jumpControlDeacceleration;

    [Export] public float timeToJumpPeak;
    [Export] public float jumpHeight;
    [Export] public float jumpDistance;
    [Export(PropertyHint.Range, "0, 1")] public float PushingSpeedMultiplier = 0.5f; 

    public float smallJump;
    [Export] float maxVerticalSpeed;
    bool jumpPressed = false;

    public float maxHorizontalSpeed;
    public float gravity;
    public float JumpForce;

    public Vector2 velocity;

    public float inputAxis;

    public float jumpBufferTimer = 0;
    public float passablePlatformTimer = 0f;
    [Export(PropertyHint.None, "suffix:seconds")]
    float jumpBufferTime;
    public float passablePlatformTime = 0.1f;

    public RayCast2D pushRayCast2D;
    public float pushRayCast2DLength;
    public Area2D BoxCheckArea;
    [ExportCategory("Visuals")]
    [Export] public AnimatedSprite2D CharacterSprite;
    

    public override void _Ready()
    {
        pushRayCast2D = GetNode<RayCast2D>("%PushRayCast");
        CharacterSprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        BoxCheckArea = GetNode<Area2D>("%BoxCheckArea");
        pushRayCast2DLength = pushRayCast2D.TargetPosition.X;
  
        CurrentState = new IdleState();
        CurrentState.player = this;

        CalculateJumpForce();
    }
    void CalculateJumpForce()
    {
        gravity = (2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        gravity = -gravity;
        JumpForce = gravity * timeToJumpPeak;
        maxHorizontalSpeed = jumpDistance / (2 * timeToJumpPeak);
    }
    public override void _Process(double delta)
    {
        if(stateLabel != null)
        {
            stateLabel.Text = $"Current State: {CurrentState.GetType().Name}";
        }

        // Jump input buffer
        inputAxis = Input.GetAxis("move_left", "move_right");
        if (Input.IsActionJustPressed("jump"))
        {
           jumpPressed = true;
           jumpBufferTimer = jumpBufferTime;
        }
        jumpBufferTimer -= (float)delta;
        passablePlatformTimer -= (float)delta;
        if(passablePlatformTimer > 0)
        {
            SetCollisionMaskValue(4, false);
        }
        else
        {
            SetCollisionMaskValue(4, true);
        }



        velocity.Y = Mathf.Clamp(velocity.Y, -maxVerticalSpeed, 10000000);
    }
    public bool IsBoxOnHead()
    {
        if(BoxCheckArea.GetOverlappingBodies().Count != 0)
        {
            return true;
        }
        return false;
    }
    public override void _PhysicsProcess(double delta)
    {
        CurrentState.Update((float)delta);
        Vector2 localVelocityFromState = velocity;
        Velocity = ToGlobalVelocity(localVelocityFromState);
        MoveAndSlide();
        CurrentState.AfterMoveAndSlideUpdate((float)delta);
    }
    private Vector2 ToGlobalVelocity(Vector2 localVelocity)
    {
        Vector2 globalUp = -GravityDirection;

        Vector2 globalRight = new Vector2(-globalUp.Y, globalUp.X);

        return (globalRight * localVelocity.X) + (globalUp * localVelocity.Y);
    }

    // Calls the state functions on change
    public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.player = this;
        CurrentState.Enter();
    }
}
