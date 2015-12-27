using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
    public Text coinText;
    public GameObject menuRoot, mainPanel, startPanel, gameTypePanel, itemPanel, canvas;
    public Text currSpeed, currLength, currBounciness, currHardHat, costSpeed, costLength, costBounciness, costHardHat;

    [HideInInspector]
    public RectTransform mainPanelRectTransform, mainMenuPanelRectTransform, gameTypePanelRectTransform, itemPanelRectTransform, canvasRectTransform;

    public static MainMenu inst { get; private set; }

    private Dictionary<string, Upgrade> upgradeMap = new Dictionary<string, Upgrade>();

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
    }

    private void quickReset(){
        PlayerPrefs.SetInt(Defaults.RopeLengthPrefString, 0);
        PlayerPrefs.SetInt(Defaults.RopeSpeedPrefString, 0);
        PlayerPrefs.SetInt(Defaults.BouncinessPrefString, 0);
        PlayerPrefs.SetInt(Defaults.CoinPrefString, 1000);
    }

    private void populateUpgrades() {
        int speed = PlayerPrefs.GetInt(Defaults.RopeLengthPrefString);
        int length = PlayerPrefs.GetInt(Defaults.RopeSpeedPrefString);
        int bounciness = PlayerPrefs.GetInt(Defaults.BouncinessPrefString);
        int hardhat = PlayerPrefs.GetInt("HardHat");
        int coins = PlayerPrefs.GetInt(Defaults.CoinPrefString);

        upgradeMap.Add("ropespeed", new Upgrade("RopeSpeed", 10, 1, 10, costSpeed, currSpeed));
        upgradeMap.Add("ropelength", new Upgrade("RopeLength", 10, 1, 20, costLength, currLength));
        upgradeMap.Add("bounciness", new Upgrade("Bounciness", 10, 1, 10, costBounciness, currBounciness));
        upgradeMap.Add("hardhat", new Upgrade("HardHat", 50, 10, 1, costHardHat, currHardHat));
    }

    /// <summary>
    /// Validates the preferences. Maybe a temporary thing?
    /// </summary>
    private void validatePrefLimits()
    {
        int ropeLengthUps = PlayerPrefs.GetInt(Defaults.RopeLengthPrefString);
        int ropeSpeedUps = PlayerPrefs.GetInt(Defaults.RopeSpeedPrefString);
        int bouncinessUps = PlayerPrefs.GetInt(Defaults.BouncinessPrefString);

        if (ropeLengthUps > Defaults.MaxRopeLengthUpgrades) ropeLengthUps = Defaults.MaxRopeLengthUpgrades;
        if (ropeSpeedUps > Defaults.maxRopeSpeedUpgrades) ropeSpeedUps = Defaults.maxRopeSpeedUpgrades;
        if (bouncinessUps > Defaults.maxBouncinessUpgrade) bouncinessUps = Defaults.maxBouncinessUpgrade;

        PlayerPrefs.SetInt(Defaults.RopeLengthPrefString, ropeLengthUps);
        PlayerPrefs.SetInt(Defaults.RopeSpeedPrefString, ropeSpeedUps);
        PlayerPrefs.SetInt(Defaults.BouncinessPrefString, bouncinessUps);
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
        int curr = 0;
        int increase = 0;

        Upgrade upgrade;
        this.upgradeMap.TryGetValue(type, out upgrade);
        curr = PlayerPrefs.GetInt(upgrade.prefName);
        increase = 1;
        amountSpent = upgrade.cost + upgrade.costIncr * curr;

        ////Set some variables based on what ww clicked.
        //if(type == "ropespeed") {
        //    pref = "RopeSpeed";
        //    increase = 1;
        //    amountSpent = Defaults.RopeSpeedCost;
        //} else if(type == "ropelength") {
        //    pref = "RopeLength";
        //    increase = 1;
        //    amountSpent = Defaults.RopeLengthCost;
        //} else if(type == "bounciness") {
        //    pref = "Bounciness";
        //    increase = 1;
        //    amountSpent = Defaults.BouncinessCost;
        //}else if (type == "hardhat"){
        //    pref = "HardHat";
        //    increase = 1;
        //    amountSpent = Defaults.HardHatCost;
        //}

        //If we don't have enough coins, return false
        if (coins - amountSpent < 0)
            return false;

        //Increase our value, set it!
        int val = PlayerPrefs.GetInt(upgrade.prefName);
        int limit = upgrade.max;
        //if (pref == "RopeSpeed") limit = Defaults.maxRopeSpeedUpgrades;
        //else if (pref == "RopeLength") limit = Defaults.MaxRopeLengthUpgrades;
        //else if (pref == "Bounciness") limit = Defaults.maxBouncinessUpgrade;
        //else if (pref == "HardHat") limit = Defaults.maxHardHatUpgrade;

        //If we are going past the limit, return false.
        if (val + increase > limit)
            return false;

        val += increase;
        PlayerPrefs.SetInt(upgrade.prefName, val);

        //Refresh the items/text
        PlayerPrefs.SetInt("Coins", coins - amountSpent);
        this.RefreshCoinAmount();
        this.refreshItem(type);
        //this.loadItemData();

        return true;
    }

    public void refreshItem(string pref) {
        Upgrade upgrade;
        this.upgradeMap.TryGetValue(pref, out upgrade);
        upgrade.costText.text = "" + upgrade.cost + upgrade.costIncr * upgrade.costIncr;
        upgrade.currText.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;
    }

    public void loadItemData() {
        Upgrade upgrade;
        this.upgradeMap.TryGetValue("bounciness", out upgrade);
        this.costBounciness.text = "" + upgrade.cost + upgrade.costIncr * upgrade.costIncr;
        this.currBounciness.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;

        this.upgradeMap.TryGetValue("ropelength", out upgrade);
        this.costLength.text = "" + upgrade.cost + upgrade.costIncr * upgrade.costIncr;
        this.currLength.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + Defaults.MaxRopeLengthUpgrades;

        this.upgradeMap.TryGetValue("ropespeed", out upgrade);
        this.costSpeed.text = "" + upgrade.cost + upgrade.costIncr * upgrade.costIncr;
        this.currSpeed.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + Defaults.maxRopeSpeedUpgrades;

        this.upgradeMap.TryGetValue("hardhat", out upgrade);
        this.costHardHat.text = "" + upgrade.cost + upgrade.costIncr * upgrade.costIncr;
        this.currHardHat.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + Defaults.maxHardHatUpgrade;
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

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }

    public struct Upgrade {
        public string prefName;
        public int cost, costIncr, max;
        public Text costText, currText;

        public Upgrade(string prefName, int cost, int costIncr, int max, Text costText, Text currText) {
            this.prefName = prefName;
            this.cost = cost;
            this.costIncr = costIncr;
            this.max = max;
            this.costText = costText;
            this.currText = currText;
        }
    }
}
