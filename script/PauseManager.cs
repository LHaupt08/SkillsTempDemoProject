using Godot;
using System;

public partial class PauseManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPaused(false);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause"))
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
