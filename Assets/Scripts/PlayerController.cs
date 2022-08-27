using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MazeObject {
	[field: SerializeField] public MazeController MazeController { get; private set; }

	[field: SerializeField] public float MouseSense { get; private set; } = 0.1f;
	[field: SerializeField] public float MovementSpeed { get; private set; } = 5f;
	[field: SerializeField] public float SprintMaxSpeed { get; private set; } = 10f;
	[field: SerializeField] public float CrouchSpeedMultiplier { get; private set; } = 0.55f;
	[field: SerializeField] public float CrouchHeightMultiplier { get; private set; } = 0.25f;
	[field: SerializeField] public float CrouchLerpSpeed { get; private set; } = 0.25f;
	[field: SerializeField] public float StartingStamina { get; private set; } = 3f;
	[field: SerializeField] public float StaminaRechargeRate { get; private set; } = 0.5f;
	[field: SerializeField] public float TiredSpeedMultiplier { get; private set; } = 0.2f;

	public StaminaAnimationController StaminaAnimationController { get; private set; }
	public PlayerWalkAudioPlayer WalkAudioPlayer { get; private set; }
	public Rigidbody Rigidbody { get; private set; }
	public MainInput Input { get; private set; }
	public Camera Camera { get; private set; }

	public bool StaminaDepleted { get; private set; }
	public bool Sprinting { get; private set; }
	public bool Crouching { get; private set; }
	public float Stamina { get; private set; }

	public UnityAction onStartSprinting;
	public UnityAction onStopSprinting;
	public UnityAction onStaminaDepleted;
	public UnityAction onStaminaReplenished;

	Vector2 movementDir;
	Vector2 lookDelta;
	Vector3 lastPos = Vector3.zero;

	float camXRot;
	float baseHeight;

	IEnumerator CrouchCoro() {
		float targetHeight = baseHeight * CrouchHeightMultiplier;
		if (!Crouching) targetHeight = baseHeight;

		while (true) {
			float newHeight = Mathf.Lerp(transform.localScale.y, targetHeight, Time.deltaTime * CrouchLerpSpeed);
			float difference = newHeight - transform.localScale.y;

			Vector3 scale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

			transform.localScale = scale;
			transform.position += new Vector3(0, difference, 0);

			if (Mathf.Abs(transform.localScale.y - targetHeight) < 0.05f) break;

			yield return null;
		}
	}

	public void Awake() {
		StaminaAnimationController = GetComponentInChildren<StaminaAnimationController>();
		WalkAudioPlayer = GetComponentInChildren<PlayerWalkAudioPlayer>();
		Camera = GetComponentInChildren<Camera>();
		Rigidbody = GetComponent<Rigidbody>();

		baseHeight = transform.localScale.y;
		Stamina = StartingStamina;

		Input = new MainInput();

		Input.Player.Move.performed += (ctx) => { movementDir = ctx.ReadValue<Vector2>(); };
		Input.Player.Move.canceled += (ctx) => { movementDir = Vector2.zero; };

		Input.Player.Look.performed += (ctx) => { lookDelta = ctx.ReadValue<Vector2>(); };
		Input.Player.Look.canceled += (ctx) => { lookDelta = Vector2.zero; };

		Input.Player.Sprint.performed += (ctx) => { SetSprinting(true); };
		Input.Player.Sprint.canceled += (ctx) => { SetSprinting(false); };

		Input.Player.Crouch.performed += (ctx) => { SetCrouching(true); };
		Input.Player.Crouch.canceled += (ctx) => { SetCrouching(false); };

		SetInputActive(true);
	}

	protected override void Start() {
		base.Start();

		while (MazeController.IsCellOccupied(WorldPosition)) {
			MazeController.RegenerateAllChunks();
		}
	}

	void FixedUpdate() {
		Move();
	}

	void Update() {
		Look();
		UpdateStamina();
	}

	protected override void LateUpdate() {
		base.LateUpdate() ;
		MoveChunks();
	}

	void MoveChunks() {
		Vector2 deltaPos = Vector2.zero;

		if (Mathf.Abs(transform.localPosition.x) > MazeController.ChunkSize.x * 0.5f) {
			deltaPos.x -= transform.localPosition.x * 2f - (transform.localPosition.x * 2f % MazeController.ChunkSize.x);
		}
		if (Mathf.Abs(transform.localPosition.z) > MazeController.ChunkSize.y * 0.5f) {
			deltaPos.y -= transform.localPosition.z * 2f - (transform.localPosition.z * 2f % MazeController.ChunkSize.y);
		}

		if (deltaPos == Vector2.zero) return;

		GameManager.InvokeMazeMove(deltaPos);
		MazeController.MoveChunks(Mathf.FloorToInt(deltaPos.x / MazeController.ChunkSize.x), Mathf.FloorToInt(deltaPos.y / MazeController.ChunkSize.y));
	}

	void UpdateStamina() {
		if (Sprinting) {
			Stamina -= Time.deltaTime;

			if (Stamina <= 0f) {
				Stamina = 0;
				StaminaDepleted = true;
				onStaminaDepleted.Invoke();
				SetSprinting(false);
			}
		} else if (Stamina < StartingStamina) {
			Stamina += Time.deltaTime * StaminaRechargeRate;

			if (Stamina >= StartingStamina) {
				Stamina = StartingStamina;
				StaminaDepleted = false;
				onStaminaReplenished.Invoke();
			}
		}
	}

	void Move() {
		float currentMoveSpeed = (Sprinting ? SprintMaxSpeed : MovementSpeed);
		if (Crouching) currentMoveSpeed *= CrouchSpeedMultiplier;
		if (StaminaDepleted) currentMoveSpeed *= TiredSpeedMultiplier;

		Vector3 newVelocity = transform.forward * movementDir.y + transform.right * movementDir.x;
		newVelocity *= currentMoveSpeed;

		Rigidbody.velocity = new Vector3(newVelocity.x, Rigidbody.velocity.y, newVelocity.z);
		Rigidbody.angularVelocity = Vector3.zero;
	}

	void Look() {
		transform.Rotate(new Vector3(0, lookDelta.x * MouseSense, 0));

		camXRot = Mathf.Clamp(camXRot - lookDelta.y * MouseSense, -89.9f, 89.9f);
		Camera.transform.localEulerAngles = new Vector3(camXRot, 0, 0);
	}

	void OnEnable() { SetInputActive(true); }
	void OnDisable() { SetInputActive(false); }

	void SetInputActive(bool active) {
		if (active) Input.Enable();
		else Input.Disable();
	}

	public void SetCrouching(bool active) {
		Crouching = active;
		StopCoroutine("CrouchCoro");
		StartCoroutine("CrouchCoro");
	}

	public void SetSprinting(bool active) {
		if (active) {
			if (!StaminaDepleted) {
				Sprinting = true;
				onStartSprinting.Invoke();
			}
		} else {
			Sprinting = false;
			onStopSprinting.Invoke();
		}
		
	}
}