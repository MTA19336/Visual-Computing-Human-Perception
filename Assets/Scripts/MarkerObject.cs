using UnityEngine;

public class MarkerObject : MonoBehaviour {

	public GameObject Model { get; private set; }
	public float size { get; private set; }
	public int id { get; private set; }
	[SerializeField] private int modelScale = 1;
	[SerializeField] private GameObject SelectIndicator;
	[SerializeField] private GameObject defaultModel;
	[SerializeField] [Range(0f, 25f)] private float timeSinceMarkerUpdateTimeout = 1f, positionLerpMax = 1f;
	[SerializeField] [Range(0f, 90f)] private float rotationLerpMax = 10f;
	private Vector3 newMarkerPosition = Vector3.zero, oldMarkerPosition = Vector3.zero;
	private Quaternion newMarkerRotation = Quaternion.identity, oldMarkerRotation = Quaternion.identity;
	private float timeSinceMarkerUpdate = 0;

	private void Start() {
		if(Model == null) {
			SetModel(defaultModel);
		}
		SelectIndicator.SetActive(false);
	}

	public void SetModel(GameObject m, float s) {
		if(Model != null) {
			Destroy(Model);
			Destroy(GetComponent<Collider>());
		}
		Model = Instantiate(m != null ? m : defaultModel, transform);

		SetSize(s);
	}

	public void SetModel(GameObject m) {
		SetModel(m, size);
	}

	public void SetSize(float s) {
		size = s;
		Model.transform.localScale = Vector3.one * s * modelScale;
		SelectIndicator.transform.localScale = new Vector3(s * 1.5f, s * 2.5f, s * 1.5f);
		SelectIndicator.transform.localPosition = Vector3.up * s * 1.25f;
	}

	public void UpdateMarkerTransform(Vector3 pos, Quaternion rot) {
		timeSinceMarkerUpdate = 0;

		oldMarkerPosition = newMarkerPosition;
		newMarkerPosition = pos;

		if((rot * Vector3.up).y > 0) {
			oldMarkerRotation = newMarkerRotation;
			newMarkerRotation = rot;
		}
	}

	public MarkerObject Select() {
		SelectIndicator.SetActive(true);
		return this;
	}
	public void Deselect() {
		SelectIndicator.SetActive(false);
	}

	private void Update() {
		timeSinceMarkerUpdate += Time.deltaTime;
		if(timeSinceMarkerUpdate >= timeSinceMarkerUpdateTimeout) {
			oldMarkerPosition = Vector3.zero;
			oldMarkerRotation = Quaternion.identity;
			Model.SetActive(false);
		} else {
			Model.SetActive(true);
		}

		if(oldMarkerPosition != Vector3.zero && oldMarkerRotation != Quaternion.identity) {
			transform.position = Vector3.Lerp(transform.position, newMarkerPosition, (oldMarkerPosition - newMarkerPosition).magnitude / (size * positionLerpMax));
			transform.rotation = Quaternion.Lerp(transform.rotation, newMarkerRotation, Quaternion.Angle(oldMarkerRotation, newMarkerRotation) / rotationLerpMax);
		} else {
			transform.position = newMarkerPosition;
			transform.rotation = newMarkerRotation;
		}
	}

	public void SetId(int v) {
		id = v;
		name = "MarkerObject[" + v + "]";
	}
}
