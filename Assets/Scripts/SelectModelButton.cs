using UnityEngine;
using UnityEngine.UI;

public class SelectModelButton : MonoBehaviour {
	public GameObject ModelPrefab { private set; get; }
	private GameObject previewModel;
	private Collider previewModelCollider;
	private RectTransform rectTransform;
	private float maxMinRatio;
	[SerializeField] private Text text;
	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}
	private void Start() {
		text.text = ModelPrefab.name;
		float xzLength = Mathf.Sqrt((previewModelCollider.bounds.size.z * previewModelCollider.bounds.size.z) + (previewModelCollider.bounds.size.x * previewModelCollider.bounds.size.x));
		maxMinRatio = Mathf.Min(previewModelCollider.bounds.size.x, previewModelCollider.bounds.size.y, previewModelCollider.bounds.size.z, xzLength) / Mathf.Max(previewModelCollider.bounds.size.x, previewModelCollider.bounds.size.y, previewModelCollider.bounds.size.z, xzLength);
	}
	public void AddModelToMarkerObject() {
		if(GameManager.instance.SelectedMarker != null) {

			GameManager.instance.SelectedMarker.SetModel(ModelPrefab);
		}
	}

	public void setModel(GameObject m) {
		ModelPrefab = m;
		previewModel = Instantiate(m, transform);
		previewModelCollider = previewModel.GetComponent<Collider>();
		setLayerInChildren(previewModel, 5);
	}

	private void OnRectTransformDimensionsChange() {
		previewModel.transform.localScale = Vector3.one * (maxMinRatio * rectTransform.rect.size.x);
		previewModel.transform.localPosition = Vector3.down * rectTransform.rect.size.y / 2;
		previewModel.transform.localPosition += Vector3.back * rectTransform.rect.size.y;
	}

	private void Update() {
		if(previewModel != null) {
			previewModel.transform.Rotate(0, Time.deltaTime * 25, 0);
		}

	}

	private void setLayerInChildren(GameObject gobj, int layer) {
		gobj.layer = layer;
		foreach(Transform child in gobj.transform) {
			setLayerInChildren(child.gameObject, layer);
		}
	}
}

