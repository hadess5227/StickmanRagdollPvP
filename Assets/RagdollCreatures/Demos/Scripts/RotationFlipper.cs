using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Flip GameObjects by specified rotation and axes.
	/// 
	/// Example: Sword always faced the correct direction.
	/// </summary>
	public class RotationFlipper : MonoBehaviour
	{
		public enum FlipMode { X, Y, Z }
		public float scale = 1f;
		public float offsetX = 0.2f;
		Vector3 pos0;
		public GameObject fMouse;
		#region Properties
		public bool activeRotationFlip = false;

		[Range(-180.0f, 180.0f)]
		public float minRotation = -90.0f;

		[Range(-180.0f, 180.0f)]
		public float endRotation = 90.0f;

		public FlipMode flipMode;
        #endregion
        private void Start()
        {
			pos0 = transform.localPosition;
        }

        void Update()
		{
			if (activeRotationFlip)
			{
				float rotation = Vector2.Angle(transform.right, Vector2.right);

				if(tag == "sword" || tag == "tyre")
                {
					rotation = Vector2.Angle(-1* fMouse.transform.up, Vector2.right);
                }

				if (rotation > 95)
				{
					switch (flipMode)
					{
						case FlipMode.X:
							transform.localScale = new Vector3(
								1,
								-1,
								1) * scale;
							transform.localPosition = new Vector3(pos0.x-offsetX, pos0.y, pos0.z);

							break;

						case FlipMode.Y:
							transform.localScale = new Vector3(
								-1,
								1,
								1) * scale;
							break;

						case FlipMode.Z:
							transform.localScale = new Vector3(
								1,
								1,
								-1) * scale;
							break;
					}
				}
				else
				{
					transform.localScale = new Vector3(
						1,
						1,
						1) * scale;

					transform.localPosition = pos0;
				}
			}
		}
	}
}