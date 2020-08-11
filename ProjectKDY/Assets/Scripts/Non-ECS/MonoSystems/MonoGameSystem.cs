using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoGameSystem : MonoBehaviour {
    [SerializeField]
    private int tickThreshold = 0;
    [SerializeField]
    private int tickCount = 0;
    [SerializeField]
    private List<MonoEntity> systemEntities = new List<MonoEntity>();

    public int TickThreshold {
        get {
            return tickThreshold;
        }

        set {
            tickThreshold = value;
        }
    }

    public int TickCount {
        get {
            return tickCount;
        }

        set {
            tickCount = value;
        }
    }

    public List<MonoEntity> SystemEntities {
        get {
            return systemEntities;
        }

        set {
            systemEntities = value;
        }
    }

    public int GetSystemEntityIndex(Component comp) {
        Component tmpComp;
        for (int i = 0; i < systemEntities.Count; i++) {
            tmpComp = systemEntities[i].GetComponent(comp.GetType());
            if (tmpComp == comp) {
                return i;
            }
        }
        throw new System.Exception("Can't check for entity index! System does not contain entity!");
    }

    public int GetSystemEntityIndex(MonoEntity ent) {
        for (int i = 0; i < systemEntities.Count; i++) {
            if (systemEntities[i] == ent) {
                return i;
            }
        }
        throw new System.Exception("Can't check for entity index! System does not contain entity!");
    }

    public virtual void Init() {

    }

    public virtual void Tick() {
        print("System Tick!");
        tickCount++;
        if (tickCount >= tickThreshold) {
            tickCount = 0;
            StartCoroutine(Execute());
        }
    }

    public virtual IEnumerator Execute() {
        Debug.LogWarning("Game system had an empty game tick execute phase!");
        yield break;
    }
}