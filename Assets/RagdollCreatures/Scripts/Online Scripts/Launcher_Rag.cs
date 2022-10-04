using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public enum CrateItem
{
	Sword = 1,
	TyreIron = 2,
	Handgun_20Ammo = 3,
	AssaultRifle_100Ammo = 4,
	Uzi_100Ammo = 5,
	Grenade_5 = 6,
	Shotgun_10Ammo = 7,
	RocketLauncher_5Ammo = 8
}
public class MultiplayerConstant
{
	public static readonly string PLAYER_WEAPONS_FREE = "pwf";
	
	public static readonly string PLAYER_WEAPONS_EPIC_1 = "pwe_1";
	public static readonly string PLAYER_WEAPONS_EPIC_2 = "pwe_2";

	public static readonly string PLAYER_WEAPONS_DESTRUCTION_1 = "pwd_1";
	public static readonly string PLAYER_WEAPONS_DESTRUCTION_2 = "pwd_2";
	public static readonly string PLAYER_WEAPONS_DESTRUCTION_3 = "pwd_3";

	public static readonly string PLAYER_WEAPONS_DEATH_1 = "pwdth_1";
	public static readonly string PLAYER_WEAPONS_DEATH_2 = "pwdth_2";
	public static readonly string PLAYER_WEAPONS_DEATH_3 = "pwdth_3";
	public static readonly string PLAYER_WEAPONS_DEATH_4 = "pwdth_4";
}

public class Launcher_Rag : MonoBehaviourPunCallbacks
{
	public delegate void PlayerLeftRoomDelegate();
	public event PlayerLeftRoomDelegate PlayerLeftRoomEvent;
	public static Launcher_Rag Instance;
	#region Private Serializable Fields

	AsyncOperation loadingOperation;
	private byte maxPlayersPerRoom = 8;

	#endregion

	#region Private Fields
	/// <summary>
	/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
	/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
	/// Typically this is used for the OnConnectedToMaster() callback.
	/// </summary>
	bool isConnecting;

	/// <summary>
	/// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
	/// </summary>
	string gameVersion = "1";

	#endregion

	#region MonoBehaviour CallBacks
	float time = 0.0f;
	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
	/// </summary>
	void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		//DontDestroyOnLoad(gameObject);
		Instance = this;
		// #Critical
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.AutomaticallySyncScene = true;

	}

    private void Update()
    {
		if(loadingOperation != null)
		StartUI.Instance.loadingTime = Mathf.Clamp01(loadingOperation.progress / 0.9f);
    }
    #endregion


    #region Public Methods

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
	{
		// we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
		//feedbackText.text = "";
		
		// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
		isConnecting = true;

		// hide the Play button for visual consistency
		//controlPanel.SetActive(false);

		// start the loader animation for visual effect.
		//if (loaderAnime != null)
		{
		//	loaderAnime.StartLoaderAnimation();
		}

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		
		if(StartUI.Instance && StartUI.Instance.onlineMode)
		{
			if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
			}
			else
			{

				LogFeedback("Connecting...");

				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = this.gameVersion;
			}
		}
		else
		{
			loadingOperation = SceneManager.LoadSceneAsync("MainGame_off");
		}
	}

	/// <summary>
	/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
	/// </summary>
	/// <param name="message">Message.</param>
	void LogFeedback(string message)
	{
		// we do not assume there is a feedbackText defined.
		//if (feedbackText == null)
		{
			return;
		}

		// add new messages as a new line and at the bottom of the log.
		//feedbackText.text += System.Environment.NewLine + message;
	}

	#endregion


	#region MonoBehaviourPunCallbacks CallBacks
	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


	/// <summary>
	/// Called after the connection to the master is established and authenticated
	/// </summary>
	public override void OnConnectedToMaster()
	{
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (isConnecting)
		{
			LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
			Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}

	/// <summary>
	/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
	/// </summary>
	/// <remarks>
	/// Most likely all rooms are full or no rooms are available. <br/>
	/// </remarks>
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
		Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
	}


	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	public override void OnDisconnected(DisconnectCause cause)
	{
		LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
		Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

		if(GamePlay.Instance)
        {
			if (GamePlay.Instance.backToMenu == false)
				GamePlay.Instance.GameOverPanel.SetActive(true);
			else
			{
				SceneManager.LoadScene(1);
			}
        }
		// #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
		//loaderAnime.StopLoaderAnimation();

		isConnecting = false;
		//controlPanel.SetActive(true);

	}

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    /// 
    
    public override void OnJoinedRoom()
	{
		LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

		// #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
		if (PhotonNetwork.InRoom)
		{
			Debug.Log("We load the 'Room for 1' ");
			
		}

		StartCoroutine(LoadLevelCoroutine());

	}
	public IEnumerator LoadLevelCoroutine()
	{
		yield return new WaitForSeconds(0.2f);
		PhotonNetwork.LoadLevel("MainGame");

		//SceneManager.LoadScene("MainGame");
	}
	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom");
		PlayerLeftRoomEvent?.Invoke();
		/*
		if (GamePlay.Instance)
		{
			if (GamePlay.Instance.backToMenu == false)
				GamePlay.Instance.GameOverPanel.SetActive(true);
			else
				SceneManager.LoadScene(1);

		}
		*/
	}
	public override void OnPlayerLeftRoom(Player other)
	{
		Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
		
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
		}
	}
	
	#endregion
}