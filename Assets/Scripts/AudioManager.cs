using System.Linq;

using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [field: SerializeField] public GameObject AudioSourcePrefab { get; private set; }

	void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

    public static AudioSource PlayFromPool(AudioPool pool, float volume, Transform parent = null, AudioClip lastClip = null) {
        return PlayFromPool(pool, volume, Vector3.zero, parent, lastClip);
    }

    public static AudioSource PlayFromPool(AudioPool pool, float volume, Vector3 localPos, Transform parent = null, AudioClip lastClip = null) {
        AudioClip clipToPlay;
        
        do {
            clipToPlay = pool.Clips[Random.Range(0, pool.Clips.Count)];
        } while (clipToPlay == lastClip);

        return PlayAudio(clipToPlay, volume, localPos, parent);
    }

    public static AudioSource PlayAudio(AudioClip clip, float volume, Transform parent = null) {
        return PlayAudio(clip, volume, Vector3.zero, parent);
    }

    public static AudioSource PlayAudio(AudioClip clip, float volume, Vector3 localPos, Transform parent = null) {
        if (parent == null) parent = Instance.transform;

        AudioSource source = GameObject.Instantiate(Instance.AudioSourcePrefab, parent).GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();

        return source;
    }
}
