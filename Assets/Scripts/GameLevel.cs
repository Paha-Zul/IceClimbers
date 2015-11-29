using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {
    public Text _text, scoreText, coinsText;
    public GameObject rightWall, leftWall, player, shard, scorePanel, canvas, spawns; //Our prefabs
    public float speed = 0.01f;
    public int wallHeight = 2;
    public float increasePerShard = 0.1f, initialSpawnTime = 2;

    private List<GameObject> walls = new List<GameObject>();
    private GameObject[] lastAddedWalls;
    private Player playerScript;
    private float horzExtent, vertExtent;
    private bool paused = false;
    private float nextTime = 0;

    private int score = 0;

	// Use this for initialization
	void Start () {
        this.playerScript = this.player.GetComponent<Player>();
        this.lastAddedWalls = new GameObject[2];

        this.horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        this.vertExtent = Camera.main.orthographicSize * Screen.height / Screen.width;

        for (int i = 0; i < vertExtent*2/wallHeight; i++)
        {
            walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, 0 - this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the left
            walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, 0 - this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the right
        }

        this.paused = false;
        Time.timeScale = 1;
        score = 0;
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
                wall.transform.Translate(Vector3.down * speed);

                if (wall.transform.position.y - wallHeight < -this.vertExtent)
                {
                    GameObject.Destroy(walls[i]);
                    walls.RemoveAt(i);
                }
            }

            //If the topmost walls are about to pass under the top of the screen, spawn more.
            if (this.lastAddedWalls[0].transform.position.y + wallHeight < this.vertExtent)
            {
                walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, this.vertExtent, 0), Quaternion.identity)); //Spawn on the left
                walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, this.vertExtent, 0), Quaternion.identity)); //Spawn on the right
            }
        }
	}

    public void SpawnShards()
    {
        if(Time.time > this.nextTime)
        {
            int numChilds = spawns.transform.childCount;
            Vector2 spawn = spawns.transform.GetChild((int)Random.Range(0f, numChilds - 0.1f)).transform.position;
            GameObject newShard = Instantiate(this.shard, spawn, Quaternion.identity) as GameObject;
            newShard.name = this.shard.name;
            newShard.GetComponent<Shard>().gameLevel = this;
            this.nextTime = Time.time + this.initialSpawnTime;
            this.initialSpawnTime -= this.increasePerShard;
        }
    }

    //Games over!
    public void GameOver()
    {
        this.paused = true;
        Time.timeScale = 0;

        //Get the current coins we have and add our score to them.
        int coins = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coins + score);

        //Move the game over screen to the center.
        RectTransform canvasTransform = this.canvas.GetComponent<RectTransform>();
        this.scorePanel.GetComponent<RectTransform>().position = new Vector2(canvasTransform.position.x, canvasTransform.position.y);

        this.scoreText.text = score.ToString();
        this.coinsText.text = "+"+(int)(score*1.3f);
        //Application.LoadLevel("MainMenu");
    }

    public void IncreaseScore(int value)
    {
        if (this.playerScript.isFalling()) return;
        score += value;
        if(_text != null)
            _text.text = score.ToString();
    }
}
