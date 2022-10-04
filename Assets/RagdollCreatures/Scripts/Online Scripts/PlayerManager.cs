using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
	PhotonView PV;

	GameObject controller;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if(PV.IsMine)
		{
			CreateController();
		}

		if(PhotonNetwork.IsMasterClient && PV.IsMine)
        {
			CreateWeapon();
        }
	}

	void CreateController()
	{
		Vector3 pos = GamePlay.Instance.spPoints[Random.RandomRange(0, 8)].position;
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), pos, Quaternion.identity, 0, new object[] { PV.ViewID });
		Camera.main.GetComponent<CameraFollow>().followTransform = controller.GetComponent<PlayerController>().body;
		GamePlay.Instance.localPlayer = controller.GetComponent<PlayerController>();
	}

	void CreateWeapon()
    {
		
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}
}