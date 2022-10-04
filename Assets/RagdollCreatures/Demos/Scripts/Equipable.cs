using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Is used to rotate each equipable item to the correct position.
	/// </summary>
	public class Equipable : MonoBehaviour
	{
		#region Settings
		[Range(-360.0f, 360.0f)]
		public float rotationOffset = 0.0f;
		#endregion
	}
}
