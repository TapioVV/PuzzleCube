using Godot;
using System;
public partial class Player : CharacterBody2D
{
    //[Export(PropertyHint.Layers2DPhysics)]
    //public uint GroundLayerMask;

    [Export] Label stateLabel;

    public State CurrentState;

    //public IdleState idleState;
    //public WalkState walkState;
    //public JumpState jumpState;
    //public FallState fallState;
    //[Export] public DeadState deadState;



    [ExportCategory("Movement variables")]
    [Export] public float horizontalAcceleration;
    [Export] public float horizontalDeacceleration;
    [Export] public float jumpControlAcceleration;
    [Export] public float jumpControlDeacceleration;

    [Export] public float timeToJumpPeak;
    [Export] public float jumpHeight;
    [Export] public float jumpDistance;

    public float smallJump;
    [Export] float maxVerticalSpeed;
    bool jumpPressed = false;

    public float maxHorizontalSpeed;
    public float gravity;
    public float jumpSpeed;

    public Vector2 velocity;

    public float inputAxis;

    public float jumpBufferTimer = 0;
    [Export(PropertyHint.None, "suffix:seconds")]
    float jumpBufferTime;

    //[HideInInspector] public Animator animator;
    //SpriteRenderer spriteRenderer;
    [ExportCategory("Visuals")]
    [Export] public Sprite2D CharacterSprite;

    public override void _Ready()
    {
        //idleState = new IdleState();
        //fallState = new FallState();
        //walkState = new WalkState();
        //jumpState = new JumpState();
        //AddChild(idleState);
        //AddChild(fallState);
        //AddChild(walkState);
        //AddChild(jumpState);

        CurrentState = new IdleState();
        CurrentState.player = this;

        gravity = (2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        gravity = -gravity;
        jumpSpeed = gravity * timeToJumpPeak;
        maxHorizontalSpeed = jumpDistance / (2 * timeToJumpPeak);
    }
    //public void MoveInput(InputAction.CallbackContext context)
    //{
    //    inputAxis = (int)context.ReadValue<float>();
    //}
    //public void StoppedMoveInput(InputAction.CallbackContext context)
    //{
    //    inputAxis = 0;
    //}


    //public void JumpBuffer()
    //{
    //    jumpBufferTimer = Mathf.MoveToward(jumpBufferTimer, -1, Time.deltaTime);
    //    jumpBufferTimer -= deltaf;
    //    if (jumpBufferTimer > 0 && jumpPressed == true)
    //    {
    //        CurrentState.JumpInput();
    //    }
    //}
    //   //private void OnTriggerEnter2D(Collider2D collision)
    //   //{
    //   //    if (collision.gameObject.tag == "Death")
    //   //    {
    //   //        CurrentState = deadState;
    //   //        OnPlayerDeath.Invoke();
    //   //    }
    //   //    if (collision.gameObject.tag == "Win")
    //   //    {
    //   //        CurrentState = deadState;
    //   //        OnPlayerWin.Invoke();
    //   //    }
    //   //}
    public override void _Process(double delta)
    {
        
        if(stateLabel != null)
        {
            stateLabel.Text = $"Current State: {CurrentState.GetType().Name}";
        }
        //Velocity = velocity;
        inputAxis = Input.GetAxis("move_left", "move_right");
        if (Input.IsActionJustPressed("jump"))
        {
           jumpPressed = true;
           jumpBufferTimer = jumpBufferTime;
        }
        jumpBufferTimer -= (float)delta;

        //JumpBuffer();
        velocity.Y = Mathf.Clamp(velocity.Y, -maxVerticalSpeed, 10000000);
    }
    Vector2 gravityDirection = Vector2.Down;
    public void ChangeGravityDirection(Vector2 direction)
    {

        gravityDirection = direction.Normalized();
        switch (gravityDirection)
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
    public override void _PhysicsProcess(double delta)
    {
        UpDirection = -gravityDirection;
        CurrentState.Update((float)delta);
        //Vector2 localRight = new Vector2(-gravityDirection.Y, gravityDirection.X);
        //Vector2 localUp = -gravityDirection;
        //Velocity = (localRight * velocity.X) + (localUp * velocity.Y);
        //Velocity = velocity;
        Vector2 localVelocityFromState = velocity;
        Velocity = ToGlobalVelocity(localVelocityFromState);
        MoveAndSlide();
    }
    private Vector2 ToGlobalVelocity(Vector2 localVelocity)
    {
        // Local Up is directly against gravity
        Vector2 globalUp = -gravityDirection;

        // Local Right is 90 degrees clockwise from Local Up
        Vector2 globalRight = new Vector2(-globalUp.Y, globalUp.X);

        // Reconstruct the global vector:
        // localVelocity.X moves along the 'Right' axis
        // localVelocity.Y moves along the 'Up' axis (assuming negative Y is jump/up in your state)
        return (globalRight * localVelocity.X) + (globalUp * localVelocity.Y);
    }

    public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        GD.Print(CurrentState.GetType().Name);
        CurrentState.player = this;
        CurrentState.Enter();
    }
    //   //public void JumpOnBouncePad(float bigJump, float smallJump)
    //   //{
    //   //    OnPlayerBouncePadJump?.Invoke();

    //   //    if (jumpPressed)
    //   //    {
    //   //        Jump(bigJump);
    //   //    }
    //   //    if (!jumpPressed)
    //   //    {
    //   //        Jump(smallJump);
    //   //    }
    //   //}


}
