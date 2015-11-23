using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Swarm.init(20527, "9f3c6aab4ea73f215c004a982c75913a");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play()
    {
        Application.LoadLevel("Game");
    }
}
