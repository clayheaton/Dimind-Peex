using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public float toleratedXOffset;
	private Vector3 offset;
	private Vector3 originalOffset;
	public float originalY;

	// Use this for initialization
	void Awake () {
		offset         = transform.position - player.transform.position;
		originalOffset = transform.position - player.transform.position;
	}

	// Best to use LateUpdate for cameras and procedural animation
	// This lets us set the camera position when we know absolutely
	// that the player has moved for the frame.
	void LateUpdate() {
		offset = transform.position - player.transform.position;
		// Debug.Log(player.transform.position);
		if (Mathf.Abs (offset.x) >= toleratedXOffset) {

			if (offset.x < 0) {
				transform.position = player.transform.position + new Vector3 (-toleratedXOffset, originalOffset.y, originalOffset.z);
			} else {
				transform.position = player.transform.position + new Vector3 (toleratedXOffset, originalOffset.y, originalOffset.z);
			}
			transform.position = new Vector3 (transform.position.x,originalY,transform.position.z);
		}
	}
}
