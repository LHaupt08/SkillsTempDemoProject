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
    [Export] public Camera3D camera;

    [ExportGroup("VehicleStuff")]
    [Export]
    public float speed = 0.0f;
    [Export]
    public float acceleration = 0.5f;
    [Export]
    public float maxSpeed = 10.0f;

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
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * this.acceleration;
			velocity.Z = direction.Z * this.acceleration;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, this.speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, this.speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		ProcessLook();
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
    }

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
