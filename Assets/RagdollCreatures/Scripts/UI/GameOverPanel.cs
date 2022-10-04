using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverPanel : MonoBehaviour
{
    static public GameOverPanel Instance;
    public Text earnedCoinTxt;
    public Text coinTxt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        earnedCoinTxt.text = GamePlay.Instance.earnedCoins + "";
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
        coinTxt.text = GamePlay.Instance.GetOriginCoins() + "";
        coinTxt.GetComponent<NumberEffect>().mark = GamePlay.Instance.GetOriginCoins();
        coinTxt.GetComponent<NumberEffect>().num = GamePlay.Instance.GetOriginCoins();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectCoins()
    {
        GamePlay.Instance.SetPlayerCoins();
    }

    public void CollectCoinsAfterAD()
    {
        GamePlay.Instance.Coins += GamePlay.Instance.earnedCoins;
        GamePlay.Instance.SetPlayerCoins();
    }
}
