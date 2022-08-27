using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CullObject : MonoBehaviour
{
    public MeshRenderer Renderer { get; private set; }

    void Awake() {
        Renderer = GetComponent<MeshRenderer>();
    }

    void Update() {
        Renderer.enabled = false;
    }
}
