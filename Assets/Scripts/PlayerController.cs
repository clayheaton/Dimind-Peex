using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Vector2 movement;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d     = GetComponent<Rigidbody2D> ();
		movement = new Vector2 (0, 0);
	}

	// Fixed update is called just before physics updates.
	// Moving the camera
	void FixedUpdate() 
	{
		// Keys are set by default in the Input Manager
		// This gets key inputs and applies it to the player object.
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical   = Input.GetAxis ("Vertical");
		movement             = new Vector2 (moveHorizontal, moveVertical);
		transform.position = new Vector3 (transform.position.x + movement.x * speed, transform.position.y, transform.position.z);
	}


	// Trying to work with a fixed camera and moving background.
	// void FixedUpdate() 
	// {
	// 	// Keys are set by default in the Input Manager
	// 	// This gets key inputs and applies it to the player object.
	// 	float moveHorizontal = Input.GetAxis ("Horizontal");
	// 	float moveVertical   = Input.GetAxis ("Vertical");

	// 	if (transform.position.x > maxMove) {
	// 		transform.position = new Vector3 (maxMove, transform.position.y, transform.position.z);
	// 		playerPushingEdge = true;
	// 	} else if (transform.position.x < -maxMove) {
	// 		transform.position = new Vector3 (-maxMove, transform.position.y, transform.position.z);
	// 		playerPushingEdge = true;
	// 	} else {
	// 		playerPushingEdge = false;
	// 		if(transform.position.x == maxMove && moveHorizontal >= 0){
	// 			movement = new Vector2 (0, moveVertical);
	// 		} else if (transform.position.x == -maxMove && moveHorizontal <= 0) {
	// 			movement = new Vector2 (0, moveVertical);
	// 		} else {
	// 			movement = new Vector2 (moveHorizontal, moveVertical);
	// 		}
	// 		transform.position = new Vector3 (transform.position.x + movement.x * speed, transform.position.y, transform.position.z);
	// 	}
	// }
}
