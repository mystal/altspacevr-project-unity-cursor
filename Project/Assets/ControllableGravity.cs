using UnityEngine;
using System.Collections;

public class ControllableGravity : MonoBehaviour {
	public Vector3 gravity = new Vector3(0, -10, 0);

	private Rigidbody rigidBody;

	// Use this for initialization
	void Start() {
		rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update() {
	}

	void FixedUpdate() {
		rigidBody.AddForce(gravity, ForceMode.Acceleration);
	}
}
