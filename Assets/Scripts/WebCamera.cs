namespace OpenCvSharp.Demo
{
	using OpenCvSharp;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(RawImage))]
	public class WebCamera : MonoBehaviour
	{
		[SerializeField]
		private bool UseMinAspect = true;

		private RawImage rawImage;
		private RectTransform rectTransform;

		private uint WebCamDeviceIndex = 0;
		private WebCamDevice[] CameraDevices;
		private WebCamTexture activeCameraTexture;

		private Vector2 LastScreenSize;


		public static WebCamera Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
			}
			else
			{
				Instance = this;
			}

			rectTransform = GetComponent<RectTransform>();
			rawImage = GetComponent<RawImage>();
			LastScreenSize = Vector2.zero;
			CameraDevices = WebCamTexture.devices;
			activeCameraTexture = new WebCamTexture();
		}

		private void Start()
		{
			if (WebCamTexture.devices.Length == 0)
			{
				Debug.Log("No camera found");
				return;
			}

			activeCameraTexture.deviceName = CameraDevices[WebCamDeviceIndex].name;
			rawImage.texture = activeCameraTexture;
			activeCameraTexture.Play();
		}

		private void Update()
		{
			UpdateScreenSize();

			if (activeCameraTexture != null && activeCameraTexture.didUpdateThisFrame)
			{
				Mat img = Unity.TextureToMat(activeCameraTexture);


				//activeCameraTexture = Unity.MatToTexture(img, rawImage.texture);
			}
		}

		private void UpdateScreenSize()
		{

			if (LastScreenSize.x != Screen.width || LastScreenSize.y != Screen.height)
			{
				LastScreenSize = new Vector2(Screen.width, Screen.height);
				float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)activeCameraTexture.width, Screen.height / (float)activeCameraTexture.height) : Mathf.Max(Screen.width / (float)activeCameraTexture.width, Screen.height / (float)activeCameraTexture.height);
				rectTransform.sizeDelta = new Vector2(activeCameraTexture.width * aspect, activeCameraTexture.height * aspect);
			}
		}

		[ContextMenu("Change Camera")]
		private void NextCamera()
		{
			activeCameraTexture.Stop();
			if (WebCamDeviceIndex >= WebCamTexture.devices.Length - 1)
			{
				WebCamDeviceIndex = 0;
				activeCameraTexture.deviceName = CameraDevices[WebCamDeviceIndex].name;
			}
			else
			{
				WebCamDeviceIndex++;
				activeCameraTexture.deviceName = CameraDevices[WebCamDeviceIndex].name;
			}
			activeCameraTexture.Play();
			LastScreenSize = Vector2.zero;
			UpdateScreenSize();
		}
	}
}