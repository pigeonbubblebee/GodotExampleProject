using Godot;
using System;

public partial class RigidBody2D : Godot.RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("poo");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print("frame pass");
		
		if(Input.IsActionPressed("move_left")) {
			this.ApplyImpulse(new Vector2(-5f, 0f));
		}
	}
}
