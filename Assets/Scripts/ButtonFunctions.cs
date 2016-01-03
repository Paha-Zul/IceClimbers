using UnityEngine;
using System.Collections;
using GooglePlayGames;
using AppodealAds.Unity.Api;

public class ButtonFunctions : MonoBehaviour {
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play(string type) {
        if (type == "normal") {
            GameLevel.levelType = GameLevel.LevelType.Normal;
            if(PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.Events.IncrementEvent("CgkIzpi-qqMDEAIQBw", 1);
        }
        if (type == "angled") {
            GameLevel.levelType = GameLevel.LevelType.Angled;
            if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.Events.IncrementEvent("CgkIzpi-qqMDEAIQCA", 1);
        }
        if (type == "double") {
            GameLevel.levelType = GameLevel.LevelType.Double;
            if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.Events.IncrementEvent("CgkIzpi-qqMDEAIQCQ", 1);
        }
        Application.LoadLevel("Game");
    }

    public void Restart(){
        Application.LoadLevel("Game");
    }

    public void LoadMainMenu(){
        Application.LoadLevel("MainMenu");
    }

    /// <summary>
    /// Moves from the main menu to the game type panel
    /// </summary>
    public void ChooseGameType() {
        MainMenu.inst.mainMenuPanelRectTransform.position = MainMenu.inst.menuRoot.transform.position; //Move the main menu out of the way
        MainMenu.inst.gameTypePanelRectTransform.position = MainMenu.inst.mainPanelRectTransform.position; //Move the gameType panel in.
    }

    /// <summary>
    /// Moves from the main menu to the item panel.
    /// </summary>
    public void ChooseItems() {
        MainMenu.inst.loadItemDataInfoIntoItemStore();
        MainMenu.inst.mainMenuPanelRectTransform.position = MainMenu.inst.menuRoot.transform.position; //Move the main menu out of the way
        MainMenu.inst.itemPanelRectTransform.position = MainMenu.inst.mainPanelRectTransform.position; //Move the item panel in.
    }

    /// <summary>
    /// Moves from the game type panel to the main menu
    /// </summary>
    public void FromGameTypeToMainMenu() {
        MainMenu.inst.gameTypePanelRectTransform.position = MainMenu.inst.menuRoot.transform.position; //Move the gameType panel out of the way.
        MainMenu.inst.mainMenuPanelRectTransform.position = MainMenu.inst.mainPanelRectTransform.position; //Move the mainMenu panel in.
    }

    /// <summary>
    /// Moves from the item panel to the main menu
    /// </summary>
    public void FromItemPanelToMainMenu() {
        MainMenu.inst.itemPanelRectTransform.position = MainMenu.inst.menuRoot.transform.position;
        MainMenu.inst.mainMenuPanelRectTransform.position = MainMenu.inst.mainPanelRectTransform.position;
    }

    public void ToMainMenu() {
        //Move everything out.
        MainMenu.inst.itemPanelRectTransform.position = MainMenu.inst.menuRoot.transform.position;
        MainMenu.inst.gameTypePanelRectTransform.position = MainMenu.inst.menuRoot.transform.position;

        //Move the mainMenu in.
        MainMenu.inst.mainMenuPanelRectTransform.position = MainMenu.inst.mainPanelRectTransform.position;
    }

    public void OpenLeaderboards() {
        if(PlayGamesPlatform.Instance.IsAuthenticated())
            Social.ShowLeaderboardUI();
    }

    public void PlayRewardVideo() {
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }

    public void Purchase(string type) {
        if(MainMenu.inst.MakePurchase(type)) {
            this.GetComponent<AudioSource>().Play();
        }
    }
}
