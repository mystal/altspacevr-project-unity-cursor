using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public bool isSelectable = true;

	public static GameObject CurrentHighlight { get; set; }
	// TODO: Make this a GameObject unless if we add support for multiselect.
	private static List<GameObject> CurrentSelection = new List<GameObject>();

	public Material NormalMaterial;
	public Material HighlightMaterial;
	public Material SelectionMaterial;

	private MeshRenderer[] meshRenderers;

	void Start() {
		this.meshRenderers = GetComponentsInChildren<MeshRenderer>();
	}

	void Update () {
		if (gameObject == CurrentHighlight) {
			SetMaterial(HighlightMaterial);
		}
		else if (CurrentSelection.Contains(gameObject)) {
			SetMaterial(SelectionMaterial);
		}
		else {
			SetMaterial(NormalMaterial);
		}
	}

	public static void Select(GameObject gameObject) {
		// This is only called on a user's click, so shouldn't hurt performance much.
		Selectable selectable = gameObject.GetComponent<Selectable>();
		if (selectable != null && selectable.isSelectable) {
			CurrentSelection.Add(gameObject);
		}
	}

	public static GameObject GetCurrentSelection() {
		if (CurrentSelection.Count > 0) {
			return CurrentSelection[0];
		}
		return null;
	}

	public static void ClearSelection() {
		CurrentSelection.Clear();
	}

	private void SetMaterial(Material material) {
		if (meshRenderers[0].sharedMaterial != material) {
			foreach (var renderer in meshRenderers) {
				renderer.sharedMaterial = material;
			}
		}
	}
}
