using UnityEngine;
using System.Collections;
using GooglePlayGames;
using AppodealAds.Unity.Api;

public class StartScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        initAds();

        Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            Application.LoadLevel("MainMenu");
        });
    }

    private void initAds() {
        Appodeal.setTesting(true);
        Appodeal.setLogging(true);

        string appKey = "17439d477eae29f7eb218efb949d88287d3388adb2fcbacd";
        Appodeal.disableNetwork("amazon_ads");
        Appodeal.disableNetwork("applovin");
        Appodeal.disableNetwork("mopub");
        Appodeal.disableNetwork("mailru");
        Appodeal.disableNetwork("inmoby");
        //Appodeal.disableNetwork("adcolony");
        Appodeal.disableNetwork("vungle");
        Appodeal.disableNetwork("facebook");
        Appodeal.disableNetwork("yandex");
        Appodeal.disableNetwork("liverail");
        Appodeal.disableLocationPermissionCheck();
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO);

        Appodeal.setTesting(true);
        Appodeal.setLogging(true);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
