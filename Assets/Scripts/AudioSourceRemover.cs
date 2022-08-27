using UnityEngine;

using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceRemover : MonoBehaviour {
	public AudioSource AudioSource { get; private set; }

	void Awake() {
		AudioSource = GetComponent<AudioSource>();
	}

	void Start() {
		StartCoroutine(RemoveClipOnFinish());
	}

	IEnumerator RemoveClipOnFinish() {
		while (AudioSource.isPlaying) {
			yield return null;
		}

		Destroy(gameObject);
	}
}