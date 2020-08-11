using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemCreationSystem : MonoGameSystem {
    private List<GameItemCreationCall> creationCalls = new List<GameItemCreationCall>();

    public override void Init() {
        
    }

    public override IEnumerator Execute() {
        return null;
    }

    //private void CreateGameItems() {
    //    GameObject callObj;
    //    for (int i = 0; i < creationCalls.Count; i++) {
    //        callObj = creationCalls[i].gameObject;
    //        callObj.AddComponent<GameItem>().ItemData = creationCalls[i].ItemData;
    //        callObj.AddComponent<GameObjectVisualsCreationCall>();
    //        Destroy(callObj);
    //    }
    //}
}