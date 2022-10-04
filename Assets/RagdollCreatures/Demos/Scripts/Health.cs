using UnityEngine;
using Photon.Pun;
using RagdollCreatures;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Health : MonoBehaviourPun
{
    public int maxHealth = 200;
    public int health;
    public GameObject floatHealthBar;
    public GameObject floatName;
    private void Start()
    {
        GameObject floatingName = Instantiate(Resources.Load("FloatingName")) as GameObject;
        GameObject floatingHealthBar = Instantiate(Resources.Load("FloatingHealthBar")) as GameObject;
        floatingName.GetComponent<UIFloatingName>().myPlayer = transform.GetChild(0).GetChild(0).gameObject;
        floatingHealthBar.GetComponent<UIFloatingHealthBar>().myPlayer = transform.GetChild(0).GetChild(0).gameObject;
        floatingName.transform.parent = GameObject.Find("UI").transform;
        floatingHealthBar.transform.parent = GameObject.Find("UI").transform;
        floatingHealthBar.transform.localScale = Vector3.one;
        floatingName.transform.localScale = Vector3.one;

        GetComponent<Health>().floatHealthBar = floatingHealthBar;
        floatName = floatingName;
        if (RoomManager.Instance && GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine == false && GetComponent<RagdollCreature>().aiCont == false)
        {
            //GetComponent<RagdollCreatureController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }

    }
    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int health)
    {
        if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
        {
            transform.GetComponent<PhotonView>().RPC("RPC_SetHealth", RpcTarget.All, health);
        }
        else
        {
            AI_SetHealth(health);
        }

        AudioClip[] auds = Resources.LoadAll<AudioClip>("Music/BodyHits");
        int index = Random.RandomRange(0, auds.Length);
        transform.GetComponent<AudioSource>().clip = auds[index];
        transform.GetComponent<AudioSource>().Play();
    }



    public void SetBoneHealth(int health)
    {
        if (GetComponent<PhotonView>() && GetComponent<PhotonView>().IsMine)
            transform.GetComponent<PhotonView>().RPC("RPC_SetHealth", RpcTarget.All, health);
        {
            //else
            //	AI_SetHealth(health);

            AudioClip[] auds = Resources.LoadAll<AudioClip>("Music/BoneBreak");
            int index = Random.RandomRange(0, auds.Length);
            transform.GetComponent<AudioSource>().clip = auds[index];
            transform.GetComponent<AudioSource>().Play();
        }
        //GamePlay.Instance.boneBonus.SetActive(true);
    }

    void AI_SetHealth(int health)
    {
        this.health = health;
        if(floatHealthBar)
            floatHealthBar.GetComponent<UIFloatingHealthBar>().UpdateHealth(health, maxHealth);
    }

    [PunRPC]
    void RPC_SetHealth(int health)
    {
        this.health = health;

        if(floatHealthBar)
            floatHealthBar.GetComponent<UIFloatingHealthBar>().UpdateHealth(health, maxHealth);
    }
    [PunRPC]
    void RPC_DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Destroy(floatHealthBar.gameObject);
        Destroy(floatName.gameObject);
    }
}

