using OpenCvSharp;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class WebCameraHandler : MonoBehaviour {
	protected internal WebCamTexture webCamTexture;
	protected internal Texture2D output;
	private RectTransform rectTransform;

	[Header("Camera properties")]
	[SerializeField] private bool UseMinAspect = true;
	[SerializeField] private RawImage rawImage;

	protected virtual void Awake() {
		webCamTexture = new WebCamTexture();
		rectTransform = rawImage.GetComponent<RectTransform>();
	}

	protected virtual void Start() {
		if(WebCamTexture.devices.Length != 0) {
			webCamTexture.deviceName = WebCamTexture.devices.FirstOrDefault(x => !x.isFrontFacing).name;
			if(webCamTexture == null) {
				webCamTexture.deviceName = WebCamTexture.devices[0].name;
			}
		}
		if(webCamTexture == null) {
			Debug.LogError("´No webcamera found");
		} else {
			webCamTexture.Play();
		}
	}

	protected virtual void Update() {
		if(webCamTexture.didUpdateThisFrame) {
			output = OpenCvSharp.Unity.MatToTexture(ImageProcessing(OpenCvSharp.Unity.TextureToMat(webCamTexture)), output);
			float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
			rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);
			rawImage.texture = output;
			webCamTexture.IncrementUpdateCount();
		}
	}

	protected abstract Mat ImageProcessing(Mat input);
}
