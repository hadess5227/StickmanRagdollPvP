using UnityEngine;

namespace RagdollCreatures
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ColorSplat : MonoBehaviour
	{
		#region Settings
		public Sprite[] sprites;
		#endregion

		#region Internal
		private SpriteRenderer spriteRenderer;
		#endregion

		void Start()
		{
			/*
			spriteRenderer = GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
			spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
			spriteRenderer.sortingOrder = 5;
			transform.localScale *= Random.Range(0.5f, 1.5f);
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(-360.0f, 360.0f));
			*/
		}
	}
}
