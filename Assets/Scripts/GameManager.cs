using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }

	public UnityEvent<Vector2> OnMazeMove { get; private set; } = new UnityEvent<Vector2>();

	void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	void Start()  {
		Cursor.lockState = CursorLockMode.Locked;
	}

	public static void InvokeMazeMove(Vector2 deltaPos) {
		Instance.OnMazeMove.Invoke(deltaPos);
	}
}