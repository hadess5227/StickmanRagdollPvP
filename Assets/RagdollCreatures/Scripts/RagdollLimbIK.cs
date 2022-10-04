using UnityEngine;
using Photon.Pun;
namespace RagdollCreatures
{
	/// <summary>
	/// Physic based inverse kinematic for RagdollCreatures.
	/// </summary>
	class RagdollLimbIK : MonoBehaviour, IPunObservable
	{
		float angle = 0;
		public enum ChainLengthResolver { Automatic, Manually }
		public FollowMouse fMouse;
		#region IK
		// The moving target
		[Header("IK Target")]
		public GameObject ikTarget = null;

		// Only needed if automatic chain length resolver is used
		[Header("IK RagdollLimb Chain")]
		public GameObject firstLimb = null;
		public GameObject lastLimb = null;

		public ChainLengthResolver chainLengthResolver = ChainLengthResolver.Manually;
		private ChainLengthResolver currentChainLengthResolver;

		[Range(0, 5)]
		public int chainLength = 0;

		[Header("IK Settings")]
		public bool isActive = false;

		[Range(0, 25)]
		public int iterations = 10;

		public bool isUseIKForce = true;

		[Range(0.0f, 10.0f)]
		public float ikMuscleForce = 1;
		public bool forceLimbToBeActive = true;

		// This must be used to compensate rotations from editor/inspector
		// Did not find a better way yet.
		// Tipp: If limbs not aim direct to the IK target, try to adjust the offset.
		[Range(-360.0f, 360.0f)]
		public float angleOffset = 0.0f;
		#endregion

		#region Gizmos
		[Header("Gizmos")]
		public bool drawGizmos = false;

		[Range(0.1f, 0.01f)]
		public float gizmoSize = 0.075f;
		public Color gizmoColor = Color.blue;
		#endregion

		#region Internal
		private Transform[] limbs;
		private Vector2[] limbPositions;
		private float[] limbLenghts;
		#endregion

		void Awake()
		{
			initialize();
		}

		void LateUpdate()
		{
			if (isActive && null != ikTarget && null != firstLimb)
			{
				// Check if chain length or resolver has changed and reinitialize
				if (limbLenghts.Length != chainLength
					|| currentChainLengthResolver != chainLengthResolver)
				{
					initialize();
				}

				inverseKinematic();
			}
		}

		void initialize()
		{
			currentChainLengthResolver = chainLengthResolver;

			int maxChainLength = calculateChainLength(true);

			// If last limb is not set, automatic resolving is not possible
			if (null == lastLimb)
			{
				currentChainLengthResolver = ChainLengthResolver.Manually;
			}

			if (currentChainLengthResolver == ChainLengthResolver.Automatic)
			{
				chainLength = calculateChainLength();
			}

			if (chainLength > maxChainLength)
			{
				chainLength = maxChainLength;
			}

			limbs = new Transform[chainLength + 1];
			limbLenghts = new float[chainLength];

			limbPositions = new Vector2[chainLength + 1];

			// Initialize limbs and limb length arrays
			Transform currentTransform = firstLimb.transform;
			for (int i = limbs.Length - 1; i >= 0; i--)
			{
				limbs[i] = currentTransform;
				if (i != chainLength)
				{
					// Calculate the length between current and last limb
					limbLenghts[i] = (limbs[i + 1].position - currentTransform.position).magnitude;
				}

				GameObject nextLimb = getNextLimb(currentTransform.gameObject);
				if (null == nextLimb)
				{
					// Should be unreachable code
					break;
				}
				currentTransform = nextLimb.transform;
			}
		}

		void inverseKinematic()
		{
			// Just collect the limb positions
			for (int i = 0; i < limbs.Length; i++)
			{
				limbPositions[i] = limbs[i].position;
			}

			// Case 1: Distance to IK target is longer than the complete chain length
			// Just calculate the direction vector and add to the current limb position
			if ((ikTarget.transform.position - limbs[0].position).magnitude >= calculateCompleteLimbLength())
			{
				Vector2 direction = (ikTarget.transform.position - limbs[0].position).normalized;
				for (int i = 1; i < limbPositions.Length; i++)
				{
					int lastIndex = i - 1;
					limbPositions[i] = limbPositions[lastIndex] + direction * limbLenghts[lastIndex];
				}
			}
			// Case 2: Distance to IK target is smaler than the complete chain length
			else
			{
				for (int iteration = 0; iteration < iterations; iteration++)
				{
					for (int i = limbPositions.Length - 1; i > 0; i--)
					{
						// First limb position is always IK target position
						if (i == limbPositions.Length - 1)
						{
							limbPositions[i] = ikTarget.transform.position;
						}
						else
						{
							Vector2 nextLimbPosition = limbPositions[i + 1];
							limbPositions[i] = nextLimbPosition + (limbPositions[i] - nextLimbPosition).normalized * limbLenghts[i];
						}
					}

					for (int i = 1; i < limbPositions.Length; i++)
					{
						Vector2 lastLimbPosition = limbPositions[i - 1];
						limbPositions[i] = lastLimbPosition + (limbPositions[i] - lastLimbPosition).normalized * limbLenghts[i - 1];
					}
				}
			}

			// Apply the calculated IK positions to the limbs
			for (int i = 0; i < limbPositions.Length; i++)
			{
				Vector2 targetPosition = ikTarget.transform.position;
				if (i < limbPositions.Length - 1)
				{
					targetPosition = limbPositions[i + 1];
				}
				Vector2 direction = (targetPosition - limbPositions[i]).normalized;

				RagdollLimb limb = limbs[i].GetComponent<RagdollLimb>();
				if (null != limb)
				{
					if (forceLimbToBeActive)
					{
						limb.isMuscleActive = true;
					}

					if (isUseIKForce)
					{
						limb.muscleForce = ikMuscleForce;
					}

					// Vector2.down is used because the limb default/initial muscle rotation is (0, -1)/-90
					if ((RoomManager.Instance && GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine) ||(!RoomManager.Instance && transform.root.GetComponent<RagdollCreature>().aiCont == false))
					{
						angle = Vector2.SignedAngle(Vector2.down, direction);// + angleOffset;

						if (GamePlay.Instance.AttackController.movementVector.magnitude > 0.2f)
							angle = Vector2.SignedAngle(Vector2.down, GamePlay.Instance.AttackController.movementVector);
						else
						{
							if (GamePlay.Instance.MoveController.movementVector.y > 0.2f)
								angle = Vector2.SignedAngle(Vector2.down, GamePlay.Instance.MoveController.movementVector);
							else
							{
								if (GamePlay.Instance.localPlayer.interact.currentInteractable.activeInHierarchy && (GamePlay.Instance.localPlayer.interact.currentInteractable.GetComponent<Sword>() || GamePlay.Instance.localPlayer.interact.currentInteractable.name.ToLower().Contains("grenade")))
								{
									angle = Vector2.SignedAngle(Vector2.down, GamePlay.Instance.AttackController.movementVector);
								}
								else
								{
									angle = 90 * fMouse.direction;// Vector2.SignedAngle(Vector2.down, GamePlay.Instance.AttackController.movementVector);
								}
							}
						}
					}
					else if(!RoomManager.Instance)
                    {
						GameObject enemy = null;
						if(transform.root.GetComponent<RagdollCreature>().centerObj.GetComponent<AIController>())
							enemy = transform.root.GetComponent<RagdollCreature>().centerObj.GetComponent<AIController>().enemy;
						if (enemy)
						{
							Vector3 _direction = enemy.GetComponent<RagdollCreature>().centerObj.transform.position - transform.root.GetComponent<RagdollCreature>().centerObj.transform.position;
							angle = Vector2.SignedAngle(Vector2.down, _direction);
						}
						else
                        {
							angle = 0.0f;
                        }
					}
					
					limb.muscleRotation = angle;
				}
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
			if (GetComponent<PhotonView>().IsMine)
			{
				stream.SendNext(angle);
			}
			else
			{
				angle = (float)stream.ReceiveNext();
				inverseKinematic();
			}
		}

		bool isAutomaticChainLengthResolver()
		{
			return currentChainLengthResolver == ChainLengthResolver.Automatic;
		}

		int calculateChainLength(bool calculateMaxLength = false)
		{
			GameObject current = firstLimb;
			int currentChainLength = 0;
			while (calculateMaxLength ? null != current : current != lastLimb && null != current)
			{
				current = getNextLimb(current);
				currentChainLength++;
			}
			return currentChainLength;
		}

		GameObject getNextLimb(GameObject current)
		{
			AnchoredJoint2D currentJoint = current.GetComponent<AnchoredJoint2D>();
			if (null != currentJoint)
			{
				return currentJoint.connectedBody.gameObject;
			}
			return null;
		}

		float calculateCompleteLimbLength()
		{
			float lenght = 0.0f;
			for (int i = 0; i < limbLenghts.Length; i++)
			{
				lenght += limbLenghts[i];
			}
			return lenght;
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (drawGizmos)
			{
				if (null != ikTarget)
				{
					Gizmos.color = gizmoColor;
					Gizmos.DrawSphere(ikTarget.transform.position, gizmoSize * 3);
				}

				if (firstLimb && null != limbs && chainLength >= 1)
				{
					for (int i = 0; i < limbs.Length; i++)
					{
						Transform limb = limbs[i];
						if (null != limb)
						{
							Gizmos.DrawSphere(limb.transform.position, gizmoSize);
						}
					}
				}

				if (null != limbPositions)
				{
					for (int i = 0; i < limbPositions.Length; i++)
					{
						Vector2 position = limbPositions[i];
						if (null != position)
						{
							Gizmos.color = Color.gray;
							Gizmos.DrawSphere(position, gizmoSize / 2);
						}
					}
				}
			}
		}
#endif
	}
}
