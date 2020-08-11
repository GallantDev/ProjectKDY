using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEventArgs : EventArgs {
    private MonoEntity entity;

    public MonoEntity Entity {
        get {
            return entity;
        }

        set {
            entity = value;
        }
    }

    public EntityEventArgs(MonoEntity ent) {
        entity = ent;
    }
}