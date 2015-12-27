using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {
    public Player player;
    public float angle, travelSpeed, windSpeed;

    [HideInInspector]
    public Rigidbody2D rigidBody;
    private float dirX, dirY;

	// Use this for initialization
	void Start () {
        this.rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        this.dirX = (Mathf.Cos(angle)*Mathf.Rad2Deg) * travelSpeed;
        this.dirY = (Mathf.Sin(angle)*Mathf.Rad2Deg) * travelSpeed;

        if(dirX < 0) this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

        this.GetComponent<Rigidbody2D>().velocity = new Vector3(dirX, dirY, 0);
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.GetComponent<CircleCollider2D>());
        Physics2D.IgnoreLayerCollision(8, 8); 
    }
	
    void OnCollisionEnter2D(Collision2D coll)
    {
        this.player.ConnectToHook(this);
        this.transform.parent = coll.transform;
        this.GetComponent<Rigidbody2D>().isKinematic = true;
        this.GetComponent<AudioSource>().Play();
    }

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
