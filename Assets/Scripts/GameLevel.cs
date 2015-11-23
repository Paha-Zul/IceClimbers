using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {
    public Text _text;
    public GameObject rightWall, leftWall, player, shard; //Our prefabs
    public List<GameObject> spawnPoints;
    public float speed = 0.01f;
    public int wallHeight = 2;
    public float increasePerShard = 0.1f, initialSpawnTime = 2;

    public static Text text;

    private List<GameObject> walls = new List<GameObject>();
    private GameObject[] lastAddedWalls;
    private float horzExtent, vertExtent;
    private bool paused = false;
    private float nextTime = 0;

    private static int score = 0;

	// Use this for initialization
	void Start () {
        this.lastAddedWalls = new GameObject[2];
        text = _text;

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
            float spawnX = spawnPoints[(int)Random.Range(0f, spawnPoints.Count - 0.1f)].transform.position.x;
            float spawnY = vertExtent / 2;
            GameObject newShard = Instantiate(this.shard, new Vector3(spawnX, spawnY), Quaternion.identity) as GameObject;
            newShard.name = this.shard.name;
            this.nextTime = Time.time + this.initialSpawnTime;
            this.initialSpawnTime -= this.increasePerShard;
        }
    }

    public void GameOver()
    {
        //SwarmLeaderboard.submitScore(20183, 10);
        //this.paused = true;
        Application.LoadLevel("MainMenu");
        //SwarmLeaderboard.showLeaderboard(20183);
    }

    public static void IncreaseScore(int value)
    {
        score += value;
        if(text != null)
            text.text = score.ToString();
    }
}
