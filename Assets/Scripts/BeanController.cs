using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanController : MonoBehaviour {
    void Start() {
        gameObject.SetActive(Debug.isDebugBuild);
    }
}
