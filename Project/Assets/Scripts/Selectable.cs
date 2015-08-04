using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public bool isSelectable = true;

	public static GameObject CurrentHighlight { get; set; }
	private static GameObject CurrentSelection;

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
		else if (gameObject == CurrentSelection) {
			SetMaterial(SelectionMaterial);
		}
		else {
			SetMaterial(NormalMaterial);
		}
	}

	public static void Select(GameObject gameObject) {
		Selectable selectable = gameObject.GetComponent<Selectable>();
		if (selectable != null && selectable.isSelectable) {
			CurrentSelection = gameObject;
		}
	}

	public static GameObject GetCurrentSelection() {
		return CurrentSelection;
	}

	public static void ClearSelection() {
		CurrentSelection = null;
	}

	private void SetMaterial(Material material) {
		if (meshRenderers[0].sharedMaterial != material) {
			foreach (var renderer in meshRenderers) {
				renderer.sharedMaterial = material;
			}
		}
	}
}
