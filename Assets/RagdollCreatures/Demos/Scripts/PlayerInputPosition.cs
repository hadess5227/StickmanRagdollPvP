using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollCreatures
{
	public class PlayerInputPosition : MonoBehaviour
	{
		public Transform[] spawnPositions;
		public Color[] colors;

		public void OnPlayerJoined(PlayerInput playerInput)
		{
			int index = Random.Range(0, spawnPositions.Length);
			playerInput.gameObject.transform.position = spawnPositions[index].position;
			Rigidbody2D rigidbody2D = playerInput.gameObject.GetComponent<Rigidbody2D>();
			if (null != rigidbody2D)
			{
				rigidbody2D.position = spawnPositions[index].position;
			}

			RagdollCreature ragdollCreature = playerInput.gameObject.GetComponent<RagdollCreature>();
			if (null != ragdollCreature)
			{
				int colorIndex = Random.Range(0, colors.Length);
				Color newColor = colors[colorIndex];
				foreach (SpriteRenderer renderer in ragdollCreature.GetComponentsInChildren<SpriteRenderer>())
				{
					if (null != renderer && null != renderer.GetComponent<RagdollLimb>())
					{
						renderer.color = newColor;
					}
				}
			}
		}
	}
}
