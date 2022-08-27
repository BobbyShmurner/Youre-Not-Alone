using UnityEngine;

public class EyeToggle : MonoBehaviour {
	GameObject target;

	void Start() {
		target = transform.GetChild(0).gameObject;
	}
	public void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			target.SetActive(!target.activeSelf);
		}
	}
}