using System;
using UnityEngine;

public class Marker : MonoBehaviour {

	public GameObject Model { get; private set; }
	public int id { get; private set; }
	[SerializeField] private GameObject defaultModel;
	[SerializeField] private float positionLerpSpeed = 0.1f, rotationLerpSpeed = 0.1f, colorLerpSpeed = 0.1f;
	private Vector3 lastMarkerPosition = Vector3.zero, lastMarkerRotation = Vector3.zero, smoothDampVelocity = Vector3.zero;
	private Color modelColor = new Color();
	private float timeSinceMarkerUpdate = 0;
	

	private void Awake() {
		SetModel(defaultModel);
		Debug.Log(name + " has awoken");
	}

	public void SetModel(GameObject m) {
		Destroy(Model);
		Model = Instantiate(m != null ? m : defaultModel, transform);
		modelColor = Model.GetComponent<Renderer>().material.color;
	}

	public void UpdateMarkerTransform(Vector3 pos, Vector3 rot) {
		timeSinceMarkerUpdate = 0;
		lastMarkerPosition = pos;
		lastMarkerRotation = rot;
	}


	private void Update() {
		timeSinceMarkerUpdate += Time.deltaTime;
		if (timeSinceMarkerUpdate >= 1) {
			Model.GetComponent<Renderer>().material.color = Color.clear; //Color.Lerp(Model.GetComponent<Renderer>().material.color, Color.clear, colorLerpSpeed);
		} else {
			Model.GetComponent<Renderer>().material.color = modelColor;
		}

		//transform.position = Vector3.SmoothDamp(transform.position, lastMarkerPosition, ref smoothDampVelocity, lerpSpeed * Time.deltaTime);
		transform.position = Vector3.Lerp(transform.position, lastMarkerPosition, positionLerpSpeed);
		transform.rotation = Quaternion.AngleAxis(lastMarkerRotation.magnitude * Mathf.Rad2Deg, lastMarkerRotation);
	}

	public void SetId(int v) {
		id = v;
		name = "MarkerObject[" + v + "]";
	}
}
