using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public static GameObject CurrentHighlight { get; set; }
	private static IList<GameObject> CurrentSelection = new List<GameObject>();

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
		CurrentSelection.Add(gameObject);
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
