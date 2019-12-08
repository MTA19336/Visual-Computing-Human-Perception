using UnityEngine;
using UnityEngine.UI;

public class PopulateModelPanel : MonoBehaviour {
	[SerializeField] private GameObject modelButtonPrefab;
	[SerializeField] private Object[] markerModels;
	private GridLayoutGroup gridLayoutGroup;
	private RectTransform rectTransform;

	private void Awake() {
		gridLayoutGroup = GetComponent<GridLayoutGroup>();
		rectTransform = GetComponent<RectTransform>();
		markerModels = Resources.LoadAll("MarkerModels", typeof(GameObject));
	}

	private void Start() {
		GameObject newObj;
		foreach(Object markerModel in markerModels) {
			newObj = (GameObject)Instantiate(modelButtonPrefab, transform);
			newObj.GetComponent<SelectModelButton>().setModel((GameObject)markerModel);
		}
	}

	private void OnRectTransformDimensionsChange() {
		if(gridLayoutGroup != null && rectTransform != null) {
			gridLayoutGroup.cellSize = Vector2.one * (rectTransform.rect.width / 2);

		}
	}
}
