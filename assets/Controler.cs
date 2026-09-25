using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;

public partial class Controler : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

    // Mouse Controls (Setup)
    private Vector2 _mouseDelta;
    [Export] public float mouseSensitivity = 0.5f;
    private float _cameraXRotation;
    [Export] public Camera3D camera;

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
	public string input_release = "KEY_ESCAPE";


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
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
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
}
