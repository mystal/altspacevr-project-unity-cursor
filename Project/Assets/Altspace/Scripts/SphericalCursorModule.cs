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
	private static int SelectableLayerMask = LayerMask.NameToLayer("SelectableObject");
	private static int EnvironmentLayerMask = LayerMask.NameToLayer("Environment");
	private static int ColliderMask = (1 << SelectableLayerMask) | (1 << EnvironmentLayerMask);

	// This is the Cursor game object. Your job is to update its transform on each frame.
	private GameObject Cursor;

	// This is the Cursor mesh. (The sphere.)
	private MeshRenderer CursorMeshRenderer;
	private Renderer CursorRenderer;

	// This is the scale to set the cursor to if no ray hit is found.
	private Vector3 DefaultCursorScale = new Vector3(10.0f, 10.0f, 10.0f);

	// Maximum distance to ray cast.
	private const float MaxDistance = 100.0f;

	// Sphere radius to project cursor onto if no raycast hit.
	private const float SphereRadius = 10.0f;

	// Screen position the cursor is currently at.
	private Vector3 CursorScreenPos = new Vector3();

    void Awake() {
		Cursor = transform.Find("Cursor").gameObject;
		CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
		CursorRenderer = CursorMeshRenderer.GetComponent<Renderer>();
        CursorRenderer.material.color = DefaultColor;
		CursorScreenPos.x = Screen.width / 2.0f;
		CursorScreenPos.y = Screen.height / 2.0f;
    }	

	void Update() {
		UpdateCursor();
	}

	private void UpdateCursor() {
		float mouseDx = Input.GetAxisRaw("Mouse X");
		float mouseDy = Input.GetAxisRaw("Mouse Y");
		CursorScreenPos.x += mouseDx * Sensitivity;
		CursorScreenPos.y += mouseDy * Sensitivity;

		// Perform ray cast to find object cursor is pointing at.
		Selectable.CurrentHighlight = null;
		if (Input.GetButtonDown("Fire1")) {
			Selectable.ClearSelection();
		}
		var ray = Camera.main.ScreenPointToRay(CursorScreenPos);
		var cursorHit = new RaycastHit();
		if (Physics.Raycast(ray, out cursorHit, MaxDistance, ColliderMask)) {
			Cursor.transform.position = cursorHit.point;
			float scale = (cursorHit.distance * DistanceScaleFactor + 1.0f) / 2.0f;
			Cursor.transform.localScale.Set(scale, scale, scale);
			if (cursorHit.collider.gameObject.layer == SelectableLayerMask) {
				if (Input.GetButtonDown("Fire1")) {
					Selectable.Select(cursorHit.collider.gameObject);
				}
				Selectable.CurrentHighlight = cursorHit.collider.gameObject;
				CursorRenderer.material.color = SelectedColor;
			} else {
				CursorRenderer.material.color = EnvironmentColor;
			}
		} else {
			Cursor.transform.position = ray.GetPoint(SphereRadius);
			Cursor.transform.localScale = DefaultCursorScale;
			CursorRenderer.material.color = DefaultColor;
		}
	}
}
