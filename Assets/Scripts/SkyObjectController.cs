using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkyObjectController : MonoBehaviour {
	public string levelNumber;
	public string levelPart;
	private float skyObjectSpeed;
	private int direction = 1;
	void Awake() {
		levelNumber = "level_01";
		levelPart   = "skies";
		CreateSkyObject();
	}

	public void CreateSkyObject()
	{
		string resource_path   = levelNumber + "/" + levelPart;
		Object[] skySprites = Resources.LoadAll(resource_path, typeof(Sprite));
		int numSkySprites = skySprites.Length;

		Sprite skySprite = skySprites[Random.Range(0,numSkySprites-1)] as Sprite;

		GameObject thisSkyObject = gameObject;
		thisSkyObject.transform.position = new Vector2(0,Random.Range(6,10));

		SpriteRenderer sr = thisSkyObject.AddComponent<SpriteRenderer>();
		sr.sprite = skySprite;
		int num = (int)Random.Range(0,100);
		if (num > 50){
			sr.sortingLayerName = "GroundBack";
		} else {
			sr.sortingLayerName = "GroundFront";
		}

		skyObjectSpeed = Random.Range(100,200)/10000f;
	}
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		// if (Mathf.Abs(gameObject.transform.position.x) > 20) {
		// 	direction *= -1;
		// }
		gameObject.transform.position = new Vector2(gameObject.transform.position.x + (direction*skyObjectSpeed), gameObject.transform.position.y);
	}
}
