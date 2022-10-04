using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class StartUI : MonoBehaviour
{
    public static StartUI Instance;
    public float loadingTime = 0;
    public GameObject FreeCratePanel;
    public GameObject EpicCratePanel;
    public GameObject DesCratePanel;
    public GameObject DeathCratePanel;

    public Text playerCoin;
    public Text playerName;
    const string playerNamePrefKey = "PlayerName";
    List<string> names = new List<string>();

    public RectTransform UI1;
    public RectTransform UI2;
    public RectTransform UI3;

    public int coinClickNum = 0;
    public int logoClickNum = 0;
    public Toggle SingleToggle;
    public Toggle MultiToggle;
    public bool onlineMode = false;
    // Start is called before the first frame update
    void Start()
    {
        float vRate = Screen.height / 1080.0f;
        float hRate = Screen.width / 1920.0f;
        UI1.transform.localScale = Vector3.one * 1.0f * vRate;
        UI1.GetComponent<RectTransform>().anchoredPosition = new Vector2(UI1.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI1.GetComponent<RectTransform>().anchoredPosition.y * vRate);
        UI2.transform.localScale = Vector3.one * 1.0f * vRate;
        UI2.GetComponent<RectTransform>().anchoredPosition = new Vector2(UI2.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI2.GetComponent<RectTransform>().anchoredPosition.y * vRate);
        UI3.transform.localScale = Vector3.one * 1.0f * vRate;
        UI3.GetComponent<RectTransform>().anchoredPosition = new Vector2(UI3.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI3.GetComponent<RectTransform>().anchoredPosition.y * vRate);

        Instance = this;
        GenerateRandomName();
    }

    void OnEnable()
    {
        SetCoins();

        if(Game_Manager.Instance)
        {
            SetPlayerMode();
            Game_Manager.Instance.ended = false;
        }
    }

    public void SetPlayerMode()
    {
        if (Game_Manager.Instance.PlayerMode == 0)
            SingleToggle.isOn = true;
        else
            MultiToggle.isOn = true;

        SetOnlineMode();
    }

    public void SetCoins()
    {
        if (Game_Manager.Instance)
        {
            playerCoin.GetComponent<NumberEffect>().mark = Game_Manager.Instance.originCoins;
            playerCoin.GetComponent<NumberEffect>().num = Game_Manager.Instance.updatedCoins;
            playerCoin.GetComponent<NumberEffect>().time = 0.0f;
            playerCoin.text = playerCoin.GetComponent<NumberEffect>().mark + "";
        }
    }

    public void GuranteeFreeCoin()
    {
        if (coinClickNum >= 5 && logoClickNum >= 5)
        {
            PlayerPrefs.SetInt("PlayerCoin", 5000);
            playerCoin.text = PlayerPrefs.GetInt("PlayerCoin").ToString();
            Game_Manager.Instance.originCoins = 5000;
            Game_Manager.Instance.updatedCoins = 5000;
            SetCoins();
        }
    }

    public void OnCoinClick()
    {
        coinClickNum++;
        GuranteeFreeCoin();
    }
    public void OnLogoClick()
    {
        logoClickNum++;
        GuranteeFreeCoin();
    }
    void SetUIAsResolution()
    {

    }
    public void GenerateRandomName()
    {
        TextAsset file = Resources.Load<TextAsset>("NameList");
        string content = file.text;
        names = new List<string>(content.Split("\n"));
        string firstName = names[Random.RandomRange(0, 24)];
        string secondName = names[Random.RandomRange(24, 56)];
        int num = Random.RandomRange(0, 1000);
        playerName.text = firstName + "-" + secondName + "-" + num;

        PhotonNetwork.NickName = playerName.text;

        PlayerPrefs.SetString(playerNamePrefKey, playerName.text);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void HideAllCrates()
    {
        FreeCratePanel.SetActive(false);
        EpicCratePanel.SetActive(false);
        DesCratePanel.SetActive(false);
        DeathCratePanel.SetActive(false);
    }
    public void QuoteButtonClicked(GameObject CratePanel)
    {
        HideAllCrates();
        CratePanel.SetActive(true);
    }

    public void StartOffline()
    {
        SceneManager.LoadScene(3);
    }

    public void SetOnlineMode()
    {
        onlineMode = MultiToggle.isOn;

        if(onlineMode == true)
        {
            PlayerPrefs.SetInt("PlayerMode",1);
        }
        else
        {
            PlayerPrefs.SetInt("PlayerMode", 0);
        }

        if(Game_Manager.Instance)
            Game_Manager.Instance.PlayerMode = PlayerPrefs.GetInt("PlayerMode");
    }
}
