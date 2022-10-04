using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollCreatures
{
	public interface IRagdollCreatureController : IInputSystem
	{
		public void Update();

		public void FixedUpdate();

		public void OnMove(InputAction.CallbackContext context);

		public void OnRagdollLimbCollisionEnter2D(object sender, Collision2D col);

		public void OnRagdollLimbCollisionExit2D(object sender, Collision2D col);

		public void OnTriggerEnter2D(Collider2D col);

		public void OnTriggerExit2D(Collider2D col);
	}
}
