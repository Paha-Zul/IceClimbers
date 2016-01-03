using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using AppodealAds.Unity.Api;
using GooglePlayGames;

public class MainMenu : MonoBehaviour {
    public Text coinText;
    public GameObject menuRoot, mainPanel, startPanel, gameTypePanel, itemPanel, canvas;
    public Text currSpeed, currLength, currBounciness, currHardHat, currHookSpeed, currBirdShield;
    public Text costSpeed, costLength, costBounciness, costHardHat, costHookSpeed, costBirdShield;

    [HideInInspector]
    public RectTransform mainPanelRectTransform, mainMenuPanelRectTransform, gameTypePanelRectTransform, itemPanelRectTransform, canvasRectTransform;

    public static MainMenu inst { get; private set; }

    private static bool startedOnce = false;

    // Use this for initialization
    void Start() {
        inst = this;
        this.RefreshCoinAmount();

        this.mainPanelRectTransform = mainPanel.GetComponent<RectTransform>();
        this.mainMenuPanelRectTransform = startPanel.GetComponent<RectTransform>();
        this.gameTypePanelRectTransform = gameTypePanel.GetComponent<RectTransform>();
        this.itemPanelRectTransform = itemPanel.GetComponent<RectTransform>();
        this.canvasRectTransform = canvas.GetComponent<RectTransform>();
        //PlayerPrefs.SetInt("Coins", 0); //Used to reset the coins when they get out of hand.

        this.spaceButtonsOnGameTypePanel();
        this.validatePrefLimits();
        this.checkHash();

        //this.quickReset();
        this.populateUpgrades();

        Appodeal.show(Appodeal.BANNER_BOTTOM);
        if(startedOnce) Appodeal.show(Appodeal.INTERSTITIAL);
        startedOnce = true;
    }

    private void quickReset(){
        PlayerPrefs.SetInt(Defaults.RopeLengthPrefString, 0);
        PlayerPrefs.SetInt(Defaults.RopeSpeedPrefString, 0);
        PlayerPrefs.SetInt(Defaults.BouncinessPrefString, 0);
        PlayerPrefs.SetInt(Defaults.CoinPrefString, 1000);
    }

    private void populateUpgrades() {
        int currRopeSpeed = PlayerPrefs.GetInt(Defaults.RopeSpeedPrefString);
        int currRopeLength = PlayerPrefs.GetInt(Defaults.RopeLengthPrefString);
        int currBounciness = PlayerPrefs.GetInt(Defaults.BouncinessPrefString);
        int currHardHat = PlayerPrefs.GetInt("HardHat");
        int currHookSpeed = PlayerPrefs.GetInt("HookSpeed");
        //int coins = PlayerPrefs.GetInt(Defaults.CoinPrefString);

        Upgrades.refreshOrAdd("ropespeed", "RopeSpeed", 10, 1, Defaults.maxRopeSpeedUpgrades, currRopeSpeed, costSpeed, currSpeed);
        Upgrades.refreshOrAdd("ropelength", "RopeLength", 10, 1, Defaults.MaxRopeLengthUpgrades, currRopeLength, costLength, currLength);
        Upgrades.refreshOrAdd("bounciness", "Bounciness", 10, 1, Defaults.maxBouncinessUpgrade, currBounciness, this.costBounciness, this.currBounciness);
        Upgrades.refreshOrAdd("hardhat", "HardHat", 50, 0, 1, currHardHat, this.costHardHat, this.currHardHat);
        Upgrades.refreshOrAdd("birdshield", "BirdShield", 50, 0, 1, currHardHat, this.costBirdShield, this.currBirdShield);
        Upgrades.refreshOrAdd("hookspeed", "HookSpeed", 10, 1, Defaults.maxHookSpeedUpgrade, currHookSpeed, this.costHookSpeed, this.currHookSpeed);
    }

    /// <summary>
    /// Validates the preferences. Maybe a temporary thing?
    /// </summary>
    private void validatePrefLimits()
    {
        int ropeLengthUps = PlayerPrefs.GetInt(Defaults.RopeLengthPrefString);
        int ropeSpeedUps = PlayerPrefs.GetInt(Defaults.RopeSpeedPrefString);
        int bouncinessUps = PlayerPrefs.GetInt(Defaults.BouncinessPrefString);
        int hookSpeedUps = PlayerPrefs.GetInt(Defaults.HookSpeedPrefString);

        if (ropeLengthUps > Defaults.MaxRopeLengthUpgrades) ropeLengthUps = Defaults.MaxRopeLengthUpgrades;
        if (ropeSpeedUps > Defaults.maxRopeSpeedUpgrades) ropeSpeedUps = Defaults.maxRopeSpeedUpgrades;
        if (bouncinessUps > Defaults.maxBouncinessUpgrade) bouncinessUps = Defaults.maxBouncinessUpgrade;
        if (bouncinessUps > Defaults.maxBouncinessUpgrade) hookSpeedUps = Defaults.maxHookSpeedUpgrade;

        PlayerPrefs.SetInt(Defaults.RopeLengthPrefString, ropeLengthUps);
        PlayerPrefs.SetInt(Defaults.RopeSpeedPrefString, ropeSpeedUps);
        PlayerPrefs.SetInt(Defaults.BouncinessPrefString, bouncinessUps);
        PlayerPrefs.SetInt(Defaults.HookSpeedPrefString, hookSpeedUps);
    }

    /// <summary>
    /// Checks some secret stuff.
    /// </summary>
    private void checkHash() {
        float hash = SecretStuff.getHash2();
        float savedHash = PlayerPrefs.GetInt("hash");

        Debug.Log("hash: " + hash + ", savedHash: " + savedHash);

        //Special problem handling for now.
        if(PlayerPrefs.GetInt("Coins") < 0)
            PlayerPrefs.SetInt("Coins", 0);
    }

    // Update is called once per frame
    void Update() {

    }

    public void RefreshCoinAmount() {
        this.coinText.text = PlayerPrefs.GetInt("Coins").ToString();
    }

    public bool MakePurchase(string type) {
        int coins = PlayerPrefs.GetInt("Coins");
        int amountSpent = 0;
        int increase = 0;

        Upgrades.Upgrade upgrade = Upgrades.getUpgrade(type);
        
        increase = 1;
        amountSpent = upgrade.cost + upgrade.costIncr * upgrade.curr;

        //If we don't have enough coins, return false
        if (coins - amountSpent < 0)
            return false;

        //Increase our value, set it!
        int val = PlayerPrefs.GetInt(upgrade.prefName);
        int limit = upgrade.max;

        //If we are going past the limit, return false.
        if (val + increase > limit)
            return false;

        val += increase;
        PlayerPrefs.SetInt(upgrade.prefName, val);
        upgrade.curr = val;

        //Refresh the items/text
        PlayerPrefs.SetInt("Coins", coins - amountSpent);
        this.RefreshCoinAmount();
        this.refreshItem(type);
        //this.loadItemData();

        return true;
    }

    public void refreshItem(string pref) {
        Upgrades.Upgrade upgrade = Upgrades.getUpgrade(pref);
        upgrade.costText.text = "" + (upgrade.cost + upgrade.costIncr * upgrade.curr).ToString();
        upgrade.currText.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;
    }

    /// <summary>
    /// Loads/refreshes the entire item store.
    /// </summary>
    public void loadItemDataInfoIntoItemStore() {
        foreach (KeyValuePair<string, Upgrades.Upgrade> entry in Upgrades.getDictionary()) {
            // do something with entry.Value or entry.Key
            entry.Value.costText.text = "" + (entry.Value.cost + entry.Value.costIncr * entry.Value.curr).ToString();
            entry.Value.currText.text = entry.Value.curr + "/" + entry.Value.max;
        }

        ////Bounciness
        //upgrade = Upgrades.getUpgrade("bounciness");
        //this.costBounciness.text = "" + (upgrade.cost + upgrade.costIncr * upgrade.curr).ToString();
        //this.currBounciness.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;

        ////length
        //upgrade = Upgrades.getUpgrade("ropelength");
        //this.costLength.text = "" + (upgrade.cost + upgrade.costIncr * upgrade.curr).ToString();
        //this.currLength.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;

        ////speed
        //this.upgradeMap.TryGetValue("ropespeed", out upgrade);
        //curr = PlayerPrefs.GetInt(upgrade.prefName);
        //this.costSpeed.text = "" + (upgrade.cost + upgrade.costIncr * curr).ToString();
        //this.currSpeed.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;

        ////hardhat
        //this.upgradeMap.TryGetValue("hardhat", out upgrade);
        //curr = PlayerPrefs.GetInt(upgrade.prefName);
        //this.costHardHat.text = "" + (upgrade.cost + upgrade.costIncr * curr).ToString();
        //this.currHardHat.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;

        ////hardhat
        //this.upgradeMap.TryGetValue("hookspeed", out upgrade);
        //curr = PlayerPrefs.GetInt(upgrade.prefName);
        //upgrade.costText.text = "" + (upgrade.cost + upgrade.costIncr * curr).ToString();
        //upgrade.currText.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;
    }

    /// <summary>
    /// Centers the buttons on the gameTypePanel both vertically and horizontally. Couldn't find an easier way to do this...
    /// </summary>
    private void spaceButtonsOnGameTypePanel() {
        List<RectTransform> list = new List<RectTransform>();
        foreach(Transform child in this.gameTypePanel.transform) {
            list.Add(child.GetComponent<RectTransform>());
        }

        RectTransform parentRect = this.gameTypePanel.GetComponent<RectTransform>();
        float height = parentRect.rect.height;
        float width = parentRect.rect.width;
        int num = list.Count + 1;
        float spaceY = (height - ((list[0].rect.height)* num)) / num;

        for(int i = 0; i < list.Count; i++) {
            list[i].position = new Vector2(parentRect.position.x, parentRect.position.y - parentRect.rect.height/2 + (list[i].rect.height + spaceY)*(i+1));
        }
    }

    void OnDestroy() {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }
}
