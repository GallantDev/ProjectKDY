using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectMeshVisualsCreationCall : MonoComponent {
    private GameObjectMeshVisualsData visData;

    public GameObjectMeshVisualsData VisData {
        get {
            return visData;
        }

        set {
            visData = value;
        }
    }

    public GameObjectMeshVisualsCreationCall() {
        visData = new GameObjectMeshVisualsData();
    }
}