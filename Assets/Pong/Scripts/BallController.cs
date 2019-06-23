using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	//speed of the ball (Set difficulty here)
	public float speed = 10F;

    [SerializeField]
    float speedIncrease = 0.15f;

	//the initial direction of the ball
	private Vector2 spawnDir;

    //Add sounds
    public AudioClip bounceSound;
    private AudioSource source;

    //ball's components
    Rigidbody2D rig2D;
	// Use this for initialization
	void Start () {
		//setting balls Rigidbody 2D
		rig2D = this.gameObject.GetComponent<Rigidbody2D>();

		//generating random number based on possible initial directions
		int rand = Random.Range(1,4);

		//setting initial direction
		if(rand == 1){
			spawnDir = new Vector2(1,1);
		} else if(rand == 2){
			spawnDir = new Vector2(1,-1);
		} else if(rand == 3){
			spawnDir = new Vector2(-1,-1);
		} else if(rand == 4){
			spawnDir = new Vector2(-1,1);
		}

		//moving ball in initial direction and adding speed
		rig2D.velocity = (spawnDir*speed);

        source = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	//void Update () {
	//
	//}

	void OnCollisionEnter2D(Collision2D col) {

		//tag check
		if (col.gameObject.tag == "Enemy") {
			//calculate angle
			float y = launchAngle(transform.position,
			                    col.transform.position,
			                    col.collider.bounds.size.y);
			
			//set angle and speed
			Vector2 d = new Vector2(1, y).normalized;
			rig2D.velocity = d * speed * 1.5F;
        }

        if (col.gameObject.tag == "Player") {
			//calculate angle
			float y = launchAngle(transform.position,
			                    col.transform.position,
			                    col.collider.bounds.size.y);

			//set angle and speed
			Vector2 d = new Vector2(-1, y).normalized;
			rig2D.velocity = d * speed * 1.5F;
        }

        if (col.gameObject.tag == "Bound")
        {
            source.PlayOneShot(bounceSound, 1F);
        }

        speed += speedIncrease;
    }

    //calculates the angle the ball hits the paddle at
    float launchAngle(Vector2 ballPos, Vector2 paddlePos,
	                float paddleHeight) {
		return (ballPos.y - paddlePos.y) / paddleHeight;
	}
}
