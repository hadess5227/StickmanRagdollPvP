using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollCreatures
{
	/// <summary>
	/// Simple explosion script to fling ragdolls around.
	/// </summary>
	public class BulletExplosion : MonoBehaviour, IInputSystem
	{
		#region Settings
		[Header("Settings")]
		[Range(0.0f, 30.0f)]
		public float explosionRadius = 2;

		[Range(0, 500)]
		public float explosionForce = 100;
		#endregion

		#region Input System
		[Header("Input System")]
		public bool useNewInputSystem;
		#endregion
		public Transform expObj;
		public int damage;
		void Start()
        {

		}
		void Awake()
		{
			useNewInputSystem = InputSystemSwitcher.UseNewInputSystem;
		}


		public void Update()
		{
			
		}

		public void OnExplode()
		{
			if (expObj)
				GameObject.Instantiate(expObj, transform.position, Quaternion.identity);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
			RagdollCreature ragdoll = null;

			foreach (Collider2D collider in colliders)
			{
				// Only RagdollCreatures are affected by the explosion
				// You can extend the script to every Rigidbody you want
				RagdollLimb limb = collider.GetComponent<RagdollLimb>();
				if (null != limb && limb.isCenterOfRagdoll)
				{
					Vector2 dir = limb.rigidbody.transform.position - transform.position;
					dir.Normalize();
					limb.rigidbody.AddForce(dir * explosionForce, ForceMode2D.Impulse);

					ragdoll = limb.transform.root.GetComponent<RagdollCreature>();
					ragdoll.deactivateMusclesInAir = true;
				}
			}

			if(ragdoll != null)
            {
				ragdoll.GetComponent<Health>().SetHealth(ragdoll.GetComponent<Health>().GetHealth() - damage);
            }
			
			Destroy(transform.root.gameObject);
		}

		public bool UseNewInputSystem()
		{
			return useNewInputSystem;
		}

		public void SetUseNewInputSystem(bool useNewInputSystem)
		{
			this.useNewInputSystem = useNewInputSystem;
		}
	}
}
