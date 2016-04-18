using UnityEngine;
using System.Collections;

public class ScoreScreen : MonoBehaviour {
	
	public int score = 0;
	float minShowTime = 150;
	public float showTime = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(showTime > 0) {
			showTime -= Time.deltaTime * 100;
			Debug.Log(showTime);
		}
	}
	
	public void showScoreScreen(string type) {
		Player player = GameObject.Find("player").GetComponent<Player>();
		LevelGenerator levelGen = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
		int tempScore = levelGen.level * player.gold;
		showTime = minShowTime;
		
		GameObject.Find("TussenScherm").GetComponent<GUITexture>().enabled = true;
		
		if(type == "ScoreScreen") {
			GameObject[] objects = GameObject.FindGameObjectsWithTag("ScoreScreen");
			foreach(GameObject o in objects) {
				if(o.GetComponent<GUITexture>()) {
					o.GetComponent<GUITexture>().enabled = true;
				}
				if(o.GetComponent<GUIText>()) {
					o.GetComponent<GUIText>().enabled = true;
				}
				if(o.name == "GoldCounter") {
					o.GetComponent<GUIText>().text = player.gold.ToString();
				}
				if(o.name == "LevelCounter") {
					o.GetComponent<GUIText>().text = levelGen.level.ToString();
				}
				if(o.name == "ScoreCounter") {
					o.GetComponent<GUIText>().text = tempScore.ToString();
				}
				if(o.name == "PrevScoreCounter") {
					o.GetComponent<GUIText>().text = score.ToString();
				}
				if(o.name == "RealScoreCounter") {
					o.GetComponent<GUIText>().text = (score + tempScore).ToString();
				}
				if(o.name == "TotalScore") {
					o.GetComponent<GUIText>().text = (score + tempScore).ToString();
				}
			}
			score = score + tempScore;
		}
		
		if(type == "GameOverScreen") {
			GameObject[] objects = GameObject.FindGameObjectsWithTag("GameOverScreen");
			foreach(GameObject o in objects) {
				if(o.GetComponent<GUITexture>()) {
					o.GetComponent<GUITexture>().enabled = true;
				}
				if(o.GetComponent<GUIText>()) {
					o.GetComponent<GUIText>().enabled = true;
				}
				if(o.name == "TotalScore") {
					o.GetComponent<GUIText>().text = score.ToString();
				}
				if(o.name == "GameOverText") {
					o.GetComponent<GUIText>().text = "You've reached level " + levelGen.level;
				}
				
			}
		}
	}
	
	public void hideScoreScreen() {
		GameObject.Find("TussenScherm").GetComponent<GUITexture>().enabled = false;
		
		GameObject[] objects = GameObject.FindGameObjectsWithTag("ScoreScreen");
		foreach(GameObject o in objects) {
			if(o.GetComponent<GUITexture>()) {
				o.GetComponent<GUITexture>().enabled = false;
			}
			if(o.GetComponent<GUIText>()) {
				o.GetComponent<GUIText>().enabled = false;
			}
		}
	
		
		objects = GameObject.FindGameObjectsWithTag("GameOverScreen");
		foreach(GameObject o in objects) {
			if(o.GetComponent<GUITexture>()) {
				o.GetComponent<GUITexture>().enabled = false;
			}
			if(o.GetComponent<GUIText>()) {
				o.GetComponent<GUIText>().enabled = false;
			}
		}
	}
}
