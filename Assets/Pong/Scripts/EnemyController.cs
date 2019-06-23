using UnityEngine;
using System.Collections;

//THIS IS THE AI OF ENEMY
public class EnemyController : MonoBehaviour {

	//Speed of the enemy (Set difficulty here)
	public float speed = 1.75F;

	//the ball
	Transform ball;

	//the ball's rigidbody 2D
	Rigidbody2D ballRig2D;

    //add the time orb hit on wall
    public int hitCount;

    //Add sounds
    public AudioClip bounceSound;
    private AudioSource source;

    //bounds of enemy
    public float topBound = 4.5F;
	public float bottomBound = -4.5F;

    Animator anim;

	// Use this for initialization
	void Start () {
		//Continously Invokes Move every x seconds (values may differ)
		InvokeRepeating("Move", .02F, .02F);

        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        hitCount = 0;
    }

	// Movement for the paddle
	void Move () {

		//finding the ball
		if (ball == null){
            if (GameObject.FindGameObjectWithTag("Ball") == null) return;
			ball = GameObject.FindGameObjectWithTag("Ball").transform;
		}
        if (ball != null)
        {
            //setting the ball's rigidbody to a variable
            ballRig2D = ball.GetComponent<Rigidbody2D>();

            //checking x direction of the ball
            if (ballRig2D.velocity.x < 0)
            {
                //checking y direction of ball
                if (ball.position.y < this.transform.position.y - .3F)
                {
                    //move ball down if lower than paddle
                    transform.Translate(Vector3.down * speed * Time.deltaTime);
                }
                else if (ball.position.y > this.transform.position.y + .3F)
                {
                    //move ball up if higher than paddle
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                }

            }

            //set bounds of enemy
            if (transform.position.y > topBound)
            {
                transform.position = new Vector3(transform.position.x, topBound, 0);
            }
            else if (transform.position.y < bottomBound)
            {
                transform.position = new Vector3(transform.position.x, bottomBound, 0);
            }
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetTrigger("Shake");
        source.PlayOneShot(bounceSound, 1F);
        hitCount++;
    }
}
