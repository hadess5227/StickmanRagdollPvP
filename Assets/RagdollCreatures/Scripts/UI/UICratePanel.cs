using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum CrateType
{
    Standard,
    Epic,
    Destruction,
    Death
}

public class UICratePanel : MonoBehaviour
{
    public GameObject CrateOpenPanel;
    public Image loadBar;
    float time = 0.0f;
    public CrateType type;
    public GameObject[] ChestItems = new GameObject[8];

    public RectTransform UI1;
    public RectTransform UI2;
    public RectTransform UI3;
    // Start is called before the first frame update
    void Start()
    {
        float vRate = Screen.height / 1080.0f;
        float hRate = Screen.width / 1920.0f;
        if (UI1 && UI2 && UI3)
        {
            UI1.transform.localScale = Vector3.one * 1.1f * vRate;
            UI1.GetComponent<RectTransform>().anchoredPosition = 1.1f * new Vector2(UI1.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI1.GetComponent<RectTransform>().anchoredPosition.y * vRate);
            UI2.transform.localScale = Vector3.one * 1.1f * vRate;
            UI2.GetComponent<RectTransform>().anchoredPosition = 1.1f * new Vector2(UI2.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI2.GetComponent<RectTransform>().anchoredPosition.y * vRate);
            UI3.transform.localScale = Vector3.one * 1.1f * vRate;
            UI3.GetComponent<RectTransform>().anchoredPosition = 1.1f * new Vector2(UI3.GetComponent<RectTransform>().anchoredPosition.x * hRate, UI3.GetComponent<RectTransform>().anchoredPosition.y * vRate);
        }
    }

    private void OnEnable()
    {
        if(loadBar && transform.GetComponent<AudioSource>())
        {
            transform.GetComponent<AudioSource>().Play();

            for(int i=0;i<ChestItems.Length; i++)
            {
                if(ChestItems[i])
                {
                    foreach(Transform trans in ChestItems[i].transform)
                    {
                        trans.gameObject.SetActive(false);
                    }
                }
            }

            for(int i =0; i< Game_Manager.Instance.chestItems.Count; i++)
            {
                if (ChestItems[Game_Manager.Instance.chestItems[i]-1])
                {
                    ChestItems[Game_Manager.Instance.chestItems[i]-1].SetActive(true);
                    foreach (Transform trans in ChestItems[Game_Manager.Instance.chestItems[i]-1].transform)
                    {
                        trans.gameObject.SetActive(true);
                    }
                    ChestItems[Game_Manager.Instance.chestItems[i]-1].transform.SetAsFirstSibling();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(loadBar)
        {
            if (StartUI.Instance)
            {
                if(StartUI.Instance.onlineMode)
                {
                    if(PhotonNetwork.IsConnected)
                    {
                        if (PhotonNetwork.CurrentRoom == null)
                        {
                            time += Time.deltaTime / 10;

                            if (time <= 2.0f / 3)
                            {
                                StartUI.Instance.loadingTime = time;
                            }
                            else
                            {
                                time = 2.0f / 3;
                            }
                        }
                        else
                        {
                            time =2.0f/3 + PhotonNetwork.LevelLoadingProgress / 3;
                            StartUI.Instance.loadingTime = time;
                        }
                    }
                    else
                    {
                        time += Time.deltaTime/30;
                        if (time <= (1.0f / 3))
                        {
                            StartUI.Instance.loadingTime = time;
                        }
                        else
                        {
                            time = 1.0f / 3;
                        }
                    }
                }
            }

            loadBar.fillAmount = StartUI.Instance.loadingTime;
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void OpenGame()
    {
        bool canOpen = false;
        Game_Manager.Instance.chestItems.Clear();
        if (!loadBar)
        {
            if (type == CrateType.Standard)
            {
                int selItem = Random.RandomRange(0, 10);
                Game_Manager.Instance.chestItems.Clear();
                Game_Manager.Instance.chestItems.Add(Game_Manager.Instance.standardItems[selItem]);
                canOpen = true;
            }
            else if (type == CrateType.Epic)
            {
                Game_Manager.Instance.chestItems.Clear();
                List<byte> tempItems = new List<byte>(Game_Manager.Instance.epicItems);
                for (int i = 0; i < 2; i++)
                {
                    int selItem = Random.RandomRange(0, tempItems.Count);
                    byte selContent = tempItems[selItem];
                    Game_Manager.Instance.chestItems.Add(selContent);
                    while (tempItems.Contains(selContent))
                    {
                        int index = tempItems.IndexOf(selContent);
                        tempItems.RemoveAt(index);
                    }
                }

                canOpen = true;
            }
            else if (type == CrateType.Destruction)
            {
                Game_Manager.Instance.chestItems.Clear();
                List<byte> tempItems = new List<byte>(Game_Manager.Instance.destructionItems);
                for (int i = 0; i < 3; i++)
                {
                    int selItem = Random.RandomRange(0, tempItems.Count);
                    byte selContent = tempItems[selItem];

                    Game_Manager.Instance.chestItems.Add(selContent);

                    while (tempItems.Contains(selContent))
                    {
                        int index = tempItems.IndexOf(selContent);
                        tempItems.RemoveAt(index);
                    }
                }

                if(PlayerPrefs.GetInt("PlayerCoin") >= 300)
                {
                    canOpen = true;
                    int _coin = PlayerPrefs.GetInt("PlayerCoin") - 300;
                    PlayerPrefs.SetInt("PlayerCoin", _coin);
                    Game_Manager.Instance.originCoins = _coin;
                    Game_Manager.Instance.updatedCoins = _coin;
                    StartUI.Instance.SetCoins();
                }
                else
                {
                    canOpen = false;
                }
            }
            else if (type == CrateType.Death)
            {
                List<byte> tempItems = new List<byte>(Game_Manager.Instance.deathItems);
                for (int i = 0; i < 4; i++)
                {
                    int selItem = Random.RandomRange(0, tempItems.Count);
                    byte selContent = tempItems[selItem];
                    Game_Manager.Instance.chestItems.Add(selContent);

                    while (tempItems.Contains(selContent))
                    {
                        int index = tempItems.IndexOf(selContent);
                        tempItems.RemoveAt(index);
                    }
                }

                if (PlayerPrefs.GetInt("PlayerCoin") >= 2500)
                {
                    canOpen = true;
                    int _coin = PlayerPrefs.GetInt("PlayerCoin") - 2500;
                    PlayerPrefs.SetInt("PlayerCoin", _coin);
                    //StartUI.Instance.playerCoin.text = _coin + "";
                    Game_Manager.Instance.originCoins = _coin;
                    Game_Manager.Instance.updatedCoins = _coin;
                    StartUI.Instance.SetCoins();
                }
                else
                {
                    canOpen = false;
                }
            }
        }
        Game_Manager.Instance.selectedType = type;

        if (canOpen)
        {
            if (type == CrateType.Epic)
            {
                AdDisplay.Instance.cratePanel = CrateOpenPanel;

                AdDisplay.Instance.ShowAds();
            }
            else
            {
                CrateOpenPanel.SetActive(true);
                CrateOpenPanel.transform.GetComponent<AudioSource>().Play();
                Launcher_Rag.Instance.Connect();
            }
        }
    }
}
