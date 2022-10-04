using UnityEngine.InputSystem;
using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Switches all RagdollCreatures in Scene from ragdoll to active ragdoll or otherwise.
	/// </summary>
	public class RagdollSwitcher : MonoBehaviour, IInputSystem
	{
		#region Input System
		[Header("Input System")]
		public bool useNewInputSystem;
		#endregion

		#region Internal
		private bool switcher = false;
		#endregion

		void Awake()
		{
			useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
		}

		void Update()
		{
			if (!useNewInputSystem)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					switchRagdollState();
				}
			}
		}

		public void OnRagdollSwitch(InputAction.CallbackContext context)
		{
			if (context.started && useNewInputSystem)
			{
				switchRagdollState();
			}
		}

		private void switchRagdollState()
		{
			RagdollCreature[] ragdolls = FindObjectsOfType<RagdollCreature>();
			foreach (RagdollCreature ragdoll in ragdolls)
			{
				if (switcher)
				{
					ragdoll.ActivateAllMuscles();
				}
				else
				{
					ragdoll.DeactivateAllMuscles();
				}
			}
			switcher = !switcher;
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
