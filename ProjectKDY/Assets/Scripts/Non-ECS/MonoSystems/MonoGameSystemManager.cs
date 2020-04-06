using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoGameSystemManager : MonoBehaviour {
    [SerializeField]
    private float tickLength = 0.0f;
    private List<MonoGameSystem> gameSystems;

    // Start is called before the first frame update
    private void Start() {
        Init();
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void Init() {
        gameSystems = new List<MonoGameSystem>();
        gameSystems.AddRange(FindObjectsOfType<MonoGameSystem>());
        InitSystems();
        StartCoroutine(TickSystems());
    }

    private void InitSystems() {
        for (int i = 0; i < gameSystems.Count; i++) {
            gameSystems[i].Init();
        }
    }

    private IEnumerator TickSystems() {
        yield return new WaitForSeconds(tickLength);
        for (int i = 0; i < gameSystems.Count; i++) {
            gameSystems[i].Tick();
        }
        StartCoroutine(TickSystems());
        yield break;
    }
}