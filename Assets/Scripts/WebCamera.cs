namespace OpenCvSharp
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(RawImage))]
	public class WebCamera : WebCameraHandler
	{
		private RawImage rawImage;
		private RectTransform rectTransform;
		[SerializeField] private bool UseMinAspect;
		protected override void Awake()
		{
			base.Awake();
			rectTransform = gameObject.GetComponent<RectTransform>();
			rawImage = gameObject.GetComponent<RawImage>();
		}
		protected override void CamUpdate(WebCamTexture input)
		{
			float aspect = UseMinAspect ? Mathf.Min(Screen.width / (float)input.width, Screen.height / (float)input.height) : Mathf.Max(Screen.width / (float)input.width, Screen.height / (float)input.height);
			rectTransform.sizeDelta = new Vector2(input.width * aspect, input.height * aspect);

			rawImage.texture = input;

            

        }
	}
}

