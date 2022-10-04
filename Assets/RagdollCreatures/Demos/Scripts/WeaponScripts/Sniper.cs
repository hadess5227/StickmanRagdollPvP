using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using RagdollCreatures;
using UnityEditor;
public class Sniper : MonoBehaviour, IInteractable
{
	#region Settings
	public WeaponKind weaponKind;
	public int ammo_num = 20;
	public Transform startPosition;
	/******* Gun Properties *******/
	public GameObject bulletPrefab;
	[Range(0, 100)]
	public int damage = 3;
	[Range(0.0f, 200.0f)]
	public float bulletSpeed = 60.0f;
	public float fireRate = 2;
	public GameObject muzzleEffect;
	public float damageRadius = 0;
	public AudioClip soundFire;
	public GameObject explodeEffect;
	/******************************/

	[Range(0.0f, 10000.0f)]
	public float recoil = 0.0f;

	[Range(0f, 10.0f)]
	public float shootDelay = 2.0f;//10/fireRate
	public int bulletNum = 0;
	private float lastShootTime;
	#endregion
	public Vector2 dir = Vector2.zero;
	public void interact(GameObject parent, Vector2 _dir)
	{
		shootDelay = 2.0f / fireRate;
		if (Time.time >= (lastShootTime + shootDelay) && ammo_num > 0)
		{
			startPosition.transform.localEulerAngles = Vector3.zero;
			if (weaponKind != WeaponKind.Grenade)
				dir = startPosition.transform.right; //startPosition.position - transform.position;
			else
				dir = _dir;

			float rotation = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
			Debug.DrawRay(startPosition.position, dir);

			if(GetComponent<AudioSource>() && soundFire)
				GetComponent<AudioSource>().Play();
			if (muzzleEffect)
			{
				GameObject shotEffect = Instantiate(muzzleEffect, startPosition);
				//shotEffect.transform.parent = startPosition;
				shotEffect.transform.localPosition = new Vector3(1, 0, 0);
				shotEffect.transform.localEulerAngles = new Vector3(-90, 0, 0);
				shotEffect.transform.localScale = Vector3.one * 0.25f;
			}
			
			GameObject bullet = Instantiate(bulletPrefab);

			if(weaponKind ==WeaponKind.Grenade)
            {
				transform.localEulerAngles -= new Vector3(0, 0, 180);
            }
			
			if(weaponKind == WeaponKind.AssaultRifle || weaponKind == WeaponKind.Uzi || weaponKind == WeaponKind.HandGun)
            {
				bullet.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            }

			SniperBullet sniperBullet = bullet.GetComponent<SniperBullet>();
			sniperBullet.damageRadius = damageRadius;
			sniperBullet.SetParent(transform.root.gameObject);
			if (explodeEffect)
				sniperBullet.expObj = explodeEffect;
			else
				sniperBullet.expObj = null;

			if (null != sniperBullet)
			{
				sniperBullet.SetDamage(damage);
			}

			bulletNum++;
			ammo_num--;
			
			bullet.transform.position = startPosition.position;
			bullet.transform.rotation = startPosition.rotation;
			dir.Normalize();
			bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

			if (weaponKind == WeaponKind.ShotGun)
			{
				startPosition.Rotate(transform.forward, 15);
				dir = startPosition.transform.right;
				Shoot(dir);
				startPosition.Rotate(transform.forward, -30);
				dir = startPosition.transform.right;
				Shoot(dir);
			}


			Rigidbody2D rigidbody = parent.GetComponent<Rigidbody2D>();
			if (null != rigidbody)
			{
				rigidbody.AddForce(-dir * recoil);
			}
			lastShootTime = Time.time;

			/*
			if (bulletNum == fireRate)
			{
				GamePlay.Instance.AttackController.GetComponent<SimpleTouchController>().EndDrag();
				bulletNum = 0;
			}
			*/
		}
	}

	void Shoot(Vector2 dir)
    {
		GameObject bullet = Instantiate(bulletPrefab);
		bullet.transform.position = startPosition.position;
		bullet.transform.rotation = startPosition.rotation;
		dir.Normalize();
		bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

		SniperBullet sniperBullet = bullet.GetComponent<SniperBullet>();

		sniperBullet.damageRadius = damageRadius;
		if (explodeEffect)
			sniperBullet.bulletExplosion.GetComponent<BulletExplosion>().expObj = explodeEffect.transform;
		else
			sniperBullet.bulletExplosion.GetComponent<BulletExplosion>().expObj = null;

		if (null != sniperBullet)
		{
			sniperBullet.SetDamage(damage);
		}

		//bulletNum++;
		//ammo_num--;
	}
}
