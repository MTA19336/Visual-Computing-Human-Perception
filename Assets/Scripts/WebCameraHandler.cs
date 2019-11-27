using UnityEngine;

public abstract class WebCameraHandler : MonoBehaviour {
    protected internal uint webCamDeviceIndex = 0;
    protected internal WebCamDevice[] webCamDevices;
    protected internal WebCamTexture webCamTexture;

    protected virtual void Awake() {
        webCamDevices = WebCamTexture.devices;
        webCamTexture = new WebCamTexture();
    }

    protected virtual void Start() {
        if (WebCamTexture.devices.Length == 0) {
            Debug.Log("WebCamDevice not found.");
            return;
        }
        webCamTexture.deviceName = webCamDevices[webCamDeviceIndex].name;
        webCamTexture.Play();
    }

    protected virtual void Update() {
        if (webCamTexture.didUpdateThisFrame)
            CamUpdate(webCamTexture);
    }

    protected abstract void CamUpdate(WebCamTexture input);

    [ContextMenu("Change Web Camera")]
    private void nextWebCamDevice() {
        webCamTexture.Stop();
        if (webCamDeviceIndex >= WebCamTexture.devices.Length - 1) {
            webCamDeviceIndex = 0;
            webCamTexture.deviceName = webCamDevices[webCamDeviceIndex].name;
        } else {
            webCamDeviceIndex++;
            webCamTexture.deviceName = webCamDevices[webCamDeviceIndex].name;
        }
        webCamTexture.Play();
    }
}
