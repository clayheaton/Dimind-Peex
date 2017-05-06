using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour {
	public Camera gameCamera;
	private LevelManager levelScript;
	public GameObject player;
	public bool useRandomSeed;
	public int randomSeed;
	public string level = "level_01";
	public string medialLayerType = "level_01";
	public string distalLayerType = "level_02";
	// Use this for initialization
	void Awake () {
		// Establish a random seed for repeatable levels
		if (!useRandomSeed){
			randomSeed = Random.Range(1,1000);
		}
		Random.InitState(randomSeed);

		player = Instantiate(Resources.Load("player") as GameObject);
		gameCamera = Instantiate(Resources.Load("cam",typeof(Camera)) as Camera);
		CameraController cc = gameCamera.GetComponent<CameraController>();
		cc.player = player;
		levelScript = GetComponent<LevelManager>();
		levelScript.randomSeed = randomSeed;
		levelScript.player = player;
		levelScript.gameCamera = gameCamera;
		InitGame();
	}

	void InitGame(){
		levelScript.SetupLevel(level,medialLayerType,distalLayerType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
