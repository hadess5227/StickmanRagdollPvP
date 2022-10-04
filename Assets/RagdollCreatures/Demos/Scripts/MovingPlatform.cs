using UnityEngine;

namespace RagdollCreatures
{
	public class MovingPlatform : MonoBehaviour
	{
		#region Settings
		[Header("Settings")]
		public Vector3 targetPosition;
		public float speed;
		#endregion

		#region Internal
		private Vector3 startPosition;
		private Vector3 nextPosition;
		#endregion

		void Awake()
		{
			startPosition = transform.position;
		}

		void Update()
		{
			if (transform.position == startPosition)
			{
				nextPosition = targetPosition;
			}
			if (transform.position == targetPosition)
			{
				nextPosition = startPosition;
			}
			transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
		}
	}
}
