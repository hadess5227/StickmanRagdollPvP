using UnityEngine;

namespace RagdollCreatures
{
	public enum WeaponType { Harmless, Bullet, Meele }
	public enum WeaponKind { Sword, TyreIron, HandGun, AssaultRifle, Uzi, Grenade, ShotGun, RocketLauncher}
	
	public interface IWeapon
	{
		public GameObject _GetParent();
		public void SetParent(GameObject parent);
		public WeaponType GetWeaponType();

		public void SetWeaponType(WeaponType weaponType);

		public int GetDamage();

		public void SetDamage(int damage);
	}
}
