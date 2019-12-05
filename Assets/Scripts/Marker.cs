using OpenCvSharp.Demo;
using System;
using UnityEngine;

public class Marker : MonoBehaviour {

	public GameObject Model { get; private set; }
	public int id { get; private set; }
	[SerializeField] private GameObject defaultModel;
	[SerializeField] private float positionLerpTime = 0.1f, rotationLerpTime = 0.1f, timeSinceMarkerUpdateTimeout = 0.5f;
	private Vector3 newMarkerPosition = Vector3.zero, newMarkerRotation = Vector3.zero, currentMarkerRotation = Vector3.zero, currentMarkerPosition = Vector3.zero, smoothDampPositionVelocity = Vector3.zero, smoothDampRotationVelocity = Vector3.zero;
	private float timeSinceMarkerUpdate = 0;
	private bool markerUpdateThisFrame = false;


	private void Start() {
		SetModel(defaultModel);
		Debug.Log(name + " has awoken");
	}

	public void SetModel(GameObject m) {
		Destroy(Model);
		Model = Instantiate(m != null ? m : defaultModel, transform);
		Model.transform.localScale = Vector3.one * WebCamera.instance.ArucoSquareDim;
	}

	public void UpdateMarkerTransform(Vector3 pos, Vector3 rot) {
		timeSinceMarkerUpdate = 0;
		newMarkerPosition = pos;

		if(rot.normalized.x > 0) {
			newMarkerRotation = rot;
		}
	}


	private void Update() {
		timeSinceMarkerUpdate += Time.deltaTime;
		if(timeSinceMarkerUpdate >= timeSinceMarkerUpdateTimeout) {
			Model.SetActive(false);
		} else {
			Model.SetActive(true);
		}

		if(markerUpdateThisFrame) { }

		currentMarkerPosition = Vector3.Lerp(currentMarkerPosition, newMarkerPosition, positionLerpTime); //Vector3.SmoothDamp(currentMarkerPosition, newMarkerPosition, ref smoothDampPositionVelocity, positionLerpTime * Time.deltaTime);
		transform.position = currentMarkerPosition;

		currentMarkerRotation = Vector3.Lerp(currentMarkerRotation, newMarkerRotation, rotationLerpTime);  //Vector3.SmoothDamp(currentMarkerRotation, newMarkerRotation, ref smoothDampRotationVelocity, rotationLerpTime * Time.deltaTime);
		transform.rotation = Quaternion.AngleAxis(currentMarkerRotation.magnitude * Mathf.Rad2Deg, currentMarkerRotation);
		transform.Rotate(Vector3.right * 90);
	}

	public void SetId(int v) {
		id = v;
		name = "MarkerObject[" + v + "]";
	}
}
