using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {
    public enum LevelType { Normal, Angled, Double }

    public Text _text, scoreText, coinsText;
    public GameObject rightWall, leftWall, player, shard, bird, scorePanel, canvas, spawns, underSpawns, leftBirdSpawn, rightBirdSpawn; //Our prefabs
    public GameObject birdWarning;
    public float wallHeight;

    public static LevelType levelType;
    public static bool paused { get; private set; }

    private static Text text;

    private List<GameObject> walls = new List<GameObject>();
    private GameObject[] lastAddedWalls;
    private static Player playerScript;
    private float horzExtent, vertExtent, nextIncrease, nextSpawn, coinMulti, spawnDelay;
    private float ropeSpeed, ropeLength, bounciness;

    private int shardsSpawnedSoFar = 0, birdsSpawnedSoFar;
    private float nextBirdSpawn, birdSpawnSpeed, birdSpawnChance = 0.5f;

    private static int overallScore = 0, shardsDodged = 0, birdsDodged = 0;

    // Use this for initialization
    void Start () {
        Vector3 bounds = rightWall.GetComponent<SpriteRenderer>().bounds.size;
        wallHeight = bounds.y;

        GameLevel.text = _text;

        GameLevel.playerScript = this.player.GetComponent<Player>();
        this.lastAddedWalls = new GameObject[2];

        this.horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        this.vertExtent = Camera.main.orthographicSize * Screen.height / Screen.width;

        for (int i = 0; i < (vertExtent*2)/wallHeight + 2; i++)
        {
            walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, -this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the left
            walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, -this.vertExtent + (wallHeight * i), 0), Quaternion.identity)); //Spawn on the right
        }

        paused = false;
        Time.timeScale = 1;
        overallScore = 0;

        if(levelType == LevelType.Normal) coinMulti = 0.5f;
        else if(levelType == LevelType.Angled) coinMulti = 0.7f;
        else if(levelType == LevelType.Double) coinMulti = 0.9f;

        this.spawnDelay = Defaults.InitialShardSpawnSpeed;
        this.birdSpawnSpeed = Defaults.InitialBirdSpawnSpeed;
        this.nextBirdSpawn = Time.time + Defaults.InitialBirdSpawnSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        if (!paused)
        {
            this.SpawnShards();
            this.spawnBirds();

            //Moves the walls down and removes them if they are below the screen.
            for (int i = 0; i < walls.Count; i++){
                GameObject wall = walls[i];
                wall.transform.Translate(Vector3.down * Defaults.WallSpeed);

                if (wall.transform.position.y + wallHeight < -this.vertExtent){
                    GameObject.Destroy(walls[i]);
                    walls.RemoveAt(i);
                }
            }

            //If the topmost walls are about to pass under the top of the screen, spawn more.
            if (this.lastAddedWalls[0].transform.position.y <= this.vertExtent){
                walls.Add(this.lastAddedWalls[0] = (GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, this.vertExtent + wallHeight, 0), Quaternion.identity)); //Spawn on the left
                walls.Add(this.lastAddedWalls[1] = (GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, this.vertExtent + wallHeight, 0), Quaternion.identity)); //Spawn on the right
            }

            this.IncreaseShardSpawn();
        }
	}

    /// <summary>
    /// Increases the spawn speed of the shard when the correct time is met.
    /// </summary>
    public void IncreaseShardSpawn() {
        if(Time.time > this.nextIncrease) {
            this.spawnDelay -= Defaults.ShardSpawnTimeIncreaseAmount;
            this.nextIncrease = Time.time + Defaults.ShardSpawnIncreaseIncreaseInterval;
        }
    }

    /// <summary>
    /// Spawns shards to fall when the time is met.
    /// </summary>
    public void SpawnShards()
    {
        if(Time.time > this.nextSpawn)
        {
            this.nextSpawn = Time.time + this.spawnDelay;

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
            shard.num = this.shardsSpawnedSoFar++;
            if(shard.transform.position.y <= 0) shard.rigidBody.gravityScale = -1;

            //If we are above the bird spawning limit and we are at the interval to increase bird spawn speed...
            if(shardsDodged > Defaults.NumShardsPerBirdSpawnIncrease && shardsDodged % Defaults.NumShardsPerBirdSpawnIncrease == 0){
                this.birdSpawnSpeed -= Defaults.BirdSpawnIncreaseSpeed;
            }
        }
    }

    public void spawnBirds() {
        if(Time.time > this.nextBirdSpawn && this.shardsSpawnedSoFar >= Defaults.ShardsDodgedBeforeBirdSpawn) {
            this.nextBirdSpawn = Time.time + this.birdSpawnSpeed;

            if(Random.Range(0f, 1f) <= 0.5f) return;

            GameObject birdSpawn;
            bool left = false;
            if(Random.Range(0f, 1f) > 0.5) {
                birdSpawn = leftBirdSpawn;
                left = true;
            } else {
                birdSpawn = rightBirdSpawn;
            }

            StartCoroutine(SpawnBirdDelayed(birdSpawn, left));
        }
    }

    //Games over!
    public void GameOver()
    {
        paused = true;
        Time.timeScale = 0;

        //Get the current coins we have and add our score to them.
        int coins = PlayerPrefs.GetInt("Coins") ;
        int newCoins = (int)(overallScore * this.coinMulti);
        PlayerPrefs.SetInt("Coins", coins + newCoins);

        //Move the game over screen to the center.
        RectTransform canvasTransform = this.canvas.GetComponent<RectTransform>();
        this.scorePanel.GetComponent<RectTransform>().position = new Vector2(canvasTransform.position.x, canvasTransform.position.y);

        //Set the text for score and coins.
        this.scoreText.text = overallScore.ToString();
        this.coinsText.text = newCoins.ToString();
    }

    /// <summary>
    /// Call when the player dodged a shard.
    /// </summary>
    public static void dodgedShard()
    {
        if (GameLevel.playerScript.isFalling()) return;
        GameLevel.shardsDodged++;
        GameLevel.overallScore++;
        if (text != null)
            text.text = overallScore.ToString();
    }

    /// <summary>
    /// Call when the player dodged a bird.
    /// </summary>
    public static void dodgedBird()
    {
        if (GameLevel.playerScript.isFalling()) return;
        GameLevel.birdsDodged++;
        GameLevel.overallScore++;
        if (text != null)
            text.text = overallScore.ToString();
    }

    /// <summary>
    /// Spawns a bird on a delay. The delay includes a blinking icon.
    /// </summary>
    /// <param name="birdSpawn"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    IEnumerator SpawnBirdDelayed(GameObject birdSpawn, bool left)
    {
        //Get a child from the spawn group
        birdSpawn = birdSpawn.transform.GetChild((int)Random.Range(0, birdSpawn.transform.childCount - 0.1f)).gameObject;

        //Get the render for size stuff.
        SpriteRenderer renderer = this.birdWarning.GetComponent<SpriteRenderer>();

        //Set the X value.
        float x = horzExtent - renderer.bounds.extents.x;
        if (left) x = -horzExtent + renderer.bounds.extents.x;

        //Find the position to spawn the warning and flip if necessary.
        Vector3 spawnPos = new Vector3(x, birdSpawn.transform.position.y, birdSpawn.transform.position.z);
        GameObject birdWarningObj = Instantiate(this.birdWarning, spawnPos, Quaternion.identity) as GameObject;
        if (left)
            birdWarningObj.transform.localScale = new Vector3(-birdWarningObj.transform.localScale.x, 
                birdWarningObj.transform.localScale.y, birdWarningObj.transform.localScale.z);

        //Blink the warning!
        bool on = false;
        for(int i = 0; i < 6; i++){
            birdWarningObj.SetActive(on = !on);
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(birdWarningObj); //Destroy the warning object.

        //Spawn the bird and fly!
        GameObject bird = Instantiate(this.bird, birdSpawn.transform.position, Quaternion.identity) as GameObject;
        bird.name = this.bird.name;
        Bird birdScript = bird.GetComponent<Bird>();
        birdScript.speed = 0.05f;
        birdScript.left = left;
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }
}
