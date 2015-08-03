using UnityEngine;
using System.Collections;

public class BallShooter : MonoBehaviour {

	public GameObject[] projectilePrefabs;
	public Transform ballParent;

	public float cooldown = 1;
	public float firingVelocity = 10;

	private float cooldownLeft = 0;

	private CharacterController characterController;

	// Use this for initialization
	void Start() {
		characterController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update() {
		if (cooldownLeft > 0) {
			cooldownLeft -= Time.deltaTime;
		} else if (Input.GetButton("Fire3")) {
			cooldownLeft = cooldown;
			int spawnIndex = Random.Range(0, projectilePrefabs.Length);
			GameObject newBall = Instantiate(projectilePrefabs[spawnIndex]);
			if (ballParent != null) {
				newBall.transform.parent = ballParent;
			}
			newBall.transform.position = transform.position;
			newBall.transform.rotation = transform.rotation;
			Rigidbody ballBody = newBall.GetComponent<Rigidbody>();
			// TODO: Add player's velocity.
			ballBody.velocity = (transform.forward * firingVelocity)/* + characterController.velocity*/;
		}
	}
}
