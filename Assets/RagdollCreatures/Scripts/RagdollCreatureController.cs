using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
namespace RagdollCreatures
{
	/// <summary>
	/// Generic controller for RagdollLimbs.
	/// 
	/// Contains many adjustment possibilities to cover as many use cases as possible.
	/// Play around with the settings in the inspector to get a feeling for what they do.
	/// For a more detailed description look at the documentation, code comments or video tutorials.
	/// </summary>
	[RequireComponent(typeof(RagdollCreature))]
	public class RagdollCreatureController : MonoBehaviour, IRagdollCreatureController
	{
		#region Movement
		public RagdollCreatureMovement movement;
		public AbstractRagdollCreatureController controller;
		
		#endregion

		#region Effects
		[Header("Effects")]
		public bool playWalkEffect = false;
		public ParticleSystem walkParticleSystem;

		public bool playJumpEffect = false;
		public ParticleSystem jumpParticleSystem;
		#endregion

		#region Internal
		private RagdollCreature creature;
		#endregion

		void Awake()
		{
			creature = GetComponent<RagdollCreature>();
			controller = new AbstractRagdollCreatureController(creature, movement);
			/*
			this.GetComponent<AbstractRagdollCreatureController>().ragdollCreature = creature;
			this.GetComponent<AbstractRagdollCreatureController>().movement = movement;
			if (null == movement)
			{
				this.GetComponent<AbstractRagdollCreatureController>().movement = ScriptableObject.CreateInstance(typeof(RagdollCreatureMovement)) as RagdollCreatureMovement;
			}
			this.GetComponent<AbstractRagdollCreatureController>().movement.useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
			*/
		}

		void OnDestroy() { }

		public void Update()
		{
			controller.Update();
		}

		public void FixedUpdate()
		{
			controller.FixedUpdate();
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			controller.OnMove(context);
		}

		public void OnRagdollLimbCollisionEnter2D(object sender, Collision2D col)
		{
			OnRagdollLimbCollisionEnter2D(sender, col);
		}

		public void OnRagdollLimbCollisionExit2D(object sender, Collision2D col)
		{
			OnRagdollLimbCollisionExit2D(sender, col);
		}

		public void OnTriggerEnter2D(Collider2D col)
		{
			OnTriggerEnter2D(col);
		}

		public void OnTriggerExit2D(Collider2D col)
		{
			OnTriggerExit2D(col);
		}

		public bool UseNewInputSystem()
		{
			return controller.UseNewInputSystem();
		}

		public void SetUseNewInputSystem(bool useNewInputSystem)
		{
			controller.SetUseNewInputSystem(useNewInputSystem);
		}
	}
}
