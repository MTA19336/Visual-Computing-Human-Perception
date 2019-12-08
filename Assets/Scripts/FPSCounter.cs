using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour {
	private Text FPSText;
	private float FPSVel, FPS;
	private void Awake() {
		FPSText = GetComponent<Text>();
	}

	private void Update() {
		FPS = Mathf.SmoothDamp(FPS, (1.0f / Time.deltaTime), ref FPSVel, 1);
		FPSText.text = "FPS: " + Mathf.RoundToInt(FPS);
	}

}
