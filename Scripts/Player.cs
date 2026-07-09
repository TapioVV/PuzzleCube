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

    public int inputAxis;

    float jumpBufferTimer = 0;
    //[SerializeField] float jumpInputBuffer;

    //[HideInInspector] public Rigidbody2D rb2D;
    //BoxCollider2D boxCollider;
    //[HideInInspector] public Animator animator;
    //SpriteRenderer spriteRenderer;
    //[SerializeField] ParticleSystem particleSystem;

    //[Header("Inputs")]
    //[SerializeField] InputActionReference move;
    //[SerializeField] InputActionReference jump;

    //[Header("SoundEvents")]
    //public UnityEvent OnPlayerNormalJump;
    //[SerializeField] UnityEvent OnPlayerBouncePadJump;

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


    //   //public void PressJump(InputAction.CallbackContext context)
    //   //{
    //   //    jumpPressed = true;
    //   //    jumpBufferTimer = jumpInputBuffer;
    //   //}
    //   //public void JumpBuffer()
    //   //{
    //   //    jumpBufferTimer = Mathf.MoveTowards(jumpBufferTimer, -1, Time.deltaTime);
    //   //    jumpBufferTimer -= deltaf;
    //   //    if (jumpBufferTimer > 0 && jumpPressed == true)
    //   //    {
    //   //        CurrentState.JumpInput();
    //   //    }
    //   //}
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
        stateLabel.Text = $"Current State: {CurrentState.GetType().Name}";
        CurrentState.Update((float)delta);
        inputAxis = (int)Input.GetAxis("move_left", "move_right");

        //JumpBuffer();
        velocity.Y = Mathf.Clamp(velocity.Y, -maxVerticalSpeed, 10000000);
    }
    public override void _PhysicsProcess(double delta)
    {
        Velocity = velocity;
        MoveAndSlide();
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
