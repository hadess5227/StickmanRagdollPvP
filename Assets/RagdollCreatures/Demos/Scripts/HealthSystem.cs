using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
namespace RagdollCreatures
{
    public class HealthSystem : MonoBehaviour
    {
        [Serializable]
        public class HealthSystemDeathEvent : UnityEvent<GameObject> { }

        #region Settings
        [Range(0.0f, 5000.0f)]
        public float minMeeleForce = 1500.0f;
        #endregion

        #region Events
        public HealthSystemDeathEvent OnDeath;
        #endregion

        #region Internal
        private List<RagdollCreature> ragdollCreatures = new List<RagdollCreature>();
        #endregion
        RagdollCreature ragdollCreature;

        private void Start()
        {

        }

        void Update()
        {
            foreach (RagdollCreature ragdollCreature in FindObjectsOfType<RagdollCreature>())
            {
                if (!ragdollCreatures.Contains(ragdollCreature))
                {
                    ragdollCreature.OnRagdollLimbCollisionEnter2D.AddListener(doHitDamage);
                    ragdollCreatures.Add(ragdollCreature);
                }
            }
        }

        IEnumerator Respawn(string name)
        {
            yield return new WaitForSeconds(20.0f);

            if (Game_Manager.Instance.ended == false)
            {
                GameObject aiObj = GameObject.Instantiate(GamePlay.Instance.aiPrefab);//respawn
                aiObj.GetComponent<RagdollCreature>().centerObj.GetComponent<AIController>().AIName = name;
                Vector3 pos = GamePlay.Instance.spPoints[UnityEngine.Random.RandomRange(0, 8)].position;
                aiObj.transform.position = pos;
            }
        }

        public void doHitDamage(RagdollLimb limb, Collision2D col)
        {
            {
                ragdollCreature = limb.GetRootParent();
                if (null != ragdollCreature)
                {
                    if (ragdollCreature.isDead)
                    {
                        return;
                    }

                    Health health = ragdollCreature.GetComponent<Health>();
                    if (null != health)
                    {
                        IWeapon weapon = col.collider.gameObject.GetComponent<IWeapon>();

                        if (null != weapon && weapon.GetWeaponType() != WeaponType.Harmless && ((weapon._GetParent() && limb.transform.root.gameObject != weapon._GetParent()) || weapon._GetParent() == null))
                        {
                            int newHealth = health.GetHealth() - weapon.GetDamage();

                            if (newHealth <= 0)
                            {
                                health.SetHealth(0);

                                if (RoomManager.Instance == null && ragdollCreature.aiCont)
                                {
                                    if(weapon._GetParent() && ragdollCreature.aiCont == true && weapon._GetParent().GetComponent<RagdollCreature>().aiCont == false)
                                    {
                                        GamePlay.Instance.AddCoins(GameConstant.KILL_MAN_COIN);
                                        
                                    }
                                    string name = ragdollCreature.centerObj.GetComponent<AIController>().AIName;
                                    StartCoroutine(Respawn(name));
                                    StartCoroutine(DestroyObjCoroutine(ragdollCreature));
                                }

                                if (RoomManager.Instance && ragdollCreature.GetComponent<PhotonView>() && ragdollCreature.GetComponent<PhotonView>().IsMine)
                                {
                                    GamePlay.Instance.WeaponDisable(ragdollCreature.gameObject);
                                    Time.timeScale = 0.25f;
                                    StartCoroutine(ShowGameOver(ragdollCreature));
                                }
                                else if(RoomManager.Instance == null)
                                {
                                    if (ragdollCreature.aiCont == false)
                                    {
                                        GamePlay.Instance.WeaponDisable(ragdollCreature.gameObject);
                                        Time.timeScale = 0.25f;
                                        StartCoroutine(ShowGameOver(ragdollCreature));
                                    }
                                }

                                ragdollCreature.DeactivateAllMuscles();
                                ragdollCreature.isDead = true;
                                OnDeath.Invoke(ragdollCreature.gameObject);

                                foreach (RagdollLimb ragdollLimb in ragdollCreature.ragdollLimbs)
                                {
                                    ragdollLimb.isActiveGroundDetection = false;
                                    if (null != ragdollLimb && ragdollLimb.isCenterOfRagdoll)
                                    {
                                        Vector2 dir = limb.rigidbody.transform.position - col.collider.transform.position;
                                        dir.Normalize();
                                        limb.rigidbody.AddForce(dir * 30, ForceMode2D.Impulse);
                                    }
                                }
                            }
                            else
                            {
                                if (!RoomManager.Instance)
                                {
                                    if (ragdollCreature.aiCont == false)
                                    {
                                        newHealth = health.GetHealth() - Mathf.RoundToInt(weapon.GetDamage() / 2.0f);
                                    }
                                    else
                                    {
                                        newHealth = health.GetHealth() - Mathf.RoundToInt(weapon.GetDamage() * 2.0f);
                                    }
                                }

                                if (col.collider.gameObject.tag != "spear")
                                    health.SetHealth(newHealth);
                                else
                                    health.SetBoneHealth(newHealth);

                            }
                        }
                    }
                }
            }
        }
        public IEnumerator DestroyObjCoroutine(RagdollCreature ragdollCreature)
        {
            yield return new WaitForSeconds(5.0f);
            if (ragdollCreature)
                Destroy(ragdollCreature.gameObject);
        }



        IEnumerator ShowGameOver(RagdollCreature ragdollCreature)
        {
            yield return new WaitForSeconds(1.5f);

            if (RoomManager.Instance && ragdollCreature.GetComponent<PhotonView>() && ragdollCreature.GetComponent<PhotonView>().IsMine)
            {
                Time.timeScale = 1.0f;
                Game_Manager.Instance.ended = true;
                GamePlay.Instance.GameOverPanel.SetActive(true);
                ragdollCreature.GetComponent<PhotonView>().RPC("RPC_DestroySelf", RpcTarget.All);

                foreach (GameObject ragdoll in GameObject.FindGameObjectsWithTag("ragdoll"))
                {
                    Destroy(ragdoll);
                }
            }
            else if(RoomManager.Instance == null)
            {
                if (ragdollCreature.aiCont == false)
                {
                    Time.timeScale = 1.0f;
                    Game_Manager.Instance.ended = true;
                    GamePlay.Instance.GameOverPanel.SetActive(true);
                    foreach (GameObject ragdoll in GameObject.FindGameObjectsWithTag("ragdoll"))
                    {
                        Destroy(ragdoll);
                    }
                }
            }
        }
    }
}
