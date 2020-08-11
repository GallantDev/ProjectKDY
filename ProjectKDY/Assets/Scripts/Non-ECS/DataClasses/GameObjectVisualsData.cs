using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectMeshVisualsData {
    private string visMeshPath;
    private Vector3 visMeshScale;
    private string visMatPath;

    public string VisMeshPath {
        get {
            return visMeshPath;
        }

        set {
            visMeshPath = value;
        }
    }

    public Vector3 VisMeshScale {
        get {
            return visMeshScale;
        }

        set {
            visMeshScale = value;
        }
    }

    public string VisMatPath {
        get {
            return visMatPath;
        }

        set {
            visMatPath = value;
        }
    }

    public GameObjectMeshVisualsData() {

    }

    public GameObjectMeshVisualsData(string meshPath, Vector3 meshScale, string materialPath) {
        visMeshPath = meshPath;
        visMeshScale = meshScale;
        visMatPath = materialPath;
    }
}
