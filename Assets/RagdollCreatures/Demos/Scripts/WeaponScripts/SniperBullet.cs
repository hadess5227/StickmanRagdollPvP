using UnityEngine;
using System.IO;
using UnityEditor;
using Photon.Pun;
using System.Collections;
namespace RagdollCreatures
{
    public class SniperBullet : MonoBehaviour, IWeapon
    {
        #region Internal
        private int damage = 0;
        public GameObject expObj;
        private WeaponType weaponType = WeaponType.Bullet;
        #endregion
        public GameObject bulletExplosion;
        public float damageRadius = 0;
        public GameObject _parent;
        public GameObject _GetParent()
        {
            return _parent;
        }
        public void SetParent(GameObject parent)
        {
            _parent = parent;
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

        void OnDestroy()
        {
            
        }

        IEnumerator explode()
        {
            if (bulletExplosion != null)
            {
                GameObject exp = GameObject.Instantiate(bulletExplosion, transform.position, Quaternion.identity);
                if(expObj)
                    exp.GetComponent<BulletExplosion>().expObj = expObj.transform;
                exp.GetComponent<BulletExplosion>().explosionRadius = damageRadius;
                exp.GetComponent<BulletExplosion>().damage = 2 * damage;
                exp.GetComponent<BulletExplosion>().OnExplode();

                if (gameObject.name.Contains("grenade"))
                {
                    Debug.LogError(exp.name);
                    Debug.LogError(exp.GetComponent<BulletExplosion>().expObj.name);
                }
            }
            yield return new WaitForSeconds(0.01f);
            if(gameObject.name.Contains("grenade"))
                Debug.LogError(gameObject.name);

            Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collider)
        {
            //if (collider.gameObject.tag != "Player" && bulletExplosion != null && damageRadius > 0)
            

            if (collider.gameObject.tag != "Bullet" && collider.transform.root.gameObject != _parent)
            {
                if (collider.gameObject.GetComponent<RagdollLimb>() && collider.transform.root.GetComponent<Health>().health <= 0)// && collider.transform.root.GetComponent<Health>().health <= 0)
                {
                    if (RoomManager.Instance)
                    {
                        if (_parent && _parent.GetComponent<PhotonView>() && _parent.GetComponent<PhotonView>().IsMine)
                        {
                            GamePlay.Instance.AddCoins(GameConstant.KILL_MAN_COIN);//get coins when dead
                        }
                    }
                    else if(RoomManager.Instance == null)
                    {
                        GameObject parent = _GetParent();
                        /*
                        if (parent && parent.GetComponent<RagdollCreature>().aiCont == false && collider.transform.root.GetComponent<RagdollCreature>().aiCont == true)
                        {
                            GamePlay.Instance.AddCoins(GameConstant.KILL_MAN_COIN);//get coins when dead
                        }
                        */
                    }
                    collider.transform.root.GetComponent<PlayerController>().targetEnemy = _parent;
                }
                StartCoroutine(explode());
                //Destroy(gameObject);
            }
        }
    }
}
