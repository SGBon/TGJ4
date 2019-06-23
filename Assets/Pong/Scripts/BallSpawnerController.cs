using UnityEngine;
using System.Collections;

public class BallSpawnerController : MonoBehaviour {

    public GameObject ball;

    [SerializeField]
    Transform player;

    [SerializeField]
    float spawnBallDelay;

    Coroutine spawnBall;

    // Use this for initialization
    void Start() {
        //GameObject ballClone;
        //ballClone = Instantiate(ball, this.transform.position, this.transform.rotation) as GameObject;
        //ballClone.transform.SetParent(this.transform);
    }

    // Update is called once per frame
    void Update() {
        if (transform.childCount == 0) {
            if (spawnBall == null)
            {
                spawnBall = StartCoroutine(SpawnBall());
            }
        }
    }

    IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(spawnBallDelay);
        GameObject ballClone;
        ballClone = Instantiate(ball, player.position, this.transform.rotation) as GameObject;
        ballClone.transform.SetParent(this.transform);
        spawnBall = null;
    }
}
