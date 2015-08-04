using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
	// This is a sensitivity parameter that should adjust how sensitive the mouse control is.
	public float Sensitivity;

	// This is a scale factor that determines how much to scale down the cursor based on its collision distance.
	public float DistanceScaleFactor;

	// Cursor colors.
	public Color DefaultColor = new Color(0.5f, 0.5f, 0.5f);
	public Color EnvironmentColor = new Color(1.0f, 1.0f, 1.0f);
	public Color SelectedColor = new Color(0.0f, 0.8f, 1.0f);

	// This is the layer mask to use when performing the ray cast for the objects.
	// The furniture in the room is in SelectableObject, the floor is in Environment, everything else is Default.
	private static int SelectableLayerMask;
	private static int EnvironmentLayerMask;
	private static int CubeLayerMask;
	private static int ColliderMask;

	// This is the Cursor game object. Your job is to update its transform on each frame.
	private GameObject Cursor;
	private Renderer CursorRenderer;

	// This is the scale to set the cursor to if no ray hit is found.
	private Vector3 DefaultCursorScale = new Vector3(10.0f, 10.0f, 10.0f);

	// Maximum distance to ray cast.
	private const float MaxDistance = 100.0f;

	// Sphere radius to project cursor onto if no raycast hit.
	private const float SphereRadius = 10.0f;

	// Screen position the cursor is currently at.
	private Vector3 CursorScreenPos;

	// Data for dragging objects around.
	private GameObject objectDragger;
	private float cursorDragRadius = 0;

    void Awake() {
		Cursor = transform.Find("Cursor").gameObject;
		var cursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
		CursorRenderer = cursorMeshRenderer.GetComponent<Renderer>();
        CursorRenderer.material.color = DefaultColor;

		// Initialize Cursor to middle of screen.
		CursorScreenPos.x = Screen.width / 2.0f;
		CursorScreenPos.y = Screen.height / 2.0f;

		SelectableLayerMask = LayerMask.NameToLayer("SelectableObject");
		EnvironmentLayerMask = LayerMask.NameToLayer("Environment");
		CubeLayerMask = LayerMask.NameToLayer("Cube");
		ColliderMask = (1 << SelectableLayerMask) | (1 << EnvironmentLayerMask)| (1 << CubeLayerMask);
    }	

	void Update() {
		UpdateCursorScreenPos();

		var cursorHit = new RaycastHit();
		var ray = RaycastFromCursorScreenPos(out cursorHit);
		UpdateCursorPositionAndScale(ray, cursorHit);
		UpdateCursorSelections(cursorHit);

		UpdateObjectPhysics(cursorHit);
		UpdateObjectDragger();
	}
	
	private void UpdateCursorScreenPos() {
		float mouseDx = Input.GetAxisRaw("Mouse X");
		float mouseDy = Input.GetAxisRaw("Mouse Y");
		CursorScreenPos.x += mouseDx * Sensitivity;
		CursorScreenPos.y += mouseDy * Sensitivity;
	}
	
	private Ray RaycastFromCursorScreenPos(out RaycastHit cursorHit) {
		// Sigh, no Tuples...
		var ray = Camera.main.ScreenPointToRay(CursorScreenPos);
		Physics.Raycast(ray, out cursorHit, MaxDistance, ColliderMask);
		return ray;
	}

	private void UpdateCursorPositionAndScale(Ray ray, RaycastHit cursorHit) {
		if (DraggingObject()) {
			// Keep Cursor locked at initial radius that object was at.
			Cursor.transform.position = ray.GetPoint(cursorDragRadius);
			float scale = (cursorDragRadius * DistanceScaleFactor + 1.0f) / 2.0f;
			Cursor.transform.localScale.Set(scale, scale, scale);
		} else if (cursorHit.collider != null) {
			// Move Cursor to hit position and scale it.
			Cursor.transform.position = cursorHit.point;
			float scale = (cursorHit.distance * DistanceScaleFactor + 1.0f) / 2.0f;
			Cursor.transform.localScale.Set(scale, scale, scale);
		} else {
			// Set Cursor at a point on a virtual sphere.
			Cursor.transform.position = ray.GetPoint(SphereRadius);
			Cursor.transform.localScale = DefaultCursorScale;
			CursorRenderer.material.color = DefaultColor;
		}
	}

	private void UpdateCursorSelections(RaycastHit cursorHit) {
		// Clear old highlight and selection.
		Selectable.CurrentHighlight = null;
		if (Input.GetButtonDown("Fire1")) {
			Selectable.ClearSelection();
		}

		if (cursorHit.collider != null) {
			GameObject hitObject = cursorHit.collider.gameObject;
			if (hitObject.layer == SelectableLayerMask ||
			    hitObject.layer == CubeLayerMask) {
				if (Input.GetButtonDown("Fire1")) {
					Selectable.Select(hitObject);
					// Set radius to lock Cursor at if dragging.
					cursorDragRadius = cursorHit.distance;
				}
				Selectable.CurrentHighlight = hitObject;
				CursorRenderer.material.color = SelectedColor;
			} else {
				CursorRenderer.material.color = EnvironmentColor;
			}
		}
	}

	private void UpdateObjectPhysics(RaycastHit cursorHit) {
		if (cursorHit.collider != null) {
			GameObject hitObject = cursorHit.collider.gameObject;
			if (Input.GetButtonDown("Fire2") && hitObject.layer == CubeLayerMask) {
				Vector3 gravity = hitObject.GetComponent<WallGravity>().gravity;
				GameObject selected = Selectable.GetCurrentSelection();
				if (selected != null) {
					// Change the selected object's gravity.
					selected.GetComponent<ControllableGravity>().gravity = gravity;
				} else {
					// TODO: Change player's gravity!
				}
			}
		}
	}

	private void UpdateObjectDragger() {
		GameObject selected = Selectable.GetCurrentSelection();
		if (Input.GetButtonDown("Fire1") && selected != null) {
			// Create joint and attach it to the Cursor.
			Rigidbody selectedRigidbody = selected.GetComponent<Rigidbody>();
			objectDragger = new GameObject("Object Dragger");
			objectDragger.transform.parent = Cursor.transform;
			Rigidbody body = objectDragger.AddComponent<Rigidbody>();
			body.isKinematic = true;

			// TODO: Use a joint type that lets the object move according to physics.
			var joint = objectDragger.AddComponent<FixedJoint>();
			joint.connectedBody = selectedRigidbody;
		} else if (Input.GetButtonUp("Fire1")) {
			// Delete joint.
			Destroy(objectDragger);
		}
	}

	private bool DraggingObject() {
		return objectDragger != null;
	}
}
