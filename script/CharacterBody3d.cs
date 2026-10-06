using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 7.0f;
	public const float JumpVelocity = 4.5f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}


		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
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

    // Mouse Controls
    private Vector2 _mouseDelta;
    [Export] public float mouseSensitivity = 0.5f;
    private float _cameraXRotation;
    [Export] public Camera3D camera;

	private void ProcessLook()
	{
        Input.MouseMode = Input.MouseModeEnum.Captured;
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

		if(@event is InputEventMouseMotion mouseMotion)
		{
			_mouseDelta += mouseMotion.Relative;
		}
    }

}
