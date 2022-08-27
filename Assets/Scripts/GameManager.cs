using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
	[field: SerializeField] public GameObject Environment { get; private set; }
	[field: SerializeField] public GameObject GameOverUI { get; private set; }
	[field: SerializeField] public PlayerController Player { get; private set; }
	[field: SerializeField] public Material RenderMat { get; private set; }

	public static GameManager Instance { get; private set; }
	public static MainInput Input { get; private set; }

	public static UnityEvent<Vector2> OnMazeMove { get; private set; } = new UnityEvent<Vector2>();
	public static UnityEvent OnPause { get; private set; } = new UnityEvent();
	public static UnityEvent OnUnpause { get; private set; } = new UnityEvent();

	public static bool IsPaused { get; private set; }
	public static bool IsPlayerCameraLocked { get; private set; }

	void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		Input = new MainInput();
	}

	void Start()  {
		Input.General.Pause.performed += (ctx) => { 
			if (!IsPaused) Pause();
			else Unpause();
		};

		SetInputActive(true); 
		SetPlayerCameraLock(false);
	}

	void OnEnable() { SetInputActive(true); }
	void OnDisable() { SetInputActive(false); }

	public static void SetInputActive(bool active) {
		if (active) Input.Enable();
		else Input.Disable();
	}

	public static void InvokeMazeMove(Vector2 deltaPos) {
		OnMazeMove.Invoke(deltaPos);
	}

	public static void Pause() {
		SetPlayerCameraLock(true);
		
		IsPaused = true;
		Time.timeScale = 0;
		OnPause.Invoke();
	}

	public static void Unpause() {
		SetPlayerCameraLock(false);

		IsPaused = false;
		Time.timeScale = 1;
		OnUnpause.Invoke();
	}

	public static void SetPlayerCameraLock(bool locked) {
		Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
		IsPlayerCameraLocked = locked;
	}

	public static void EndGame() {
		foreach (DemonController demon in GameObject.FindObjectsOfType<DemonController>()) {
			demon.gameObject.SetActive(false);
		}

		foreach (AudioSource audioSource in GameObject.FindObjectsOfType<AudioSource>()) {
			audioSource.Stop();
		}

		SetPlayerCameraLock(true);

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