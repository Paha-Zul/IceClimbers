using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public GameObject hook;
    public GameLevel levelScript;
    public GameObject ropePrefab;
    public float ropeLength=2, ropeSpeed=0.1f;

    private bool aTap;
    private Vector3 touchPos;
    private float distToHook;
    private DistanceJoint2D joint;
    private Hook connectedHook;
    private float touchDownTime;
    private float holdDelay = 0.2f;
    private GameObject rope;
    private MeshRenderer ropeRenderer;

	// Use this for initialization
	void Start () {
        this.joint = GetComponent<DistanceJoint2D>();
        this.rope = Instantiate(this.ropePrefab);
        this.ropeRenderer = this.rope.transform.GetChild(0).GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        this.aTap = false;

        //If not Iphone or Android, use mouse button.
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
        {
            // use the input stuff
            this.aTap = Input.GetMouseButtonDown(0);
            this.touchPos = Input.mousePosition;
        }
        //Otherwise, use touch events.
        else
        {
            if (Input.touchCount > 0) //For future reference, if we try to get GetTouch(0) with a touch count of 0, it will mess up the game badly (weird things happen).
            {
                //Adnroid and Iphone
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    this.touchDownTime = Time.time;
                if (Input.GetTouch(0).phase == TouchPhase.Ended && Time.time <= this.touchDownTime + holdDelay) //If we end the touch within the delay period, we have 'tapped'
                    this.aTap = true;
            }

            this.touchPos = Input.mousePosition;
        }

        if (this.aTap)
            this.OnScreenTap();

        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        if (pos.y < 0)
        {
            levelScript.GameOver();
            Time.timeScale = 0;
        }

        if (this.connectedHook != null && this.joint != null)
        {
            ReelInHook();
            MakeRope();
        }
    }

    //When the screen is tapped.
    void OnScreenTap()
    {
        //Spawn the hook and get the hookComp
        GameObject newHook = Instantiate(hook, this.transform.position, Quaternion.identity) as GameObject;
        Hook newHookComp = newHook.GetComponent<Hook>();

        Vector3 playerPos = Camera.main.WorldToScreenPoint(this.transform.position); //Player screen position

        float angle = Mathf.Atan2(touchPos.y - playerPos.y, touchPos.x - playerPos.x); //Angle to mouse

        //Set some stuff.
        newHookComp.angle = angle;
        newHookComp.player = this;
    }

    //Connects to a hook.
    public void ConnectToHook(Hook hook)
    {
        //If the joint was destroyed, that means we are falling.
        if (this.joint != null)
        {
            this.connectedHook = hook;
            this.distToHook = Vector3.Distance(this.transform.position, hook.transform.position);
            this.joint.distance = this.distToHook;
            this.joint.connectedBody = hook.GetComponent<Rigidbody2D>();
        }
    }

    //Reels in the hook.
    void ReelInHook()
    {
        if (this.joint.distance > this.ropeLength)
        {
            this.joint.distance -= this.ropeSpeed;
            if (this.joint.distance <= this.ropeLength) this.joint.distance = this.ropeLength;
        }
    }

    void MakeRope()
    {
        GameObject r = this.rope;

        //Set the position, the scale (to the wall), and the angle of the rope.
        r.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        r.transform.localScale = new Vector3(r.transform.localScale.x, this.joint.distance, r.transform.localScale.z);
        float angle = Mathf.Atan2(this.joint.connectedBody.transform.position.y - r.transform.position.y, this.joint.connectedBody.transform.position.x - r.transform.position.x); //Angle to mouse
        r.transform.rotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg - 90);


        //Here we set the material tile scaling to match the rope length.
        Vector2 scale = ropeRenderer.material.mainTextureScale;
        scale.y = r.transform.localScale.y;
        ropeRenderer.material.mainTextureScale = scale;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.name == "Shard")
        {
            DistanceJoint2D joint = this.GetComponent<DistanceJoint2D>();
            Destroy(joint);
            this.connectedHook = null;
            //joint.connectedBody = null;
            //joint.distance = 9999999;
        }
    }
}
