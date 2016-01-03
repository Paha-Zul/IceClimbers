using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {
    public Sprite deadTexture;

    public bool left { get; set; }
    public float speed { get; set; }

    private bool dead = false;

    // Use this for initialization
    void Start() {
        if(!left) {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
            this.speed = -this.speed;
        }
    }

    // Update is called once per frame
    void Update() {
        if(!GameLevel.paused && !this.dead)
            this.transform.Translate(new Vector3(speed, 0, 0));
    }

    public void OnBecameInvisible() {
        GameLevel.dodgedBird();
        Destroy(this.gameObject);
    }

    public void Kill() {
        this.dead = true;
        this.GetComponent<Rigidbody2D>().isKinematic = false;
        this.GetComponent<SpriteRenderer>().sprite = deadTexture;
        this.GetComponent<Collider2D>().enabled = false;
    }
}
