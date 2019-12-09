using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour {
	private Text FPSText;
	private float FPSVel, FPS, averageFPS;
    private int totalFrames, currFPS;
	private void Awake() {
		FPSText = GetComponent<Text>();
	}


	private void Update() {
		FPS = Mathf.SmoothDamp(FPS, (1.0f / Time.deltaTime), ref FPSVel, 1);
        currFPS = (int)(1f / Time.unscaledDeltaTime);
        updateAvrageFPS(currFPS);
		FPSText.text = "FPS: " + Mathf.RoundToInt(FPS) + "\n Avg. FPS: "+ averageFPS.ToString();
	}

    private void updateAvrageFPS(float newFPS) {
        ++totalFrames;
        averageFPS += (newFPS - averageFPS) / totalFrames;
    }

}
