using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFloatComp : MonoComponent {
    [SerializeField]
    private float testFloat = 0.0f;

    public float TestFloat {
        get {
            return testFloat;
        }

        set {
            testFloat = value;
        }
    }
}