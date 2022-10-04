using System.Collections.Generic;
using UnityEngine;

namespace RagdollCreatures
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ColorSplatParticles : MonoBehaviour
	{
		#region Settings
		public GameObject colorSplatPrefab;
		public GameObject colorSplatParent;
		#endregion

		#region Internal
		private new ParticleSystem particleSystem;
		#endregion

		void Awake()
		{
			particleSystem = GetComponent<ParticleSystem>();
		}

		void OnParticleCollision(GameObject other)
		{
			List<ParticleCollisionEvent> events = new List<ParticleCollisionEvent>();
			Color color = particleSystem.main.startColor.color;
			ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, events);
			foreach (ParticleCollisionEvent colEvent in events)
			{
				GameObject colorSplat = Instantiate(colorSplatPrefab, colEvent.intersection, Quaternion.identity);
				if (null != colorSplatParent)
				{
					colorSplat.transform.parent = colorSplatParent.transform;
				}
				SpriteRenderer spriteRenderer = colorSplat.GetComponent<SpriteRenderer>();
				if (null != spriteRenderer)
				{
					spriteRenderer.color = color;
				}
			}
		}
	}
}