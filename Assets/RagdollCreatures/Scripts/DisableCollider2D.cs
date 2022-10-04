using System.Collections.Generic;
using UnityEngine;

namespace RagdollCreatures
{
	/// <summary>
	/// Disable colliders of different groups within a GameObject.
	/// 
	/// Example: Used to prevent collision of limbs of a ragdoll.
	/// </summary>
	public class DisableCollider2D : MonoBehaviour
	{
		public enum Target
		{
			ThisColliders,
			ThisAndChildrenColliders,
			ChildrenColliders
		}

		public Target target1 = Target.ChildrenColliders;
		public Target target2 = Target.ChildrenColliders;

		private Target currentTarget1;
		private Target currentTarget2;

		void Awake()
		{
			disableColliders(target1, target2);
		}

		void Update()
		{
			// Only recalculate if targets have changed. Performance!
			if (target1 != currentTarget1 || target2 != currentTarget2)
			{
				disableColliders(target1, target2);
			}
		}

		private void disableColliders(Target target1, Target target2)
		{
			// Save current targets
			currentTarget1 = target1;
			currentTarget2 = target2;
			foreach (Collider2D target1Collider in getColliders(target1))
			{
				foreach (Collider2D target2Collider in getColliders(target2))
				{
					Physics2D.IgnoreCollision(target1Collider, target2Collider, true);
				}
			}
		}

		List<Collider2D> getColliders(Target target)
		{
			List<Collider2D> colliders = new List<Collider2D>(GetComponents<Collider2D>());
			switch (target)
			{
				case Target.ThisAndChildrenColliders: return new List<Collider2D>(GetComponentsInChildren<Collider2D>());
				case Target.ChildrenColliders:
					List<Collider2D> childColliders = new List<Collider2D>(GetComponentsInChildren<Collider2D>());
					childColliders.RemoveAll(c => colliders.Contains(c));
					return childColliders;
				default:
					return colliders;
			}
		}
	}
}
