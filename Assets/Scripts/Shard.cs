using UnityEngine;
using System.Collections;

public class Shard : MonoBehaviour {
    private Rigidbody2D rigidBody;

    private float startTime;
    private bool started = false;
    private Vector3 originalPosition;
    public GameLevel gameLevel {get; set;}

	// Use this for initialization
	void Start () {
        //Get the rigibody, initially set to kinematic, set start time.
        this.originalPosition = this.transform.position;
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.rigidBody.isKinematic = true;
        this.startTime = Time.time + 1;
	}
	
	// Update is called once per frame
	void Update () {
	    if(!this.started && Time.time > this.startTime)
        {
            this.started = true;
            this.rigidBody.isKinematic = false;
            this.transform.position = this.originalPosition;
        }else if (!this.started)
        {
            shake();
        }

	}

    void shake()
    {
        this.transform.position = new Vector3(Random.Range(-0.05f, 0.05f) + this.originalPosition.x, this.originalPosition.y, this.originalPosition.z);
    }

    void OnBecameInvisible()
    {
        this.gameLevel.IncreaseScore(1);
        Destroy(this.gameObject);
    }
}
