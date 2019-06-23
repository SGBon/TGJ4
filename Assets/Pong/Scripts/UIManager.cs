using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	GameObject[] pauseObjects, finishObjects;
	public BoundController rightBound;
	public BoundController leftBound;
    public EnemyController enemy;
	public bool isFinished;
	public bool playerWon, enemyWon;

	// Use this for initialization
	void Start () {
		pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
		finishObjects = GameObject.FindGameObjectsWithTag("ShowOnFinish");
		hideFinished();
	}
	
	// Update is called once per frame
	void Update () {

		//if(rightBound.enemyScore >= 3 && !isFinished){
		//	isFinished = true;
		//	enemyWon = true;
		//	playerWon = false;
		//} else if (leftBound.playerScore >= 3  && !isFinished){
		//	isFinished = true;
		//	enemyWon = false;
		//	playerWon = true;
		//}

        //end the game if the orb hit the wall 40 times
        if(enemy.hitCount >= 5 && !isFinished)
        {
            isFinished = true;
        }

		if(isFinished){
            Time.timeScale = 0;
            showFinished();
		}

		
		//uses the p button to pause and unpause the game
		if(Input.GetKeyDown(KeyCode.P) && !isFinished)
		{
			if(Time.timeScale == 1)
			{
				Time.timeScale = 0;
				showPaused();
			} else if (Time.timeScale == 0){
				Time.timeScale = 1;
				hidePaused();
			}
		}


		if(Time.timeScale == 0  && !isFinished){
			foreach(GameObject g in pauseObjects){
				if(g.name == "PauseText")
					g.SetActive(true);
			}
		} else {
			foreach(GameObject g in pauseObjects){
				if(g.name == "PauseText")
					g.SetActive(false);
			}
		}
	}


    //Reloads the Level
    [System.Obsolete]
    public void Reload(){
		Application.LoadLevel(Application.loadedLevel);
	}
	
	//controls the pausing of the scene
	public void pauseControl(){
		if(Time.timeScale == 1)
		{
			Time.timeScale = 0;
			showPaused();
		} else if (Time.timeScale == 0){
			Time.timeScale = 1;
			hidePaused();
		}
	}
	
	//shows objects with ShowOnPause tag
	public void showPaused(){
		foreach(GameObject g in pauseObjects){
			g.SetActive(true);
		}
	}

	//hides objects with ShowOnPause tag
	public void hidePaused(){
		foreach(GameObject g in pauseObjects){
			g.SetActive(false);
		}
	}

	//shows objects with ShowOnFinish tag
	public void showFinished(){
		foreach(GameObject g in finishObjects){
			g.SetActive(true);
		}
	}
	
	//hides objects with ShowOnFinish tag
	public void hideFinished(){
		foreach(GameObject g in finishObjects){
			g.SetActive(false);
		}
	}

    //loads (scene) level
    [System.Obsolete]
    public void LoadLevel(string level){
		Application.LoadLevel(level);
	}
}
