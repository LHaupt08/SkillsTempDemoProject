using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;

public partial class Controler : CharacterBody3D
{
    [Export]
    public float JumpVelocity = 4.5f;

    // Mouse Controls (Setup)
    private Vector2 _mouseDelta;
    [Export] public float mouseSensitivity = 0.5f;
    public float headYRotation = 0f;
    private float _cameraXRotation;
    [Export]
    public CharacterBody3D player;
    [Export]
    public Camera3D camera;
    [Export]
    public Node3D head;
    [Export]
    private Label3D debugVelocity;
    [Export]
    private Label3D debugSpeed;
    [Export]
    private Label3D debugAccel;
    [Export]
    private Label3D debugDecel;
    [Export]
    private Label3D debugTurn;
    [Export]
    private Label3D debugIsReversable;
    [Export]
    private Label3D debugIsBreaking;
    [Export]
    private Label3D mouseTarget;

    [ExportGroup("VehicleXSpeed")]
    [Export]
    public float curSpeed = 0.0f;
    [Export]
    public float minSpeed = 0.0f;
    [Export]
    public float maxSpeed = 60.0f;
    [Export]
    public float acceleration = 0.81f;
    [Export]
    public float deceleration = 0.35f;
    [Export]
    public float breakDeceleration = 0.22f;
    [Export]
    public bool isAbleToReverse = true;
    [Export]
    public bool isBreaking = false;
    [Export]
    public float revMaxSpeed = 10.0f;
    [Export]
    public float revAcceleration = 0.6f;
    [Export]
    public float revDeceleration = 0.2f;

    [ExportGroup("VehicleTurning")]
    [Export]
    public float turnSpeed = 0.0f;
    [Export]
    public float maxTurnSpeed = 10.0f;

    [Export]
    public float turnAcceleration = 0f;
    [Export]
    public bool isAbleToTurn = true;
    [Export]
    public float startingTurnPlus = 0.01f;
    public float turnUp;
    public int dirOfTurn = 0;

    [ExportGroup("Input Actions")]
    [Export]
    public string input_left = "ui_left";
	[Export]
	public string input_right = "ui_right";
	[Export]
	public string input_forward = "ui_up";
    [Export]
    public string input_back = "ui_down";
    [Export]
    public string input_accept = "ui_accept";
	[Export]
	public string input_release = "ui_cancel";

    public override void _PhysicsProcess(double delta)
	{
        Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed(input_accept) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector(input_left, input_right, input_forward, input_back);

		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        Vector3 xDirection = (Transform.Basis * new Vector3(0, 0, inputDir.Y)).Normalized();

        Vector3 zDirection = (Transform.Basis * new Vector3(inputDir.X, 0, 0)).Normalized();

        // Handles For/Back Movement.
        if (xDirection != Vector3.Zero)
		{

            if (zDirection != Vector3.Zero)
            {

                if ((Input.IsActionPressed(input_left) || Input.IsActionPressed(input_right)) & isAbleToTurn == true)
                {

                    if (Input.IsActionJustPressed(input_left))
                    {
                        dirOfTurn = 1;
                        if (turnUp > startingTurnPlus)
                        {
                            turnUp -= startingTurnPlus * 4;
                        }
                    }
                    else if (Input.IsActionJustPressed(input_right))
                    {
                        dirOfTurn = -1;
                        if (turnUp > startingTurnPlus)
                        {
                            turnUp -= startingTurnPlus * 4;
                        }

                    }

                    if (turnSpeed >= maxTurnSpeed)
                    {
                        turnSpeed = maxTurnSpeed;
                    }
                    else if (turnSpeed <= -(maxTurnSpeed))
                    {
                        turnSpeed = -(maxTurnSpeed);
                    }
                    else
                    {
                        turnSpeed += turnUp;
                        turnSpeed /= (turnAcceleration * ((float)Math.PI / 180f));
                        turnSpeed = ((turnSpeed * ((float)Math.PI / 180f)) * (float)delta) * dirOfTurn;
                        if (turnSpeed < maxTurnSpeed)
                        {
                            turnUp += startingTurnPlus;
                        }
                    }

                    if (turnSpeed == 0f)
                    {

                        turnUp = startingTurnPlus;

                    }

                }


            }
            else
            {
                dirOfTurn = 0;

                if (turnSpeed > 0)
                {
                    turnSpeed -= (maxTurnSpeed * 2) * (float)delta;
                    if (turnSpeed < 0)
                    {
                        turnSpeed = 0f;
                    }
                }
                else if (turnSpeed < 0)
                {
                    turnSpeed += (maxTurnSpeed * 2) * (float)delta;
                    if (turnSpeed > 0)
                    {
                        turnSpeed = 0f;
                    }
                }

                if (turnUp > startingTurnPlus)
                {
                    turnUp -= startingTurnPlus * 4;
                    if (turnUp < startingTurnPlus)
                    {
                        turnUp = startingTurnPlus;
                    }
                }

                if (turnSpeed == 0f)
                {

                    turnUp = startingTurnPlus;

                }

            }

            player.RotateY(turnSpeed);

            if (curSpeed >= 0)
            {

                if (Input.IsActionPressed(input_forward))
                {

                    if (curSpeed >= maxSpeed)
                    {

                        this.curSpeed = maxSpeed;

                    }
                    else
                    {

                        if (curSpeed < 10)
                        {
                            this.curSpeed += 0.3f;
                        }
                        if (curSpeed > 45)
                        {
                            this.curSpeed -= 1.8f;
                        }

                        this.curSpeed /= acceleration;

                    }

                    //this.curSpeed = (float)Math.Round(this.curSpeed);
                    velocity.X = xDirection.X * this.curSpeed;
                    velocity.Z = xDirection.Z * this.curSpeed;

                    isAbleToReverse = false;

                }

                // Break

                if (Input.IsActionPressed(input_back) && isAbleToReverse == false)
                {
                    isBreaking = true;

                    if (curSpeed < minSpeed)
                    {
                        this.curSpeed = minSpeed;
                    }
                    else
                    {

                        this.curSpeed *= breakDeceleration;
                        this.curSpeed = (float)Math.Round(this.curSpeed, 2);

                    }
                    velocity.X = Mathf.MoveToward(Velocity.X, 0, (this.curSpeed + breakDeceleration));
                    velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (this.curSpeed + breakDeceleration));

                } 
                else
                {
                    isBreaking = false;
                }

                // Reverse
                if (isAbleToReverse == true & Input.IsActionPressed(input_back))
                {

                    if (curSpeed >= revMaxSpeed)
                    {

                        this.curSpeed = revMaxSpeed;

                    }
                    else
                    {

                        if (curSpeed == 0)
                        {
                            this.curSpeed += 0.3f;
                        }

                        this.curSpeed /= revAcceleration;
                        this.curSpeed = (float)Math.Round(this.curSpeed, 2);

                    }

                    velocity.X = xDirection.X * (this.curSpeed + revAcceleration);
                    velocity.Z = xDirection.Z * (this.curSpeed + revAcceleration);

                }

            }

        }
        else
		{

            if (curSpeed <= minSpeed)
            {
                this.curSpeed = minSpeed;
            }
            else
            {

                if (curSpeed > 45)
                {
                    this.curSpeed += 8f;
                }
                this.curSpeed *= deceleration;

            }

            velocity.X = Mathf.MoveToward(Velocity.X, 0, (float)Math.Round((this.curSpeed+deceleration), 2));
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (float)Math.Round((this.curSpeed + deceleration), 2));

            if (velocity.X == 0 & velocity.Z == 0 & curSpeed== 0 & !(Input.IsActionPressed(input_back)))
            {

                isAbleToReverse = true;

            }
            
		}

		Velocity = velocity;
		MoveAndSlide();
		ProcessLook();

        //debugAccel.Text = Convert.ToString(turnUp);
        debugDecel.Text = Convert.ToString(head.Rotation.Y);

        debugSpeed.Text = Convert.ToString(this.curSpeed);
        debugVelocity.Text = "( " + Convert.ToString(Velocity.X) + ", " + Convert.ToString(Velocity.Y) + ", " + Convert.ToString(Velocity.Z) + " )";
        debugTurn.Text = Convert.ToString(turnUp);
        debugIsReversable.Text = Convert.ToString(isAbleToReverse);
        debugIsBreaking.Text = Convert.ToString(isBreaking);

    }

    private void ProcessLook()
    {
        var deltaX = _mouseDelta.Y * mouseSensitivity;
        var deltaY = -_mouseDelta.X * mouseSensitivity;
        headYRotation = Mathf.DegToRad(deltaY);
        debugAccel.Text = Convert.ToString(head.GlobalRotation.Y);

        if (head.Rotation.Y < -Mathf.Pi / 2)
        {
            headYRotation = -Mathf.Pi / 2;
        }
        else if (head.Rotation.Y > Mathf.Pi / 2)
        {
            headYRotation = Mathf.Pi / 2 ;
        }
        head.RotateY(headYRotation);

        head.RotateX(head.Rotation.X);
        head.RotateZ(head.Rotation.X);

        if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
        {
            camera.RotateX(Mathf.DegToRad(-deltaX));
            _cameraXRotation += deltaX;
        }

       /* Vector2 mousePos = GetViewport().GetMousePosition();

        mouseTarget.Position = new Vector3(mousePos.X, mousePos.Y, debugAccel.Position.Z);*/
        _mouseDelta = Vector2.Zero;

        debugVelocity.Text = "( " + Convert.ToString(Velocity.X) + ", " + Convert.ToString(Velocity.Y) + ", " + Convert.ToString(Velocity.Z) + " )";
    }

    // Camera

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion mouseMotion)
        {
            _mouseDelta += mouseMotion.Relative;
        }
    }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetPaused(false);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed(input_release))
            {
                SetPaused(!GetTree().Paused);
            }
        }

        public void SetPaused(bool paused)
        {
            GetTree().Paused = paused;

           if (paused)
            {
                //Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                //Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    
}
