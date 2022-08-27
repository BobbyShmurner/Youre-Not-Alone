using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [field: SerializeField] public Material RenderMat { get; private set; }

    void Awake() {
        RenderMat.SetFloat("_ScanlinesStrength", 0.01f);
        RenderMat.SetFloat("_TintStrength", 0.01f);
    }

    public void Play() {
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Application.Quit();
    }
}
