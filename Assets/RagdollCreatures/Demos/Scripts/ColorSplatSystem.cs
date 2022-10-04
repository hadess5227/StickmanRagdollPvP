using System.Collections.Generic;
using UnityEngine;

namespace RagdollCreatures
{
	public class ColorSplatSystem : MonoBehaviour
	{
		#region Settings
		public ParticleSystem colorSplatParticleSystemPrefab;
		public ParticleSystem hitParticleSystemPrefab;
		#endregion

		#region Internal
		private ParticleSystem colorSplatSystem;
		private ParticleSystem hitParticleSystem;
		#endregion

		#region Internal
		private List<RagdollCreature> ragdollCreatures = new List<RagdollCreature>();
		#endregion

		void Update()
		{
			foreach (RagdollCreature ragdollCreature in FindObjectsOfType<RagdollCreature>())
			{
				if (!ragdollCreatures.Contains(ragdollCreature))
				{
					ragdollCreature.OnRagdollLimbCollisionEnter2D.AddListener(doColorSplat);
					ragdollCreatures.Add(ragdollCreature);
				}
			}
		}

		public void doColorSplat(RagdollLimb limb, Collision2D col)
		{
			IWeapon weapon = col.collider.GetComponent<IWeapon>();
			if (null == weapon)
			{
				return;
			}

			RagdollCreature ragdollCreature = limb.GetRootParent();
			if (null != ragdollCreature)
			{
				if (ragdollCreature.isDead)
				{
					return;
				}
			}

			if (weapon.GetWeaponType() == WeaponType.Bullet && weapon._GetParent() != ragdollCreature.gameObject)
			{
				SpwanColorSplats(limb, col);
				SpawnBulletHitEffect(limb, col);
			}
			else if (weapon.GetWeaponType() == WeaponType.Meele && weapon._GetParent() != ragdollCreature.gameObject)
			{
				SpwanColorSplats(limb, col);
			}
		}

		private void SpawnBulletHitEffect(RagdollLimb limb, Collision2D col)
		{
			SpriteRenderer spriteRenderer = limb.GetComponent<SpriteRenderer>();
			if (null != spriteRenderer)
			{
				Color color = spriteRenderer.color;
				if (null == hitParticleSystem)
				{
					hitParticleSystem = Instantiate(hitParticleSystemPrefab);
				}

				ParticleSystem.MainModule main2 = hitParticleSystem.main;
				main2.startColor = color;
				ContactPoint2D point2 = col.GetContact(0);
				hitParticleSystem.transform.position = point2.point;
				hitParticleSystem.transform.forward = point2.relativeVelocity;

				hitParticleSystem.Play();
			}
		}

		private void SpwanColorSplats(RagdollLimb limb, Collision2D col)
		{
			SpriteRenderer spriteRenderer = limb.GetComponent<SpriteRenderer>();
			if (null != spriteRenderer)
			{
				Color color = spriteRenderer.color;
				if (null == colorSplatSystem)
				{
					colorSplatSystem = Instantiate(colorSplatParticleSystemPrefab);
				}

				ParticleSystem.MainModule main = colorSplatSystem.main;
				main.startColor = color;
				ContactPoint2D point = col.GetContact(0);
				colorSplatSystem.transform.position = point.point;
				colorSplatSystem.transform.rotation = col.collider.gameObject.transform.rotation;

				colorSplatSystem.Play();
			}
		}
	}
}
