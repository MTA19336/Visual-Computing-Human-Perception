using OpenCvSharp;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class WebCameraHandler : MonoBehaviour {
	private WebCamTexture webCamTexture;
	private Texture2D output;
	private RectTransform rectTransform;
	private int currentId = 0;

	[Header("Camera properties")]
	//[SerializeField] private bool UseMinAspect = true;
	[SerializeField] private RawImage WebCameraSurface;

	protected virtual void Awake() {
		webCamTexture = new WebCamTexture(Screen.width, Screen.height);
		rectTransform = WebCameraSurface.GetComponent<RectTransform>();
	}

	protected virtual void Start() {
		changeCamera(currentId);
	}

	public void changeCamera(int id) {
		if(webCamTexture.isPlaying) {
			webCamTexture.Stop();
		}
		webCamTexture.deviceName = WebCamTexture.devices[id].name;
		webCamTexture.Play();
	}

	[ContextMenu("nextCamera")]
	public void nextCamera() {
		currentId++;
		if (currentId >= WebCamTexture.devices.Length) {
			currentId = 0;
		}
		changeCamera(currentId);
	}

	protected virtual void Update() {
		if(webCamTexture.didUpdateThisFrame) {
			output = OpenCvSharp.Unity.MatToTexture(ImageProcessing(OpenCvSharp.Unity.TextureToMat(webCamTexture)), output);
			//float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)output.width, Screen.height / (float)output.height) : Mathf.Max(Screen.width / (float)output.width, Screen.height / (float)output.height);
			float aspect = Screen.height / (float)output.height;
			rectTransform.sizeDelta = new Vector2(output.width * aspect, output.height * aspect);
			WebCameraSurface.texture = output;
			webCamTexture.IncrementUpdateCount();
		}
	}

	protected abstract Mat ImageProcessing(Mat input);
}
