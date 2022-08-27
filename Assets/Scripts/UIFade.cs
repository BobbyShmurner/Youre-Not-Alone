
using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : MonoBehaviour {
    [field: SerializeField] public bool FadeInOnEnable { get; private set; } = true;
    [field: SerializeField] public bool FadeOutOnEnable { get; private set; } = false;
    [field: SerializeField] public bool DisableOnFadeOut { get; private set; } = true;
    [field: SerializeField] public float LerpSpeed { get; private set; } = 3;
    [field: SerializeField] public float Delay { get; private set; }
    [field: SerializeField] public UnityEvent OnFadedIn { get; private set; }
    [field: SerializeField] public UnityEvent OnFadedOut { get; private set; }

    public CanvasGroup CanvasGroup { get; private set; }

    void Awake() {
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable() {
        if (FadeInOnEnable) FadeIn();
        else if (FadeOutOnEnable) FadeOut();
    }

    public void FadeIn() {
        gameObject.SetActive(true);
        CanvasGroup.alpha = 0;
        
        StartCoroutine(Fade(1));
    }

    public void FadeOut() {
        gameObject.SetActive(true);
        CanvasGroup.alpha = 1;

        StartCoroutine(Fade(0));
    }

    IEnumerator Fade(float target) {
        if (Delay != 0) yield return new WaitForSeconds(Delay);

        while (Mathf.Abs(CanvasGroup.alpha - target) > 0.05) {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, target, LerpSpeed * Time.deltaTime);
            yield return null;
        }

        CanvasGroup.alpha = target;

        if (target == 1) {
            OnFadedIn.Invoke();
        } else {
            OnFadedOut.Invoke();
            if (DisableOnFadeOut) gameObject.SetActive(false);
        }
    }
}
