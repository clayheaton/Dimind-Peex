using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Camera gameCamera;
	private LevelManager levelScript;
	public GameObject player;

	private string level = "level_01";
	// Use this for initialization
	void Awake () {
		player = Instantiate(Resources.Load("player") as GameObject);
		gameCamera = Instantiate(Resources.Load("cam",typeof(Camera)) as Camera);
		//Instantiate(player);
		//Instantiate(gameCamera);
		CameraController cc = gameCamera.GetComponent<CameraController>();
		cc.player   = player;
		levelScript = GetComponent<LevelManager>();
		levelScript.player = player;
		levelScript.gameCamera = gameCamera;
		InitGame();
	}

	void InitGame(){
		levelScript.SetupLevel(level);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
