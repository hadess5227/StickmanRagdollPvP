using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Instance;

	void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		//DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
		if(newPlayer.IsLocal == false)
        {
			if(GamePlay.Instance)
            {
				for(int i=0; i<GamePlay.Instance.toggleGroup.transform.childCount; i++)
                {
					GameObject toggle = GamePlay.Instance.toggleGroup.transform.GetChild(i).gameObject;

					if(toggle.activeInHierarchy && toggle.GetComponent<Toggle>().isOn)
                    {
						GamePlay.Instance.WeaponSelect();
						break;
                    }
                }
            }
        }
    }

    public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.buildIndex > 1) // We're in the game scene
		{
			PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
		}
	}
    public override void OnLeftRoom()
    {
		SceneManager.LoadScene(1);
    }
    public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
		SceneManager.LoadScene(1);
		//GamePlay.Instance.GameOverPanel.SetActive(true);
	}
}