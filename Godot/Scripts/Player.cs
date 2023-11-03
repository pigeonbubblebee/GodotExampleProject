using Godot;
using System;

public partial class Player : Godot.RigidBody2D
{
	// You can use // to comment!
	// This will not change the code and is just for documentation purposes!
	// It is recommended to comment so you know what your code does when you revisit it.
	
	// Usually we would move a lot of this stuff into different classes, like a data class, a run class, and a jump class to follow Single-Responsibility Principle and to make more readable code.
	// We usually don't want a 500 line player class because its harder to debug and if we want to change it, we'd have to go through a bajillion functions instead of just a couple
	// A lot of functionality in this class is also in the Integrate Forces method. We usually want to split those up into their own functions, which I haven't done here.
	
	[Export] // Export tag allows us to see this property in the inspector!
	float deccelerationSpeed = 60f;
	[Export]
	float accelerationSpeed = 40f;
	[Export]
	float accelerationDirectionSwitchSpeed = 80f;
	[Export]
	float topSpeed = 900f;
	
	[Export]
	float jumpHeight = 10;
	[Export]
	float upwardsGravityScale = -10f;
	[Export]
	float downwardsGravityScale = 60f;
	[Export]
	float jumpTime = 1f;
	
	private bool _grounded = false;
	private bool _isJumpReset = true;
	
	[Export]
	NodePath groundedCheckPath;
	private Area2D _groundedCheckArea;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// GD.Print("Started Program!");
		_groundedCheckArea = GetNode<Area2D>(groundedCheckPath); // Caches Ground Check Area So We Don't Have To Reget It Everytime.
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_getGroundedRequest();
		// GD.Print("Frame Passed!");
	}
	
	// Use integrate forces to safely change rigidbody values without janky stuff happening
	public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
		float velocityX = state.LinearVelocity.X; // Stores X velocity for future use
		// GD.Print("Current velocity: " + velocityX);
		
		float moveDirection = Input.GetAxis("move_left", "move_right"); // Stores a float between -1 and 1 depending on which key we pressed
		float jumpDirection = Input.IsActionPressed("jump") ? 1f : 0f; // Ternary Expression To Get Jump (If pressed then itll choose 1. Else 0).
		
		if(_isJumpReset && jumpDirection == 0) { // Released Jump Midair, So Can't Jump Again
			_isJumpReset = false;
		}
		
		if(_grounded) { // Resets Jump
			_isJumpReset = true;
		}
		
		if(jumpDirection == 1) {
			if(!_isJumpReset) { // Try To Jump Again Midair. Stops It
				jumpDirection = 0;
			} else { // Ungrounds.
				_grounded = false;
			}
		}
	
		state.LinearVelocity = new Vector2(velocityX, jumpDirection * jumpHeight * -100f); // Jumps
		
		bool moveDirectionSign = moveDirection > 0f ? true : false; // this part can be optimized, but leaving it like this is for readability
		bool velocityXSign = velocityX > 0f ? true : false;
		
		if(velocityXSign != moveDirectionSign)
			state.ApplyImpulse(new Vector2(accelerationDirectionSwitchSpeed * moveDirection, 0f));
		else
			state.ApplyImpulse(new Vector2(accelerationSpeed * moveDirection, 0f));
		
		if(moveDirection == 0) { // deccelerates while no input
			state = _dampVelocity(state, state.LinearVelocity.X);
		}
		
		if(Math.Abs(velocityX) > topSpeed) {
			state.LinearVelocity = new Vector2(velocityX > 0 ? topSpeed : -topSpeed, // Ternary Expression To Cap Top Speed (If velocityX > 0 then itll choose topSpeed. Else -topSpeed).
				state.LinearVelocity.Y);
		}
		
		if(state.LinearVelocity.Y < 0f) { // Tweaks Gravity Scale Based On If You're Going Up Or Down.
			this.GravityScale = upwardsGravityScale;
		} else {
			this.GravityScale = downwardsGravityScale;
		}
	}
	
	// Method to deccelerate velocity.
	private PhysicsDirectBodyState2D _dampVelocity(PhysicsDirectBodyState2D state, float originalVelocity) {
		state.ApplyImpulse(new Vector2((originalVelocity > 0 ? -1f : 1f) * deccelerationSpeed, 0f)); // Pushes player back
		
		bool originalSign = originalVelocity > 0 ? true : false;
		bool newSign = state.LinearVelocity.X > 0 ? true : false;
		
		if(newSign != originalSign) { // If player is now going the other way, it stops instead.
			state.LinearVelocity = new Vector2(0f, state.LinearVelocity.Y);
		}
		
		return state;
	}
	
	private void _checkColliderGrounded(Node2D body) {
		if(body.IsInGroup("ground")) { // Checks if body is the ground using the Group feature of Godot (You can add objects to groups)
			_grounded = true;
			
		}
	}
	
	private void _getGroundedRequest() {
		foreach(Node2D body in _groundedCheckArea.GetOverlappingBodies()) { // Checks all bodies in the Area2D we cached. We then pass it to checkColliderGrounded to see if its the ground.
			_checkColliderGrounded(body);
		}
	}
}
