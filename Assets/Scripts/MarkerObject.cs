using UnityEngine;

public class MarkerObject : MonoBehaviour {

	public GameObject Model { get; private set; }
	public int id { get; private set; }
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
	}

	public void SetModel(GameObject m) {
		if(Model != null) {
			Destroy(Model);
		}
		Model = Instantiate(m != null ? m : defaultModel, transform);

		Model.transform.localScale = Vector3.one * ArucoMarkerDetector.instance.ArucoSquareDim;
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
			transform.position = Vector3.Lerp(transform.position, newMarkerPosition, (oldMarkerPosition - newMarkerPosition).magnitude / (ArucoMarkerDetector.instance.ArucoSquareDim * positionLerpMax));
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
