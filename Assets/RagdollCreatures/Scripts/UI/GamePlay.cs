using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GamePlay : MonoBehaviourPunCallbacks
{
    public static GamePlay Instance;
    public GameObject boneScoreObj;
    public GameObject boneBonusObj;
    public GameObject GameOverPanel;
    public GameObject aiPrefab;
    public GameObject playerPrefab;
    public PlayerController localPlayer;

    public int Coins = 0;
    public int earnedCoins = 0;
    public ToggleGroup toggleGroup;
    public Toggle sword_toggle;
    public Toggle tyreiron_toggle;
    public Toggle handgun_toggle;
    public Text handgun_txt;
    public Toggle rifle_toggle;
    public Text rifle_txt;
    public Toggle uzi_toggle;
    public Text uzi_txt;
    public Toggle grenade_toggle;
    public Text grenade_txt;
    public Toggle shotgun_toggle;
    public Text shotgun_txt;
    public Toggle rocketlauncher_toggle;
    public Text rocketlauncher_txt;

    public Text CoinTxt;
    public GameObject boneBonus;
    public SimpleTouchController MoveController;
    public SimpleTouchController AttackController;
    public Transform[] spPoints;
    public int aiNum = 8;
    public bool jumpClicked = false;
    public bool backToMenu = false;

    public CrateType selectedType;
    public List<byte> standardItems = new List<byte>() { 1, 1, 1, 2, 2, 2, 3, 3, 3, 3 };
    public List<byte> epicItems = new List<byte>() { 2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6, 8 };
    public List<byte> destructionItems = new List<byte>() { 4, 4, 5, 5, 5, 6, 7, 7, 7, 8 };
    public List<byte> deathItems = new List<byte>() { 5, 6, 6, 6, 7, 7, 7, 8, 8, 8 };
    public List<byte> chestItems = new List<byte>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Coins = GetOriginCoins();
        earnedCoins = 0;
        //CoinTxt.GetComponent<NumberEffect>().mark = Coins;
        //CoinTxt.GetComponent<NumberEffect>().num = Coins;
        //CoinTxt.GetComponent<NumberEffect>().num_txt = CoinTxt;

        if (RoomManager.Instance == null)
            SpawnAIEnemies();
        //SetPlayerFromRoom();
    }

    string GenerateRandomName()
    {
        TextAsset file = Resources.Load<TextAsset>("NameList");
        string content = file.text;
        List<string> names = new List<string>(content.Split("\n"));
        string firstName = names[Random.RandomRange(0, 24)];
        string secondName = names[Random.RandomRange(24, 56)];
        int num = Random.RandomRange(0, 1000);
        return firstName + "-" + secondName + "-" + num;
    }

    void SpawnAIEnemies()
    {
        List<Transform> temp_points = new List<Transform>();
        foreach (Transform trans in spPoints)
        {
            temp_points.Add(trans);
        }

        GameObject aiObj = GameObject.Instantiate(playerPrefab);
        int index = Random.RandomRange(0, temp_points.Count);
        Vector3 pos = temp_points[index].position;
        aiObj.transform.position = pos;
        localPlayer = aiObj.GetComponent<PlayerController>();
        Camera.main.GetComponent<CameraFollow>().followTransform = localPlayer.body.transform;
        
        temp_points.RemoveAt(index);
        for (int i = 0; i < 4; i++)
        {
            aiObj = GameObject.Instantiate(aiPrefab);//respawn
            index = Random.RandomRange(0, temp_points.Count);
            pos = temp_points[index].position;
            temp_points.RemoveAt(index);
            aiObj.transform.position = pos;
            aiObj.GetComponent<RagdollCreature>().centerObj.GetComponent<AIController>().AIName = GenerateRandomName();
        }
    }

    void SetPlayerFromRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("We load the 'Room for 1' ");
            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            if (selectedType == CrateType.Standard)
            {
                Hashtable playerProperty = new Hashtable();
                PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.NickName;
                playerProperty[MultiplayerConstant.PLAYER_WEAPONS_FREE] = chestItems[0];
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);

            }
            else if (selectedType == CrateType.Epic)
            {
                Hashtable playerProperty = new Hashtable();
                for (int i = 0; i < 2; i++)
                {
                    string props = "pwe_" + (i + 1);
                    playerProperty[props] = chestItems[i];
                }

                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
            }
            else if (selectedType == CrateType.Destruction)
            {
                Hashtable playerProperty = new Hashtable();
                for (int i = 0; i < 3; i++)
                {
                    string props = "pwd_" + (i + 1);
                    playerProperty[props] = chestItems[i];
                }

                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
            }
            else if (selectedType == CrateType.Death)
            {
                Hashtable playerProperty = new Hashtable();
                List<byte> tempItems = deathItems;
                for (int i = 0; i < 4; i++)
                {
                    string props = "pwdth_" + (i + 1);
                    playerProperty[props] = chestItems[i];
                }

                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
            }
        }
    }

    private void OnEnable()
    {
        //if(RoomManager.Instance)
        Invoke("SetItems", 0.5f);
        if (Launcher_Rag.Instance)
            Launcher_Rag.Instance.PlayerLeftRoomEvent += PlayerLeftRoomEvent;
    }
    public void JumpClickedDown()
    {
        jumpClicked = true;
    }
    public void JumpClickedUp()
    {
        jumpClicked = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (localPlayer)
        {
            handgun_txt.text = localPlayer.Weapons[2].GetComponent<Sniper>().ammo_num + "";
            rifle_txt.text = localPlayer.Weapons[3].GetComponent<Sniper>().ammo_num + "";
            uzi_txt.text = localPlayer.Weapons[4].GetComponent<Sniper>().ammo_num + "";
            grenade_txt.text = localPlayer.Weapons[5].GetComponent<Sniper>().ammo_num + "";
            shotgun_txt.text = localPlayer.Weapons[6].GetComponent<Sniper>().ammo_num + "";
            rocketlauncher_txt.text = localPlayer.Weapons[7].GetComponent<Sniper>().ammo_num + "";
        }
    }
    private void PlayerLeftRoomEvent()
    {
        SceneManager.LoadScene(1);
    }
    public void BackToMenu()
    {
        backToMenu = true;
        if (RoomManager.Instance)
            PhotonNetwork.LeaveRoom();
        else
            SceneManager.LoadScene(1);
        //PhotonNetwork.Disconnect();
        //PhotonNetwork.LeaveLobby();
        //PhotonNetwork.Disconnect();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }

    void SetItems()
    {
        if (Game_Manager.Instance.selectedType == CrateType.Standard)
        {
            int index = Game_Manager.Instance.chestItems[0];
            for (int i = 0; i < toggleGroup.transform.childCount; i++)
            {
                if (i == (index - 1))
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else if (Game_Manager.Instance.selectedType == CrateType.Epic)
        {
            int index_1 = Game_Manager.Instance.chestItems[0];
            int index_2 = Game_Manager.Instance.chestItems[1];

            for (int i = 0; i < toggleGroup.transform.childCount; i++)
            {
                if (i == (index_1 - 1) || i == (index_2 - 1))
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else if (Game_Manager.Instance.selectedType == CrateType.Destruction)
        {
            int index_1 = Game_Manager.Instance.chestItems[0];
            int index_2 = Game_Manager.Instance.chestItems[1];
            int index_3 = Game_Manager.Instance.chestItems[2];

            for (int i = 0; i < toggleGroup.transform.childCount; i++)
            {
                if (i == (index_1 - 1) || i == (index_2 - 1) || i == (index_3 - 1))
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else if (Game_Manager.Instance.selectedType == CrateType.Death)
        {
            int index_1 = Game_Manager.Instance.chestItems[0];
            int index_2 = Game_Manager.Instance.chestItems[1];
            int index_3 = Game_Manager.Instance.chestItems[2];
            int index_4 = Game_Manager.Instance.chestItems[3];

            for (int i = 0; i < toggleGroup.transform.childCount; i++)
            {
                if (i == (index_1 - 1) || i == (index_2 - 1) || i == (index_3 - 1) || i == (index_4 - 1))
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    toggleGroup.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < toggleGroup.transform.childCount; i++)
        {
            if (toggleGroup.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                WeaponSelect();
                break;
            }
        }
    }

    public void WeaponDisable(GameObject player)
    {
        if (RoomManager.Instance && player && player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<PhotonView>().RPC("RPC_DisableWeapon", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
        }
        else if(RoomManager.Instance == null)
        {
            if (player.GetComponent<RagdollCreature>().aiCont == false)
                player.GetComponent<PlayerController>().DisableWeapon();
        }
    }

    public void WeaponSelect()
    {
        for (int i = 0; i < toggleGroup.transform.childCount; i++)
        {
            if (RoomManager.Instance && localPlayer && localPlayer.GetComponent<PhotonView>().IsMine)
            {
                localPlayer.GetComponent<PhotonView>().RPC("RPC_SelectWeapon", RpcTarget.All, toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn && toggleGroup.transform.GetChild(i).gameObject.activeInHierarchy, i);
            }
            else if(RoomManager.Instance == null)
            {
                localPlayer.SelectWeapon(toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn && toggleGroup.transform.GetChild(i).gameObject.activeInHierarchy, i);
            }
        }
    }

    public int GetOriginCoins()
    {
        if (PlayerPrefs.HasKey("PlayerCoin"))
            return PlayerPrefs.GetInt("PlayerCoin");
        else
            return 0;
    }

    public void SetPlayerCoins()
    {
        /*
        if (GetOriginCoins() < Coins)
            transform.GetComponent<AudioSource>().Play();
        */
        Game_Manager.Instance.originCoins = GetOriginCoins();
        CoinTxt.text = GetOriginCoins() + "";
        Game_Manager.Instance.updatedCoins = Coins;
        PlayerPrefs.SetInt("PlayerCoin", Coins);
        BackToMenu();
    }

    public void AddCoins(int coins)
    {
        Coins += coins;
        earnedCoins += coins;
        boneScoreObj.SetActive(true);
    }

    void HideScore()
    {
        boneScoreObj.SetActive(false);
    }

    void HideBonus()
    {
        boneBonusObj.SetActive(false);
    }
}
