namespace OpenCvSharp.Demo
{
	using OpenCvSharp;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public class WebCamera : WebCameraHandler
    {
        private RawImage rawImage;
        protected override void Awake()
        {
            base.Awake();
            rawImage = gameObject.GetComponent<RawImage>();
        }
        protected override void WebCamInput(WebCamTexture input)
        {
            rawImage.texture = input;
        }
    }
}