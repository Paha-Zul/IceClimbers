using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
    public Text coinText;
    public GameObject menuRoot, mainPanel, startPanel, gameTypePanel, itemPanel, canvas;
    public Text currSpeed, currLength, currBounciness, costSpeed, costLength, costBounciness;

    [HideInInspector]
    public RectTransform mainPanelRectTransform, mainMenuPanelRectTransform, gameTypePanelRectTransform, itemPanelRectTransform, canvasRectTransform;

    public static MainMenu inst { get; private set; }

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
        string pref = null;
        float val;
        float increase = 0;

        //Set some variables based on what ww clicked.
        if(type == "ropespeed") {
            pref = "RopeSpeed";
            increase += 0.1f;
            amountSpent = 10;
        } else if(type == "ropelength") {
            pref = "RopeLength";
            increase += 0.1f;
            amountSpent = 10;
        } else if(type == "bounciness") {
            pref = "Bounciness";
            increase = 0.1f;
            amountSpent = 10;
        }
        
        //If we don't have enough coins, return false
        if(coins - amountSpent < 0)
            return false;

        //Increase our value, set it!
        val = PlayerPrefs.GetFloat(pref);
        val += increase;
        PlayerPrefs.SetFloat(pref, val);

        //Refresh the items/text
        PlayerPrefs.SetInt("Coins", coins - amountSpent);
        RefreshCoinAmount();
        this.loadItemData();

        return true;
    }

    public void loadItemData() {
        this.costBounciness.text = "10";
        this.costLength.text = "10";
        this.costSpeed.text = "10";

        this.currSpeed.text = (Defaults.RopeSpeed + PlayerPrefs.GetFloat("RopeSpeed")).ToString();
        this.currLength.text = (Defaults.RopeLength - PlayerPrefs.GetFloat("RopeLength")).ToString();
        this.currBounciness.text = (Defaults.Bounciness - PlayerPrefs.GetFloat("Bounciness")).ToString();
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
}
