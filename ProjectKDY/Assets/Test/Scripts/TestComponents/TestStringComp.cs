using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStringComp : MonoComponent {
    [SerializeField]
    private string testString = "";

    public string TestString {
        get {
            return testString;
        }

        set {
            testString = value;
        }
    }
}