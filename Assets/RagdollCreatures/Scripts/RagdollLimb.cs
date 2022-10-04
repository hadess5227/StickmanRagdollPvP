using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RagdollCreatures;
using Photon.Pun;
/// <summary>
/// Represents a limb of a ragdoll creature.
/// 
/// For each limb a rotaion can be set to maintain the idle position.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class RagdollLimb : MonoBehaviour
{
	[Serializable]
	public class RagdollLimbCollisionEvent : UnityEvent<RagdollLimb, Collision2D> { }

	[Serializable]
	public class RagdollLimbTriggerEvent : UnityEvent<RagdollLimb, Collider2D> { }

	#region Muscle
	[Header("Muscle")]
	public bool isMuscleActive = true;

	[Range(-180.0f, 180.0f)]
	public float muscleRotation;

	[Range(0.0f, 10.0f)]
	public float muscleForce = 1;

	private static float MINIMUM_MUSCLE_FORCE = 0.001f;
	#endregion

	#region Ground Control
	[Header("Ground Check")]
	public bool isActiveGroundDetection = false;

	public LayerMask groundLayer;

	public Vector2 groundCheckOriginOffset = Vector2.down;

	[Range(0.0f, 5.0f)]
	public float groundCheckRadius = 0.25f;

	public bool isGrounded = false;
	#endregion

	#region Physic
	// A complete ragdoll can only have on limb as center of mass.
	// If multiple limbs are set as center a random one is used.
	[Header("Physic")]
	public bool isCenterOfRagdoll = false;

	// If false the standard center of mass of the rigidbody is used
	public bool useCustomCenterOfMass = false;
	public Vector2 customCenterOfMass;
	#endregion

	#region Gizmos
	[Header("Gizmos")]
	public bool isGizmos = false;

	public bool isMuscleRotationEditor = false;
	#endregion

	#region Collision
	[Header("Collision Events")]
	public RagdollLimbCollisionEvent OnRagdollLimbCollisionEnter2D;
	public RagdollLimbCollisionEvent OnRagdollLimbCollisionExit2D;

	public RagdollLimbTriggerEvent OnRagdollLimbTriggerEnter2D;
	public RagdollLimbTriggerEvent OnRagdollLimbTriggerExit2D;
	#endregion

	#region Internal
	[NonSerialized]
	public new Rigidbody2D rigidbody;
	private new Collider2D collider;
	private float currentInertia;

	private bool currentIsMuscleActive;
	private bool muscleActivated;
	#endregion

	void Awake()
	{
		// Get required rigidbody
		rigidbody = GetComponent<Rigidbody2D>();
		currentInertia = rigidbody.inertia;

		// Get optinal collider
		collider = GetComponent<Collider2D>();
	}

		
	void FixedUpdate()
	{
		if (muscleActivated)
		{
			StartCoroutine(SmoothStandUp());
		}

		// Check whether muscle should do its job
		if (isMuscleActive)
		{
			// Moves the limb to the specified position
			rigidbody.MoveRotation(muscleRotation);

			// In most 2D ragdoll tutorials you see something like that. 
			// Tipp: If you want to use this method, remember that Mathf.Lerp t only supports a value from 0 to 1.
			// So if Time.fixedDeltaTime = 0.02 then muscleForce can only be 0 to 50.
			// rigidbody.MoveRotation(Mathf.Lerp(rigidbody.rotation, muscleRotation, muscleForce * Time.fixedDeltaTime));
		}

		if (muscleForce >= MINIMUM_MUSCLE_FORCE)
		{
			// To stand up smoothly from ragdoll to active ragdoll, use Mathf.Lerp or Mathf.SmoothDump to set the muscleForce
			rigidbody.inertia = currentInertia * muscleForce;
		}
		else
		{
			// If muscle is deactivated or muscle force <= MINIMUM_MUSCLE_FORCE, set MINIMUM_MUSCLE_FORCE
			rigidbody.inertia = MINIMUM_MUSCLE_FORCE;
		}
	}

	void Update()
	{
		// Check if muscle is activted/reactivated
		if (isMuscleActive && !currentIsMuscleActive)
		{
			muscleActivated = true;
		} else
		{
			muscleActivated = false;
		}
		currentIsMuscleActive = isMuscleActive;

		// Ground check
		if (collider && isActiveGroundDetection && groundLayer != 0)
		{
			// Actually, a simple check is also fine. Like:
			// isGrounded = collider.Raycast(Vector2.down, new RaycastHit2D[1], colliderPositionY + groundCheckRadius, groundLayer) > 0;
			// But i wanted that the ground check also take the DisableCollider2D script into account.
			Vector2 groundCheckOrigin = collider.bounds.center
				+ transform.TransformDirection(groundCheckOriginOffset);

			Collider2D[] colliderHits = Physics2D.OverlapCircleAll(groundCheckOrigin, groundCheckRadius, groundLayer);

			bool isHittingGround = false;
			foreach (Collider2D col in colliderHits)
			{
				// Check if some collisions are ignored
				if (!Physics2D.GetIgnoreCollision(collider, col) && collider != col)
				{
					isHittingGround = true;
					
					if(col.tag == "spear")
                    {
						if(transform.root.GetComponent<PlayerController>().targetEnemy && transform.root.GetComponent<Health>().GetHealth() <= 0)
                        {
							if (RoomManager.Instance)
								transform.root.GetComponent<PhotonView>().RPC("RPC_GetBoneBonus", RpcTarget.All, transform.root.GetComponent<PlayerController>().targetEnemy.GetComponent<PhotonView>().ViewID);
							else if(transform.root.GetComponent<PlayerController>().targetEnemy.GetComponent<RagdollCreature>().aiCont == false && transform.root.GetComponent<RagdollCreature>().aiCont)
								GamePlay.Instance.localPlayer.GetBoneBonus();
						}
					}
					
					break;
				}
			}
			isGrounded = isHittingGround;

			/*
			if(isGrounded)
            {
				if (transform.root.GetComponent<RagdollCreature>())
				{
					if(isMuscleActive == false)
                    {
						GamePlay.Instance.boneBonus.SetActive(true);
						GamePlay.Instance.SetCoins(5);
					}
				}
            }
			*/
		}
		else
		{
			isGrounded = false;
		}

		// Set custom center of mass
		if (useCustomCenterOfMass && rigidbody.centerOfMass != customCenterOfMass)
		{
			rigidbody.centerOfMass = customCenterOfMass;
		}
	}

	// The OnCollison/OnTrigger events trigger UnityEvents so that you can bind
	// to the events from outside via the Inspector with other scripts.
	void OnCollisionEnter2D(Collision2D col)
	{
		/*
		GameObject ragDoll = transform.root.gameObject;
		if (col.gameObject.tag == "sword" || col.gameObject.tag == "tyre")
		{
			ragDoll.GetComponent<Health>().SetHealth(ragDoll.GetComponent<Health>().GetHealth() - col.gameObject.GetComponent<Sword>().damage);
		}
		*/
		OnRagdollLimbCollisionEnter2D.Invoke(this, col);
	}

	void OnCollisionExit2D(Collision2D col)
	{
		OnRagdollLimbCollisionExit2D.Invoke(this, col);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		OnRagdollLimbTriggerEnter2D.Invoke(this, col);
	}

	void OnTriggerExit2D(Collider2D col)
	{
		OnRagdollLimbTriggerExit2D.Invoke(this, col);
	}

	private IEnumerator SmoothStandUp()
	{
		if (!isMuscleActive)
		{
			yield return null;
		}
		float elapsedTime = 0;
		float waitTime = 0.35f;
		float velocityY = 0.0f;

		while (elapsedTime < waitTime)
		{
			float smoothY = Mathf.SmoothDamp(rigidbody.velocity.y, 0, ref velocityY, Time.fixedDeltaTime);
			rigidbody.velocity = new Vector2(rigidbody.velocity.x, smoothY);
			elapsedTime += Time.fixedDeltaTime;
			yield return null;
		}
	}

	public void ResetMuscleRotation()
	{
		muscleRotation = 0;
	}

	public void SyncMuscleRotationWithGameObject()
	{
		muscleRotation = transform.localRotation.eulerAngles.z;
	}

	public RagdollCreature GetRootParent()
	{
		GameObject parent = gameObject;
		RagdollCreature ragdollCreature = null;
		while (null != parent.transform.parent)
		{
			parent = parent.transform.parent.gameObject;
			ragdollCreature = parent.GetComponent<RagdollCreature>();
			if (null != ragdollCreature)
			{
				break;
			}
		}

		return ragdollCreature;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (isGizmos)
		{
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			Gizmos.DrawIcon(rb.worldCenterOfMass, "ToolHandleCenter@2x", true);

			Collider2D collider = GetComponent<Collider2D>();
			if (null != collider)
			{
				Vector2 groundCheckOrigin = collider.bounds.center
					+ transform.TransformDirection(groundCheckOriginOffset);

				Gizmos.DrawLine(collider.bounds.center, groundCheckOrigin);
				Gizmos.DrawWireSphere(groundCheckOrigin, groundCheckRadius);
			}
		}
	}
#endif
}
