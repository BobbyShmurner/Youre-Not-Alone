using UnityEngine;
using UnityEngine.UI;


public class StaminaAnimationController : MonoBehaviour {
	public PlayerController Player { get; private set; }
	public Animator Animator { get; private set; }
	public Slider Slider { get; private set; }

	void Awake() {
		Player = GetComponentInParent<PlayerController>();
		Animator = GetComponentInParent<Animator>();
		Slider = GetComponentInChildren<Slider>();
	}

	void LateUpdate() {
		Slider.maxValue = Player.StartingStamina;
		Slider.value = Player.Stamina;
	}

	void Start() {
		Player.onStartSprinting += Player_OnStartSprinting;	
		Player.onStopSprinting += Player_OnStopSprinting;	
		Player.onStaminaDepleted += Player_OnStaminaDepleted;	
		Player.onStaminaReplenished += Player_OnStaminaReplenished;	
	}

	void Player_OnStartSprinting() {
		Animator.SetBool("Depleting", true);
	}

	void Player_OnStopSprinting() {
		Animator.SetBool("Depleting", false);
		Animator.SetBool("Replenishing", true);
	}

	void Player_OnStaminaDepleted() {
		Animator.SetBool("Depleted", true);
	}

	void Player_OnStaminaReplenished() {
		Animator.SetBool("Depleted", false);
		Animator.SetBool("Replenishing", false);
	}
}