using UnityEngine;
using UnityEngine.Events;
using RagdollCreatures;

/// <summary>
/// Represents a RagdollCreature made of multiple RagdollLimbs.
/// </summary>
[RequireComponent(typeof(DisableCollider2D))]
public class RagdollCreature : MonoBehaviour
{
	#region States
	public GameObject centerObj;
	public bool aiCont = false;
	// Public for other scripts
	[HideInInspector]
	public bool isGrounded;

	public bool deactivateMusclesInAir = true;
	public float deactivateMusclesInAirDelay = 0.2f;
	private float lastGroundedTime;

	[HideInInspector]
	public bool isDead;
	#endregion

	#region Ragdoll Limbs
	// Public for other scripts
	[HideInInspector]
	public RagdollLimb[] ragdollLimbs;

	// Public for other scripts
	// Is used by the RagdollCreatureController to move the ragdoll
	[HideInInspector]
	public RagdollLimb centerOfMass = null;
	#endregion

	#region Animation
	[Header("Animation")]
	public bool useAnimation = false;
	public Animator animator;
	#endregion

	#region Gizmos
	[Header("Gizmos")]
	public bool isGizmos = false;

	public bool isDrawVelocity = false;
	public bool isNormalizedVelocity = false;

	// Shorten the length of the velocity gizmos.
	// Does not affect the actual velocity.
	[Range(0.1f, 2.0f)]
	public float velocityLenghtFactor = 0.5f;

	public bool isDrawCenterOfMass = false;
	#endregion

	#region Collision Events
	// These events summarize all events of the individual RagdollLimbs
	// If you want to have a special action for individual RagdollLimb, use the events at the individual RagdollLimb
	[Header("Collison Events")]
	public RagdollLimb.RagdollLimbCollisionEvent OnRagdollLimbCollisionEnter2D;
	public RagdollLimb.RagdollLimbCollisionEvent OnRagdollLimbCollisionExit2D;

	public RagdollLimb.RagdollLimbTriggerEvent OnRagdollLimbTriggerEnter2D;
	public RagdollLimb.RagdollLimbTriggerEvent OnRagdollLimbTriggerExit2D;
	#endregion

	void Awake()
	{
		Initialize();
	}

	void Update()
	{
		// Try to reinitialize if the current state is broken
		if (ragdollLimbs.Length > 0 && null == centerOfMass)
		{
			Initialize();
		}

		// If any limb is grounded set whole creature as grounded
		isGrounded = false;
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			if (limb.isGrounded)
			{
				isGrounded = true;
			}
		}

		
	}

	private void FixedUpdate()
	{
		if (!isGrounded)
		{
			if (deactivateMusclesInAir && Time.time >= lastGroundedTime + deactivateMusclesInAirDelay)
			{
				DeactivateAllMuscles();
			}
		}
		else
		{
			if (deactivateMusclesInAir)
			{
				ActivateAllMuscles();
				lastGroundedTime = Time.time;
			}
		}
	}

	private void Initialize()
	{
		// Get all limbs of this creature
		ragdollLimbs = GetAllRagdollLimbs();

		resetEvents();

		RagdollLimb highestMass = null;
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			// Subscribe to the limb collision events.
			limb.OnRagdollLimbCollisionEnter2D.AddListener((l, c) => OnRagdollLimbCollisionEnter2D.Invoke(l, c));
			limb.OnRagdollLimbCollisionExit2D.AddListener((l, c) => OnRagdollLimbCollisionExit2D.Invoke(l, c));
			limb.OnRagdollLimbTriggerEnter2D.AddListener((l, c) => OnRagdollLimbTriggerEnter2D.Invoke(l, c));
			limb.OnRagdollLimbTriggerExit2D.AddListener((l, c) => OnRagdollLimbTriggerEnter2D.Invoke(l, c));

			// Try to find the limb that is defined as center of mass
			// If multiple limbs are defined as center of mass, only the last one is taken
			if (limb.isCenterOfRagdoll)
			{
				centerOfMass = limb;
			}

			// Determine the limb with the highest mass
			if (null == highestMass || limb.rigidbody.mass > highestMass.rigidbody.mass)
			{
				highestMass = limb;
			}
		}

		// If no limb is explicit defined as center of mass, take limb with highest mass
		if (null == centerOfMass)
		{
			centerOfMass = highestMass;
		}
	}

	void OnDestroy()
	{
		resetEvents();
	}

	void resetEvents()
	{
		// Unsubscribe to all ragdoll limb collision events
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			limb.OnRagdollLimbCollisionEnter2D.RemoveAllListeners();
			limb.OnRagdollLimbCollisionExit2D.RemoveAllListeners();
			limb.OnRagdollLimbTriggerEnter2D.RemoveAllListeners();
			limb.OnRagdollLimbTriggerExit2D.RemoveAllListeners();
		}
	}

	/// <summary>
	/// Play a phyisc base walk animation.
	/// 
	/// Define more methods for different animations and call them from
	/// RagdollCreatureController or a own controller script.
	/// </summary>
	/// <param name="moveVector"></param>
	public void PlayWalkAnimation(Vector2 moveVector)
	{
		if (null != animator && useAnimation)
		{
			animator.SetBool("WalkR", moveVector.x > 0);
			animator.SetBool("WalkL", moveVector.x < 0);
		}
	}

	// Switch to plain ragdoll mode
	public void DeactivateAllMuscles()
	{
		SetMuscleActive(false);
	}

	// Switch to active ragdoll mode
	public void ActivateAllMuscles()
	{
		SetMuscleActive(true);
	}

	private void SetMuscleActive(bool isMuscleActive)
	{
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			limb.isMuscleActive = isMuscleActive;
		}
	}

	public void ResetAllMuscleRotations()
	{
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			limb.ResetMuscleRotation();
		}
	}

	public void SyncAllMuscleRotationsWithGameObject()
	{
		foreach (RagdollLimb limb in ragdollLimbs)
		{
			limb.SyncMuscleRotationWithGameObject();
		}
	}

	private RagdollLimb[] GetAllRagdollLimbs()
	{
		return GetComponentsInChildren<RagdollLimb>();
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (isGizmos) {
			foreach (RagdollLimb limb in ragdollLimbs)
			{
				// Draw velocity gizmos
				if (isDrawVelocity)
				{
					Vector2 velocity = limb.rigidbody.velocity;
					if (isNormalizedVelocity)
					{
						velocity = velocity.normalized;
					}
					else
					{
						velocity *= velocityLenghtFactor;
					}
					Gizmos.DrawRay(limb.transform.position, velocity);
				}
			}

			// Draw center of mass gizmo
			if (isDrawCenterOfMass && null != centerOfMass)
			{
				Gizmos.DrawIcon(centerOfMass.transform.position, "ToolHandleCenter@2x", true);
			}
		}
	}
#endif
}
