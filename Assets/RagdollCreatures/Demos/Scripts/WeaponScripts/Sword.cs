using System;
using System.Collections;
using UnityEngine;

namespace RagdollCreatures
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Sword : MonoBehaviour, IInteractable, IWeapon
	{
		#region Settings
		public WeaponKind weaponKind;
		[Range(0, 100)]
		public int damage = 10;

		[Range(0.0f, 5000.0f)]
		public float hitForce = 60.0f;
		public int fireRate = 2;
		public AudioClip soundFire;

		[Range(0f, 10.0f)]
		public float hitDelay = 2.0f;
		private float lastHitTime;
		#endregion

		#region Internal
		private new Rigidbody2D rigidbody;
		private WeaponType weaponType = WeaponType.Meele;
		#endregion
		public GameObject _parent;
		public GameObject _GetParent()
		{
			return _parent;
		}
		public void SetParent(GameObject parent)
		{
			_parent = parent;
		}
		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
		}

		
		public WeaponType GetWeaponType()
		{
			return weaponType;
		}

		public void SetWeaponType(WeaponType weaponType)
		{
			this.weaponType = weaponType;
		}

		public int GetDamage()
		{
			return damage;
		}

		public void SetDamage(int damage)
		{
			this.damage = damage;
		}

		public void interact(GameObject parent, Vector2 _dir)
		{
			if (Time.time >= lastHitTime + hitDelay)
			{
				Rigidbody2D parentRigidbody = parent.GetComponent<Rigidbody2D>();
				if (null != parentRigidbody)
				{
					float angle = parentRigidbody.rotation % 360;
					if (angle > 180)
					{
						angle = (360 - angle) * -1;
					} else if (angle < -180)
					{
						angle = 360 + angle;
					}

					Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
					parentRigidbody.AddForce(q * Vector2.down * hitForce);
				}
				
				StartCoroutine(Hit());
				lastHitTime = Time.time;
			}
		}

		IEnumerator Hit()
		{
			weaponType = WeaponType.Meele;
			yield return new WaitForSeconds(1.0f);
			weaponType = WeaponType.Harmless;
		}
	}
}
