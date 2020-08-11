using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectMeshVisuals : MonoComponent{
    [SerializeField]
    private GameObject visualsObj;
    [SerializeField]
    private MeshFilter objMeshFilter;
    [SerializeField]
    private MeshRenderer objMeshRend;
    [SerializeField]
    private Vector3 objMeshScale;

    public GameObject VisualsObj {
        get {
            return visualsObj;
        }

        set {
            visualsObj = value;
        }
    }

    public MeshFilter ObjMeshFilter {
        get {
            return objMeshFilter;
        }

        set {
            objMeshFilter = value;
        }
    }

    public MeshRenderer ObjMeshRend {
        get {
            return objMeshRend;
        }

        set {
            objMeshRend = value;
        }
    }

    public Vector3 ObjMeshScale {
        get {
            return objMeshScale;
        }

        set {
            objMeshScale = value;
        }
    }
}