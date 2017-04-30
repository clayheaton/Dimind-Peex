using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;


public enum SideCode {Low, High};
public class TileData : MonoBehaviour
{
	public SideCode LeftCode;
	public SideCode RightCode;
	public int tileNumber;
	public List<GameObject> attachedObjects = new List<GameObject>();
}

public class CameraFrameData : MonoBehaviour
{
	public int cameraFrame;
}

public class LevelManager : MonoBehaviour {

	private string levelType;
	private List<GameObject> layers;
	public GameObject player;
	public Camera gameCamera;

	// Use this for initialization
	public void SetupLevel(string level){
		levelType = level;

		layers = new List<GameObject>();

		// Create a ground collider that moves with the camera
		GameObject groundCollider = new GameObject("Ground Collider");
		groundCollider.transform.position = new Vector2(0.0f,1.0f);
		groundCollider.layer = LayerMask.NameToLayer("Ground");

		BoxCollider2D groundBox = groundCollider.AddComponent<BoxCollider2D>();
		groundBox.transform.parent = groundCollider.transform;
		groundBox.size = new Vector2(100000f,1.0f);
		groundBox.offset = new Vector2(0,-0.8f);

		// Ground Layer
		GameObject go    = new GameObject("Ground Layer");
		LevelLayer ll    = go.AddComponent<LevelLayer>();
		ll.gameCamera    = gameCamera;
		ll.levelNumber   = level;
		ll.levelPart     = "grounds";
		ll.yPosition     = 0.65f;
		ll.sortLayerName = "Ground";
		ll.needsCollider = true;
		ll.SetupLevelLayer();
		layers.Add(go);

		// Background Layer
		// The proximate background layer will spawn clouds, too
		GameObject go2    = new GameObject("Background Layer");
		LevelLayer ll2    = go.AddComponent<LevelLayer>();
		ll2.gameCamera    = gameCamera;
		ll2.levelNumber   = level;
		ll2.levelPart     = "backgrounds";
		ll2.yPosition     = 4.5f;
		ll2.sortLayerName = "BackgroundProximate";
		ll2.needsCollider = false;
		ll2.SetupLevelLayer();
		layers.Add(go2);
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}

public class LevelLayer : MonoBehaviour {
	public string levelNumber;
	public string levelPart;
	public float yPosition;
	public string sortLayerName;
	public bool needsCollider;
	private List<GameObject> tiles;
	private List<GameObject> tilesLeftLow;
	private List<GameObject> tilesLeftHigh;
	private List<GameObject> tilesRightLow;
	private List<GameObject> tilesRightHigh;
	private Dictionary<int,GameObject> tileLookup;
	private Dictionary<int,GameObject> activeTiles;
	public Camera gameCamera;
	private float tileWidth;
	private int numTiles;
	private static int levelSize = 100;
	private int[] levelLayout = new int[levelSize];
	private static int tileBuffer  = 4;
	private List<GameObject> objectsNoLongerNeeded;
	private Object[] ground_items_sprites;
	public void SetupLevelLayer()
	{
		tiles          = new List<GameObject>();
		tilesLeftLow   = new List<GameObject>();
		tilesLeftHigh  = new List<GameObject>();
		tilesRightLow  = new List<GameObject>();
		tilesRightHigh = new List<GameObject>();
		tileLookup     = new Dictionary<int,GameObject>();
		activeTiles    = new Dictionary<int,GameObject>();
		objectsNoLongerNeeded = new List<GameObject>();

		string ground_items_resource_path = levelNumber + "/ground_items";
		ground_items_sprites = Resources.LoadAll(ground_items_resource_path, typeof(Sprite));

		string ground_resource_path   = levelNumber + "/" + levelPart;
		Object[] groundSprites = Resources.LoadAll(ground_resource_path, typeof(Sprite));

		// Create the sprites
		for (int i = 0; i < groundSprites.Length; i++){
			Sprite s = groundSprites[i] as Sprite;
			GameObject tile = new GameObject(s.name);
			SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
			sr.sprite = s;
			sr.sortingLayerName = sortLayerName;

			string[] nameParts = s.name.Split (new char[]{'_'});
			string sides = nameParts[nameParts.Length - 1];

			TileData data = tile.AddComponent<TileData>();
			data.tileNumber = i;

			// Hide the prototypes
			tile.SetActive(false);
			tileLookup.Add(i,tile);

			// Set initial y position
			tile.transform.position = new Vector3(tile.transform.position.x,
			                                      yPosition,
												  tile.transform.position.z);

			// Inspect the left side of the tile and add copies of it to
			// the appropriate pool for randomization
			if (sides[0] == 'L'){
				data.LeftCode = SideCode.Low;
				tilesLeftLow.Add(tile);
			} else if (sides[0] == 'H'){
				data.LeftCode = SideCode.High;
				tilesLeftHigh.Add(tile);
			}

			// Same for the right side of the tile
			if (sides[1] == 'L'){
				data.RightCode = SideCode.Low;
				tilesRightLow.Add(tile);
			} else if (sides[1] == 'H'){
				data.RightCode = SideCode.High;
				tilesRightHigh.Add(tile);
			}

			// Add to the general pool
			tiles.Add(tile);
		}

		// Determine the width of the tiles
		// Random.Range is inclusive
		int tn       = Random.Range(0,tiles.Count - 1);
		GameObject t = tiles[tn];
		tileWidth    = t.GetComponent<Renderer>().bounds.size.x - 0.01f;

		// Populate an array with the numbers of the tiles
		for (int i = 0; i < levelSize; i++){
			// Initialize the level by placing a random tile at position 0 and then build off of it.
			if (i == 0){
				int r          = (int)Random.Range(0,numTiles);
				levelLayout[i] = r;
			} else if(i == levelSize - 1){
				// The last tile should match the first tile and the tile before it
				GameObject previousTile = tileLookup[levelLayout[i-1]];
				SideCode lCode          = previousTile.GetComponent<TileData>().RightCode;

				GameObject firstTile    = tileLookup[levelLayout[0]];
				SideCode rCode          = firstTile.GetComponent<TileData>().LeftCode;

				if (lCode == SideCode.Low) {
					foreach (GameObject candidate in tilesLeftLow){
						SideCode candidateRight = candidate.GetComponent<TileData>().RightCode;
						if (candidateRight == rCode){
							TileData tiledata = candidate.GetComponent<TileData>();
							int nextTileCode  = tiledata.tileNumber;
							levelLayout[i]    = nextTileCode;
							break;
						}
					}
				} else if (lCode == SideCode.High){
					foreach (GameObject candidate in tilesLeftHigh){
						SideCode candidateRight = candidate.GetComponent<TileData>().RightCode;
						if (candidateRight == rCode){
							TileData tiledata = candidate.GetComponent<TileData>();
							int nextTileCode  = tiledata.tileNumber;
							levelLayout[i]    = nextTileCode;
							break;
						}
					}
				}

			} else {
				GameObject previousTile = tileLookup[levelLayout[i-1]];
				SideCode lCode          = previousTile.GetComponent<TileData>().RightCode;

				if (lCode == SideCode.Low) {
					int r               = (int)Random.Range(0,tilesLeftLow.Count);
					GameObject nexttile = tilesLeftLow[r];
					TileData tiledata   = nexttile.GetComponent<TileData>();
					int nextTileCode    = tiledata.tileNumber;
					levelLayout[i]      = nextTileCode;
				} else if (lCode == SideCode.High){
					int r               = (int)Random.Range(0,tilesLeftHigh.Count);
					GameObject nexttile = tilesLeftHigh[r];
					TileData tiledata   = nexttile.GetComponent<TileData>();
					int nextTileCode    = tiledata.tileNumber;
					levelLayout[i]      = nextTileCode;
				}
			}
		}
	}

	void addGroundDecorations(int numDecorations, string displayLayer, GameObject referenceTile){
		for (int x = 0; x < numDecorations; x++){
			int idx = (int)(Random.Range(0,ground_items_sprites.Length+1) - 0.001);
			
			Sprite s = ground_items_sprites[idx] as Sprite;
			GameObject decoration = new GameObject(s.name);
			SpriteRenderer sr = decoration.AddComponent<SpriteRenderer>();
			sr.sprite = s;
			sr.sortingLayerName = displayLayer;
			sr.sortingOrder = x;
			decoration.transform.position = new Vector2(referenceTile.transform.position.x + Random.Range(-2,2), 
			                                            referenceTile.transform.position.y + 0.1f);

			// This adds them to the deletion queue for when they no longer are near the player
			objectsNoLongerNeeded.Add(decoration);
		}
	}

	void Update(){
		int cameraFrame = (int)(gameCamera.transform.position.x / tileWidth);

		HashSet<int> framesNeededForDisplay = new HashSet<int>();

		// Add and Display tiles that we need
		for (int i = cameraFrame - tileBuffer; i < cameraFrame + tileBuffer; i++){

			int ii = i;
			if (i < 0 && i%levelSize == 0){
				ii = 0;
			} else if (i < 0 ){
				ii = levelSize + (i % levelSize);
			} else if (i >= levelSize){
				ii = i % levelSize;
			}

			int neededFrameNumber = ii;
			framesNeededForDisplay.Add(neededFrameNumber);

			// Do we already have this tile active?
			bool haveTile = false;
			foreach (int key in activeTiles.Keys){
				if (key == neededFrameNumber) {
					haveTile = true;
					break;
				}
			}
			// We don't have it so create it and add it to activeTiles
			if (!haveTile){
				int neededTileNumber = levelLayout[neededFrameNumber];
				GameObject thistile  = tileLookup[neededTileNumber];
				GameObject tilecopy  = Instantiate(thistile);
				tilecopy.name        = levelNumber + " " + levelPart + " tile " + ii.ToString();
				tilecopy.AddComponent<CameraFrameData>();

				CameraFrameData fd = tilecopy.GetComponent<CameraFrameData>();
				fd.cameraFrame     = neededFrameNumber;

				// Position the tile
				int actualpos = i;
				tilecopy.transform.position = new Vector3(actualpos*tileWidth,
				                                          thistile.transform.position.y,
														  tilecopy.transform.position.z);
				tilecopy.SetActive(true);

				// If we are the ground layer, then create the ground items, based on the
				// tile number as a seed.
				if (sortLayerName == "Ground"){
					Random.InitState(ii);
					int numFront = (int)Random.Range(5,7);
					int numBack  = (int)Random.Range(5,7);

					addGroundDecorations(numFront,"GroundFront",tilecopy);
					addGroundDecorations(numBack, "GroundBack",tilecopy);
				}
				
				// If we are the Proximate Background layer, attempt to attach clouds.
				if (sortLayerName == "BackgroundProximate"){
					if (Random.Range(0,100) > 60){
						GameObject skyObjectPrefab = Resources.Load("SkyObject") as GameObject;
						GameObject skyObject = Instantiate(skyObjectPrefab);
						skyObject.transform.position = new Vector2 (tilecopy.transform.position.x,skyObject.transform.position.y);
						skyObject.SetActive(true);
						tilecopy.GetComponent<TileData>().attachedObjects.Add(skyObject);
					}
				}
				// Keep a dicionary of active tiles
				activeTiles.Add(neededFrameNumber,tilecopy);
			}
		}

		// Remove tiles that we no longer need to display
		List<int> keysToRemove = new List<int>();

		// They keys in activeTiles are equal to the cameraFrame
		foreach (int key in activeTiles.Keys){
			if (framesNeededForDisplay.Contains(key)){
				continue;
			} else {
				// Flag the tile for removal
				keysToRemove.Add(key);
			}
		}

		// Get a reference to the tile that we don't need,
		// remove it from the activeTiles List, then destroy the tile.
		foreach (int key in keysToRemove){
			GameObject toDestroy = activeTiles[key];

			// Deactivate the sky objects.
			// Hand sky objects to destroy over to a level-scoped list that destroys them when they are 6+ units from the player
			foreach(GameObject attachedObject in toDestroy.GetComponent<TileData>().attachedObjects){
				objectsNoLongerNeeded.Add(attachedObject);
			}

			toDestroy.GetComponent<TileData>().attachedObjects.Clear();

			activeTiles.Remove(key);
			Destroy(toDestroy);
		}

		// Loop through all objects in objectsNoLongerNeeded and if they are
		// far enough away from the player, destroy them.
		foreach(GameObject go in objectsNoLongerNeeded){
				GameObject p = GameObject.FindWithTag("Player");
				if (p != null && go != null){
					float dist = Vector2.Distance(p.transform.position,go.transform.position);
					if (Mathf.Abs(dist) > 20.0f){
						Destroy(go);
					}
				}
			}

	}
}
