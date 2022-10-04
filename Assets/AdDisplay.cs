using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
public class AdDisplay : MonoBehaviour,IUnityAdsInitializationListener, IUnityAdsLoadListener,IUnityAdsShowListener
{
    static public AdDisplay Instance;
    public string myGameIdAndroid = "4909436";
    public string myGameIdIOS = "4909437";
    public string adUnitIdAndroid = "Rewarded_Android";
    public string adUnitIdIOS = "Rewarded_iOS";
    public string myAdUnitId;
    public string myAdStatus = "";
    public bool adStarted;
    public bool adCompleted;

    private bool testMode = true;

    public GameObject cratePanel;

    public GameObject loadingMask;
    public GameObject loading;
    public Text adVert;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;


    }
    void OnEnable()
    {

    }
    public void ShowRewardedVideo()
    {
#if UNITY_IOS
	        Advertisement.Initialize(myGameIdIOS, testMode, this);
	        myAdUnitId = adUnitIdIOS;
#else
        Advertisement.Initialize(myGameIdAndroid, testMode, this);
        myAdUnitId = adUnitIdAndroid;
#endif
        loadingMask.gameObject.SetActive(true);
        loading.gameObject.SetActive(true);

        
    }
    // Update is called once per frame
    void Update()
    {
        //adVert.text = myAdStatus;
    }

    public void OnInitializationComplete()
    {
        if(adVert)
            adVert.text = "Unity Ads initialization complete.";

        Debug.Log("Unity Ads initialization complete.");
        Advertisement.Load(myAdUnitId, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        myAdStatus = message;
        if(adVert)
            adVert.text = "Unity Ads Initialization Failed:";
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        if(adVert)
            adVert.text = "Ad Loaded: " + adStarted + ":" + adCompleted;
        if (!adStarted)
        {
            Advertisement.Show(myAdUnitId, this);
        }
        else
        {
            DoActionAfterADS();
        }
    }
    
    public void ShowAds()
    {
        if (Advertisement.isInitialized)
        {
            loadingMask.gameObject.SetActive(true);
            loading.gameObject.SetActive(true);
            Advertisement.Load(myAdUnitId, this);
        }
        else
        {
            if(adVert)
                adVert.text = "Ad Clicked: INITIAL";
            ShowRewardedVideo();
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        myAdStatus = message;
        if(adVert)
            adVert.text = "Error showing Ad Unit {adUnitId}:Load";
        Advertisement.Load(myAdUnitId, this);
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        myAdStatus = message;
        if(adVert)
            adVert.text = "Error showing Ad Unit:" + adStarted;
        
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        if(adVert)
            adVert.text = "Ad Started: ";
        loadingMask.gameObject.SetActive(false);
        loading.gameObject.SetActive(false);
        adStarted = true;
        Debug.Log("Ad Started: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        if(adVert)
            adVert.text = "Ad Clicked: ";
        Debug.Log("Ad Clicked: " + adUnitId);
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            if(adVert)
                adVert.text = "Ad Finished: ";
            adStarted = false;
            DoActionAfterADS();
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            if(adVert)
                adVert.text = "Ad Skipped: ";
            adStarted = false;
            DoActionAfterADS();
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            if(adVert)
                adVert.text = "Ad Failed: ";
        }
    }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {

        adCompleted = showCompletionState == UnityAdsShowCompletionState.COMPLETED;
        if(adVert)
            adVert.text = "Ad Completed: " + adCompleted;
        //adStarted = false;

        DoActionAfterADS();

        return;
        
        Debug.Log("Ad Completed: " + adUnitId);
    }

    void DoActionAfterADS()
    {
        if (!GamePlay.Instance)
        {
            cratePanel.gameObject.SetActive(true);
            cratePanel.transform.GetComponent<AudioSource>().Play();
            Launcher_Rag.Instance.Connect();
        }
        else
        {
            if (this.gameObject == GamePlay.Instance.GameOverPanel)
            {
                GamePlay.Instance.GameOverPanel.transform.GetChild(2).GetComponent<GameOverPanel>().CollectCoinsAfterAD();
            }
        }
    }
}
