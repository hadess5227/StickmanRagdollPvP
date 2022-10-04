using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RagdollCreatures;

public class PlayerController : MonoBehaviour
{
    public GameObject[] Weapons;
    public Interact interact;
    public Transform body;
    public GameObject targetEnemy;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(body.transform.localPosition.y < -10 && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        */
    }

    private void OnEnable()
    {
        
    }
    public void GetBoneBonus()
    {
        GamePlay.Instance.boneBonusObj.SetActive(true);
        GamePlay.Instance.AddCoins(5);
    }
    [PunRPC]
    public void RPC_GetBoneBonus(int viewID)
    {
        if (viewID == GetComponent<PhotonView>().ViewID)
        {
            GamePlay.Instance.boneBonusObj.SetActive(true);
            GamePlay.Instance.AddCoins(5);
        }
    }

    [PunRPC]
    public void RPC_OnAttack(Vector2 dir)
    {
        if (null != interact.currentInteractable)
        {
            IInteractable interactable = interact.currentInteractable.GetComponent<IInteractable>();
            if (null != interactable)
            {
                interactable.interact(interact.parent.gameObject, dir);
            }
        }
    }
    public void OnAttack()
    {
        if (null != interact.currentInteractable)
        {
            IInteractable interactable = interact.currentInteractable.GetComponent<IInteractable>();
            if (null != interactable)
            {
                interactable.interact(interact.parent.gameObject, interact.dir);
            }
        }
    }
    [PunRPC]
    public void RPC_SelectWeapon(bool selected, int index = 0)
    {
        Weapons[index].SetActive(selected);
        if (selected)
            interact.Instance.currentInteractable = Weapons[index];
    }

    public void SelectWeapon(bool selected, int index = 0)
    {
        Weapons[index].SetActive(selected);
        if (selected)
            interact.Instance.currentInteractable = Weapons[index];
    }

    public void DisableWeapon()
    {
        foreach (GameObject weapon in Weapons)
        {
            weapon.SetActive(false);
        }
    }
    [PunRPC]
    public void RPC_DisableWeapon(int viewID)
    {
        if (transform.GetComponent<PhotonView>().ViewID == viewID)
        {
            foreach (GameObject weapon in Weapons)
            {
                weapon.SetActive(false);
            }
        }
    }
}
