using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;

public partial class Controler : CharacterBody3D
{
	[Export]
	public float JumpVelocity = 4.5f;

    // Mouse Controls (Setup)
    private Vector2 _mouseDelta;
    [Export] public float mouseSensitivity = 0.5f;
    private float _cameraXRotation;
    [Export] 
    public Camera3D camera;
    [Export]
    private Label3D debugVelocity;
    [Export]
    private Label3D debugSpeed;
    [Export]
    private Label3D debugAccel;
    [Export]
    private Label3D debugDecel;

    [ExportGroup("VehicleStuff")]
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
        debugAccel.Text = Convert.ToString(acceleration);
        debugDecel.Text = Convert.ToString(deceleration);
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
		if (direction != Vector3.Zero)
		{

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

                    velocity.X = direction.X * this.curSpeed;
                    velocity.Z = direction.Z * this.curSpeed;

                }

                if (curSpeed > 0 && Input.IsActionPressed(input_back))
                {

                    if (curSpeed <= minSpeed)
                    {
                        this.curSpeed = minSpeed;
                    }
                    else
                    {

                        this.curSpeed *= breakDeceleration;

                    }

                    velocity.X = Mathf.MoveToward(Velocity.X, 0, (this.curSpeed + breakDeceleration));
                    velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (this.curSpeed + breakDeceleration));

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

            velocity.X = Mathf.MoveToward(Velocity.X, 0, (this.curSpeed+deceleration));
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (this.curSpeed+deceleration));
            
		}

		Velocity = velocity;
		MoveAndSlide();
		ProcessLook();

        debugSpeed.Text = Convert.ToString(this.curSpeed);
        debugVelocity.Text = "( " + Convert.ToString(Velocity.X) + ", " + Convert.ToString(Velocity.Y) + ", " + Convert.ToString(Velocity.Z) + " )";

    }

    private void ProcessLook()
    {
        var deltaX = _mouseDelta.Y * mouseSensitivity;
        var deltaY = -_mouseDelta.X * mouseSensitivity;

        RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deltaY));
        if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
        {
            camera.RotateX(Mathf.DegToRad(-deltaX));
            _cameraXRotation += deltaX;
        }

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
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    
}
