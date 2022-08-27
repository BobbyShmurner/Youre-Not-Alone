using UnityEngine;

public class AudioPoolPlayer : MonoBehaviour {
	[field: SerializeField] public AudioPool Pool { get; private set; }
	[field: SerializeField] public float Volume { get; private set; }
	[field: SerializeField] public float Speed { get; private set; }

	float timeSinceLastFinished;
	AudioSource latestSource;
	AudioClip lastClip;

	void Update() {
		if (ShouldPlayClip()) latestSource = PlayClip();
	}

	protected virtual bool ShouldPlayClip() {
		if (latestSource) {
			timeSinceLastFinished = Time.realtimeSinceStartup;
		}

		return latestSource == null && Time.realtimeSinceStartup > timeSinceLastFinished + Speed;
	}

	public AudioSource PlayClip() {
		AudioSource source =  AudioManager.PlayFromPool(Pool, Volume, transform, lastClip);
		lastClip = source.clip;
		
		return source;
	}
}