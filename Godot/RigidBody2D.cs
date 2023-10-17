using Godot;
using System;

public partial class RigidBody2D : Godot.RigidBody2D
{
	// You can use // to comment!
	// This will not change the code and is just for documentation purposes!
	// It is recommended to comment so you know what your code does when you revisit it.
	
	float deccelerationSpeed = 6f;
	float accelerationSpeed = 10f;
	
	float topSpeed = 200f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// GD.Print("Started Program!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print("Frame Passed!");
	}
	
	// Use integrate forces to safely change rigidbody values without janky stuff happenings
	public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
		float velocityX = state.LinearVelocity.X; // Stores X velocity for future use8
		// GD.Print("Current velocity: " + velocityX);
		
		float moveDirection = Input.GetAxis("move_left", "move_right"); // Stores a float between -1 and 1 depending on which key we pressed
		state.ApplyImpulse(new Vector2(accelerationSpeed * moveDirection, 0f));
		
		if(moveDirection == 0) { // deccelerates while no input
			this.LinearDamp = deccelerationSpeed; // Damping will slow the player down to a stop when we aren't pressing A or D
		} else {
			this.LinearDamp = 0f; // Disables damping while pressing A or D.
		}
		
		if(Math.Abs(velocityX) > topSpeed) {
			state.LinearVelocity = new Vector2(velocityX > 0 ? topSpeed : -topSpeed, // Ternary Expression To Cap Top Speed (If velocityX > 0 then itll choose topSpeed. Else -topSpeed).
				state.LinearVelocity.Y);
		}
	}
}
