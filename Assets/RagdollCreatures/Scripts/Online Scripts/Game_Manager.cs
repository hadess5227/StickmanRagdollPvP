using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    static public Game_Manager Instance = null;
    public List<byte> standardItems = new List<byte>() { 1, 1, 1, 2, 2, 2, 3, 3, 3, 3 };
    public List<byte> epicItems = new List<byte>() { 2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6, 8 };
    public List<byte> destructionItems = new List<byte>() { 4, 4, 5, 5, 5, 6, 7, 7, 7, 8 };
    public List<byte> deathItems = new List<byte>() { 5, 6, 6, 6, 7, 7, 7, 8, 8, 8 };
    public List<byte> chestItems = new List<byte>();
    public CrateType selectedType;

    public int originCoins = 0;
    public int updatedCoins = 0;
    public bool ended = false;
    public int PlayerMode = 0;
    void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ended = false;
        if (PlayerPrefs.HasKey("PlayerCoin"))
            originCoins = PlayerPrefs.GetInt("PlayerCoin");
        else
            originCoins = 0;

        if(PlayerPrefs.HasKey("PlayerMode"))
        {
            PlayerMode = PlayerPrefs.GetInt("PlayerMode");
        }
        else
        {
            PlayerMode = 0;
        }


        if (PlayerMode == 0)
            StartUI.Instance.SingleToggle.isOn = true;
        else
            StartUI.Instance.MultiToggle.isOn = true;
        StartUI.Instance.SetOnlineMode();

        updatedCoins = originCoins;

        DontDestroyOnLoad(gameObject);

        if(StartUI.Instance)
        {
            StartUI.Instance.SetCoins();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
