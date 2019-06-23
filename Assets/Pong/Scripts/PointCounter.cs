using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointCounter : MonoBehaviour {

	public GameObject rightBound;
	public GameObject leftBound;
    public GameObject enemy;
	Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
        text.text = enemy.GetComponent<EnemyController>().hitCount.ToString();

    }
	
	// Update is called once per frame
	void Update () {
        text.text = enemy.GetComponent<EnemyController>().hitCount.ToString();
    }
}
