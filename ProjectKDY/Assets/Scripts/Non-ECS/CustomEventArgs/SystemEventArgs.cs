using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemEventArgs : EventArgs {
    private MonoGameSystem gameSystem;

    public MonoGameSystem GameSystem {
        get {
            return gameSystem;
        }

        set {
            gameSystem = value;
        }
    }
}