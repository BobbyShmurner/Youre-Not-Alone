using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
    [field: SerializeField] public Transform Target { get; private set; }

    void LateUpdate() {
        transform.LookAt(Target);
    }
}
