using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Interface for interactable objects.
	/// </summary>
	public interface IInteractable
	{
		void interact(GameObject parent, Vector2 _dir);
	}
}
