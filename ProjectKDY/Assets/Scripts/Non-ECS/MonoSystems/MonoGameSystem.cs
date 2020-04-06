using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoGameSystem : MonoBehaviour {
    public int tickThreshold = 0;
    public int tickCount = 0;

    public virtual void Init() {}

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