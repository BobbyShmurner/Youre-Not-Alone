using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
	[field: SerializeField] public GameObject Environment { get; private set; }
	[field: SerializeField] public GameObject GameOverUI { get; private set; }
	[field: SerializeField] public PlayerController Player { get; private set; }
	[field: SerializeField] public Material RenderMat { get; private set; }

	public static GameManager Instance { get; private set; }
	public static UnityEvent<Vector2> OnMazeMove { get; private set; } = new UnityEvent<Vector2>();

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
		OnMazeMove.Invoke(deltaPos);
	}

	public static void EndGame() {
		foreach (DemonController demon in GameObject.FindObjectsOfType<DemonController>()) {
			demon.gameObject.SetActive(false);
		}

		foreach (AudioSource audioSource in GameObject.FindObjectsOfType<AudioSource>()) {
			audioSource.Stop();
		}

		Cursor.lockState = CursorLockMode.None;

		Instance.RenderMat.SetFloat("_ScanlinesStrength", 0);
		Instance.RenderMat.SetFloat("_TintStrength", 0);

		Instance.Player.enabled = false;
		Instance.GameOverUI.SetActive(true);
		Instance.Environment.SetActive(false);
		Instance.Player.WalkAudioPlayer.enabled = false;
		Instance.Player.StaminaAnimationController.Slider.gameObject.SetActive(false);
	}

	public static void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public static void MainMenu() {
		SceneManager.LoadScene(0);
	}
}