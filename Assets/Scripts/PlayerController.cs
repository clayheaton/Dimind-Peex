using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed = 8;
	public Vector2 movement;
	private Rigidbody2D rb2d;
	public Vector2 jump;
	public float jumpForce = 2.0f;
	public float moveForce = 3650f;
	public bool isGrounded = true;
	private bool doJump = false;

	// Use this for initialization
	void Start () {
		rb2d     = GetComponent<Rigidbody2D> ();
		movement = new Vector2 (0, 0);
		jump     = new Vector3 (0.0f, 2.0f);
	}

	void OnCollisionStay2D(Collision2D collision){
		Debug.Log(collision);
		isGrounded = true;
	}

	void Update(){
		if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
			isGrounded = false;
			doJump = true;
		}
	}

	// Fixed update is called just before physics updates.
	// Moving the camera
	void FixedUpdate() 
	{
		// Keys are set by default in the Input Manager
		// This gets key inputs and applies it to the player object.
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical   = Input.GetAxis ("Vertical");
		// movement             = new Vector2 (moveHorizontal, moveVertical);
		// transform.position   = new Vector3 (transform.position.x + movement.x * speed, transform.position.y, transform.position.z);


		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(moveHorizontal * rb2d.velocity.x < speed)
			// ... add a force to the player.
			rb2d.AddForce(Vector2.right * moveHorizontal * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rb2d.velocity.x) > speed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * speed, rb2d.velocity.y);

		if (doJump){
			doJump = false;
			isGrounded = false;
			rb2d.AddForce(jump * jumpForce, ForceMode2D.Impulse);
		}
			

	}
}
