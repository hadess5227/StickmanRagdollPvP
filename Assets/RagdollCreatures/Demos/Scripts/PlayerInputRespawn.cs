using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollCreatures
{
	public class PlayerInputRespawn : MonoBehaviour
	{
		IEnumerator coroutine;

		public void OnPlayerInputDeath(GameObject gameObject)
		{
			foreach (PlayerInput playerInput in PlayerInput.all)
			{
				if (playerInput.gameObject == gameObject)
				{
					coroutine = ExecuteAfterTime(playerInput);
					StartCoroutine(coroutine);
					break;
				}
			}
		}

		IEnumerator ExecuteAfterTime(PlayerInput playerInput)
		{
			yield return new WaitForSeconds(2.0f);

			foreach (Interact interact in playerInput.gameObject.GetComponentsInChildren<Interact>())
			{
				interact.Reset();
			}

			Destroy(playerInput.gameObject);
			Destroy(playerInput);
		}
	}
}