using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollCreatures
{
	/// <summary>
	/// Grab physics objects.
	/// </summary>
	[RequireComponent(typeof(RagdollLimb))]
	public class Grab : MonoBehaviour, IInputSystem
	{
		public enum GrabMode { Hinge, Fixed, Spring }

		#region Settings
		[Header("Settings")]
		public GrabMode grabMode;

		public Transform grabPoint;

		[Range(1.0f, 1000.0f)]
		public float springfrequency = 1.0f;

		public LayerMask grabLayer;
		#endregion

		#region Input System
		[Header("Input System")]
		public bool useNewInputSystem;
		#endregion

		#region Internal
		private RagdollLimb limb;

		private bool isGrabbing = false;

		private Joint2D joint;

		private float originalMass;
		#endregion

		void Awake()
		{
			useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
		}

		void Start()
		{
			limb = GetComponent<RagdollLimb>();
			limb.OnRagdollLimbCollisionEnter2D.AddListener((l, collision) =>
			{
				if ((grabLayer.value & (1 << collision.otherCollider.gameObject.layer)) > 0)
				{
					Reset();

					switch (grabMode)
					{
						case GrabMode.Hinge:
							HingeJoint2D hingeJoint = limb.gameObject.AddComponent<HingeJoint2D>();
							if (null != grabPoint)
							{
								hingeJoint.anchor = grabPoint.localPosition;
								hingeJoint.connectedAnchor = grabPoint.localPosition;
							}
							joint = hingeJoint;
							break;
						case GrabMode.Spring:
							SpringJoint2D springJoint = limb.gameObject.AddComponent<SpringJoint2D>();
							springJoint.frequency = springfrequency;
							joint = springJoint;
							break;
						default:
							joint = limb.gameObject.AddComponent<FixedJoint2D>();
							break;
					}
					
					if (null != collision.rigidbody)
					{
						originalMass = limb.rigidbody.gravityScale;
						limb.rigidbody.gravityScale = 3;
						joint.connectedBody = collision.rigidbody;
					}
				}
			});
		}

		void Update()
		{
			if (!isGrabbing)
			{
				Reset();
			}

			if (!useNewInputSystem)
			{
				if (Input.GetMouseButtonDown(1))
				{
					OnGrab(InputActionPhase.Started);
				}
				else if (Input.GetMouseButtonUp(1))
				{
					OnGrab(InputActionPhase.Canceled);
				}
			}
		}

		void Reset()
		{
			limb.rigidbody.gravityScale = originalMass;
			Destroy(joint);
			joint = null;
		}

		public void OnGrab(InputAction.CallbackContext context)
		{
			if (useNewInputSystem)
			{
				OnGrab(context.phase);
			}
		}

		private void OnGrab(InputActionPhase phase)
		{
			switch (phase)
			{
				case InputActionPhase.Started:
					isGrabbing = true;
					break;

				case InputActionPhase.Canceled:
					isGrabbing = false;
					break;
			}
		}

		public bool UseNewInputSystem()
		{
			return useNewInputSystem;
		}

		public void SetUseNewInputSystem(bool useNewInputSystem)
		{
			this.useNewInputSystem = useNewInputSystem;
		}
	}
}
