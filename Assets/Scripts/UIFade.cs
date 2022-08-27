
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : MonoBehaviour {
    [field: SerializeField] public float LerpSpeed { get; private set; } = 3;
    [field: SerializeField] public UnityEvent OnFadedIn { get; private set; }

    public CanvasGroup CanvasGroup { get; private set; }

    void Awake() {
        CanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 0;
    }

    void OnEnable() {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn() {
        while (CanvasGroup.alpha < 0.95) {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 1, LerpSpeed * Time.deltaTime);
            yield return null;
        }

        CanvasGroup.alpha = 1;
        OnFadedIn.Invoke();
    }

    void OnDisable() {
        CanvasGroup.alpha = 0;
    }
}
