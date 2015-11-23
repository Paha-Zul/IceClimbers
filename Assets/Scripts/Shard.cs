using UnityEngine;
using System.Collections;

public class Shard : MonoBehaviour {
    private Rigidbody2D rigidBody;

    private float startTime;
    private bool started = false;

	// Use this for initialization
	void Start () {
        //Get the rigibody, initially set to kinematic, set start time.
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
        }

        //Need to erase this thing when if goes off screen.

	}

    void OnBecameInvisible()
    {
        GameLevel.IncreaseScore(1);
        Destroy(this.gameObject);
    }
}
