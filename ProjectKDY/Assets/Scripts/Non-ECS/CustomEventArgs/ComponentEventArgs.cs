using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentEventArgs : EventArgs {
    private MonoComponent comp;

    public MonoComponent Comp {
        get {
            return comp;
        }

        set {
            comp = value;
        }
    }

    public ComponentEventArgs(MonoComponent monoComp) {
        comp = monoComp;
    }
}