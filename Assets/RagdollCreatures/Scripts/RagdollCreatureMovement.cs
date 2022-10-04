using UnityEngine;

using RagdollCreatures;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RagdollCreatureMovement", order = 1)]
public class RagdollCreatureMovement : ScriptableObject
{
	#region Movement
	[Header("Movement")]
	[Range(0, 100)]
	public int movementSpeed = 18;

	// How fast should the direction be changed
	[Range(0, 100)]
	public int movementLerpFactor = 25;

	// How fast should the player move in the air
	[Range(0, 100)]
	public int jumpMovementSpeed = 8;

	// Interval in which jumping is allowed
	[Range(0f, 3.0f)]
	public float jumpDelay = 0.75f;

	[Range(0, 1000)]
	public int highForce = 240;

	[Range(0, 1000)]
	public int jumpForce = 200;
	// Modifies the gravity while jumping to get better jumping results.
	// Tipp: Use a value over 1 to get a smooth and good looking jump.
	[Range(-10.0f, 10.0f)]
	public float jumpGravityScale = 2f;

	// Modifies the gravity while falling.
	// Tipp: Use a value over 1 to speed up the fall. Feels better than the real physics. 
	[Range(-10.0f, 10.0f)]
	public float fallGravityScale = 2f;

	// Modifies the gravity while grounded.
	// Tipp: Use a low value between 0 and 1 for smoother walking.
	[Range(-10.0f, 10.0f)]
	public float groundGravityScale = 0.5f;

	public bool isRemoveYVelocityBeforeJumping = true;

	public bool isRemoveXVelocityBeforeDirectionSwitch = true;

	// This is not yet fully developed.
	// Actually you can already achieve good results with a low groundGravityScale.
	public bool isSmootherWalking = false;
	#endregion

	#region Input System
	[Header("Input System")]
	public bool useNewInputSystem;
	#endregion
}
