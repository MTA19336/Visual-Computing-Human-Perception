using OpenCvSharp.Demo;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {

	public GameObject Model { get; private set; }
	public int id { get; private set; }
	[SerializeField] private GameObject defaultModel;
	[SerializeField] [Range(0f, 1f)] private float positionLerpTime = 0.1f, rotationLerpTime = 0.1f, timeSinceMarkerUpdateTimeout = 0.5f;
	private Vector3 newMarkerPosition = Vector3.zero, markerPositionDelta = Vector3.zero;
	private Quaternion newMarkerRotation = Quaternion.identity, currentMarkerRotation = Quaternion.identity;
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

	public void UpdateMarkerTransform(Vector3 pos, Quaternion rot) {
		timeSinceMarkerUpdate = 0;
		markerPositionDelta = pos - newMarkerPosition;
		newMarkerPosition = pos;

		markerUpdateThisFrame = true;

		if((rot * Vector3.down).z < 0) {
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

		if(markerUpdateThisFrame) {
			markerUpdateThisFrame = false;
		} else {
			newMarkerPosition += markerPositionDelta*Time.deltaTime;
		}

		transform.position = Vector3.Lerp(transform.position, newMarkerPosition, positionLerpTime);

		currentMarkerRotation = Quaternion.Lerp(currentMarkerRotation, newMarkerRotation, rotationLerpTime); //currentMarkerRotation = Vector3.Lerp(currentMarkerRotation, newMarkerRotation, rotationLerpTime);  //Vector3.SmoothDamp(currentMarkerRotation, newMarkerRotation, ref smoothDampRotationVelocity, rotationLerpTime * Time.deltaTime);
		transform.rotation = currentMarkerRotation;
		transform.Rotate(Vector3.right * 90);


	}

	public void SetId(int v) {
		id = v;
		name = "MarkerObject[" + v + "]";
	}
}
