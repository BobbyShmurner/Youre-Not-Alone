using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWalkAudioPlayer : AudioPoolPlayer {
	public PlayerController PlayerController { get; private set; }
	public Rigidbody Rigidbody { get { return PlayerController.Rigidbody; } }

	float lastTimeWalkingClipPlayed;

	void Awake() {
		PlayerController = GetComponent<PlayerController>();
	}

	protected override bool ShouldPlayClip() {
		if (Time.realtimeSinceStartup > lastTimeWalkingClipPlayed + Speed / Rigidbody.velocity.magnitude) {
			lastTimeWalkingClipPlayed = Time.realtimeSinceStartup;
			return true;
		}

		return false;
	}
}