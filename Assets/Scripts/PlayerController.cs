using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb2d;
	public float maxSpeed = 5f;
	private Transform groundCheck;
	public float jumpForce = 1000f;
	public float moveForce = 365f;
	public bool grounded = false;
	public bool jump = false;


	// Use this for initialization
	void Awake () {
		groundCheck = transform.Find("groundcheck");
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void Update(){
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  

		if (Input.GetKeyDown(KeyCode.Space) && grounded) {
			jump = true;
		}
	}

	// Fixed update is called just before physics updates.
	// Moving the camera
	void FixedUpdate() 
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rb2d.velocity.x < maxSpeed)
			// ... add a force to the player.
			rb2d.AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rb2d.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

		// If the player should jump...
		Debug.Log(rb2d.velocity);
		if(jump)
		{
			// Add a vertical force to the player.
			rb2d.AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
}
