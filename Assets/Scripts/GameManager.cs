using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance { private set; get; }
	public MarkerObject SelectedMarker;
	public bool clickReset = true;

	private void Awake() {
		if(instance != null) {
			Destroy(gameObject);
		} else {
			instance = this;
		}
	}

	private void Update() {
		if(Input.GetAxisRaw("Fire1") > 0 && clickReset) {
			clickReset = false;

			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				if(hit.transform.gameObject.layer != 5) {
					Transform hitParent = hit.transform.parent;
					if(hitParent != null) {
						MarkerObject markerObject = hitParent.GetComponent<MarkerObject>();
						if(markerObject != null) {
							if(markerObject == SelectedMarker) {
								SelectedMarker.Deselect();
								SelectedMarker = null;
							} else if(markerObject != null) {
								if(SelectedMarker != null) {
									SelectedMarker.Deselect();
									SelectedMarker = null;
								}
								SelectedMarker = markerObject.Select();
							}
						}
					}
				}

			}
		} else if(Input.GetAxisRaw("Fire1") == 0 && !clickReset) {
			clickReset = true;
		}
	}
}
