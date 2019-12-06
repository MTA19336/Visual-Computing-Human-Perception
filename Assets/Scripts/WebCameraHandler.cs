using OpenCvSharp;
using UnityEngine;
using UnityEngine.Android;

public abstract class WebCameraHandler : MonoBehaviour {
	protected internal uint webCamDeviceIndex = 0;
	protected internal WebCamTexture webCamTexture;
	protected internal Texture2D output;

	[Header("Camera properties")] [SerializeField] protected internal uint requestedWidth = 1280;
	[SerializeField] protected internal uint requestedHeight = 720, requestedFPS = 30;

	protected virtual void Awake() {
		webCamTexture = new WebCamTexture((int)requestedWidth, (int)requestedHeight, (int)requestedFPS);
	}

	protected virtual void Start() {
		if(WebCamTexture.devices.Length == 0) {
			Debug.Log("WebCamDevice not found.");
			return;
		}
		Debug.Log(WebCamTexture.devices.Length + " webCamTexture devices found");
		webCamTexture.deviceName = WebCamTexture.devices[webCamDeviceIndex].name;
		webCamTexture.Play();
	}

	protected virtual void Update() {
		if(webCamTexture.didUpdateThisFrame) {
			output = OpenCvSharp.Unity.MatToTexture(ImageProcessing(OpenCvSharp.Unity.TextureToMat(webCamTexture)), output);
			CameraUpdate(output);
			webCamTexture.IncrementUpdateCount();
		}
	}

	protected abstract Mat ImageProcessing(Mat input);
	protected abstract void CameraUpdate(Texture2D output);

	[ContextMenu("Change Web Camera")]
	private void nextWebCamDevice() {
		webCamTexture.Stop();
		if(webCamDeviceIndex >= WebCamTexture.devices.Length - 1) {
			webCamDeviceIndex = 0;
			webCamTexture.deviceName = WebCamTexture.devices[webCamDeviceIndex].name;
		} else {
			webCamDeviceIndex++;
			webCamTexture.deviceName = WebCamTexture.devices[webCamDeviceIndex].name;
		}
		webCamTexture.requestedFPS = requestedFPS;
		webCamTexture.requestedHeight = (int)requestedHeight;
		webCamTexture.requestedWidth = (int)requestedWidth;
		webCamTexture.Play();
	}

	[ContextMenu("Reload Web Camera")]
	private void reloadWebCamDevice() {
		webCamTexture.Stop();
		webCamTexture.requestedFPS = requestedFPS;
		webCamTexture.requestedHeight = (int)requestedHeight;
		webCamTexture.requestedWidth = (int)requestedWidth;
		webCamTexture.Play();
	}
}
