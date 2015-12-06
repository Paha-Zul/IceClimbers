using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {
    public enum LevelType { Normal, Angled, Double }

    public Text _text, scoreText, coinsText;
    public GameObject rightWall, leftWall, player, shard, scorePanel, canvas, spawns, underSpawns; //Our prefabs
    public float wallHeight;

    public static LevelType levelType;

    private List<GameObject> walls = new List<GameObject>();
    private GameObject[] lastAddedWalls;
    private Player playerScript;
    private float horzExtent, vertExtent, nextIncrease, nextSpawn, bonusRopeSpeed, bonusRopeLength, coinMulti, spawnDelay;
    private bool paused = false;

    private int score = 0;
    private int shardCount = 0;

	// Use this for initialization
	void Start () {
        Vector3 bounds = rightWall.GetComponent<SpriteRenderer>().bounds.size;
        wallHeight = bounds.y;

        this.playerScript = this.player.GetComponent<Player>();
        this.lastAddedWalls = new GameObject[2];

        this.horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        this.vertExtent = Camera.main.orthographicSize * Screen.height / Screen.width;

        for (int i = 0; i < (vertExtent*2)/wallHeight + 2; i++)
        {
            walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, -this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the left
            walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, -this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the right
        }

        this.paused = false;
        Time.timeScale = 1;
        score = 0;

        if(levelType == LevelType.Normal) coinMulti = 0.5f;
        else if(levelType == LevelType.Angled) coinMulti = 0.7f;
        else if(levelType == LevelType.Double) coinMulti = 0.9f;

        this.spawnDelay = Defaults.SpawnDelay;
    }
	
	// Update is called once per frame
	void Update () {
        if (!this.paused)
        {
            this.SpawnShards();

            //Moves the walls down and removes them if they are below the screen.
            for (int i = 0; i < walls.Count; i++)
            {
                GameObject wall = walls[i];
                wall.transform.Translate(Vector3.down * Defaults.WallSpeed);

                if (wall.transform.position.y + wallHeight < -this.vertExtent)
                {
                    GameObject.Destroy(walls[i]);
                    walls.RemoveAt(i);
                }
            }

            //If the topmost walls are about to pass under the top of the screen, spawn more.
            if (this.lastAddedWalls[0].transform.position.y <= this.vertExtent)
            {
                walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, this.vertExtent + wallHeight, 0), Quaternion.identity)); //Spawn on the left
                walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, this.vertExtent + wallHeight, 0), Quaternion.identity)); //Spawn on the right
            }

            this.IncreaseSpawn();
        }
	}

    public void IncreaseSpawn() {
        if(Time.time > this.nextIncrease) {
            this.spawnDelay -= Defaults.SpawnTimeIncreaseAmount;
            this.nextIncrease = Time.time + Defaults.SpawnIncreaseIncreaseInterval;
        }
    }

    public void SpawnShards()
    {
        if(Time.time > this.nextSpawn)
        {
            GameObject spawnToUse;
            if(levelType != LevelType.Double)
                spawnToUse = this.spawns;
            else
                spawnToUse = Random.Range(0f, 1f) >= 0.5f ? underSpawns : spawns;

            //Spawn a shard!
            int numChilds = spawnToUse.transform.childCount;
            Vector2 spawn = spawnToUse.transform.GetChild((int)Random.Range(0f, numChilds - 0.1f)).transform.position;
            GameObject newShard = Instantiate(this.shard, spawn, Quaternion.identity) as GameObject;

            //Set the name (and rotation if we are playing rotated game type)
            newShard.name = this.shard.name;
            if(levelType == LevelType.Angled) newShard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-40, 40));

            //Set some stuff.
            Shard shard = newShard.GetComponent<Shard>();
            shard.gameLevel = this;
            shard.num = this.shardCount++;
            if(shard.transform.position.y <= 0) shard.rigidBody.gravityScale = -1;

            this.nextSpawn = Time.time + this.spawnDelay;
        }
    }

    //Games over!
    public void GameOver()
    {
        this.paused = true;
        Time.timeScale = 0;

        //Get the current coins we have and add our score to them.
        int coins = PlayerPrefs.GetInt("Coins") ;
        int newCoins = (int)(score * this.coinMulti);
        PlayerPrefs.SetInt("Coins", coins + newCoins);

        //Move the game over screen to the center.
        RectTransform canvasTransform = this.canvas.GetComponent<RectTransform>();
        this.scorePanel.GetComponent<RectTransform>().position = new Vector2(canvasTransform.position.x, canvasTransform.position.y);

        //Set the text for score and coins.
        this.scoreText.text = score.ToString();
        this.coinsText.text = "+" + newCoins;
    }

    public void IncreaseScore(int value)
    {
        if (this.playerScript.isFalling()) return;
        score += value;
        if(_text != null)
            _text.text = score.ToString();
    }
}
