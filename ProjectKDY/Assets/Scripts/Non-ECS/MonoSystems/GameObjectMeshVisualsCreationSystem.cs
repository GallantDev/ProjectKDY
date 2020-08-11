using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectMeshVisualsCreationSystem : MonoGameSystem {
    [SerializeField]
    private List<GameObjectMeshVisualsCreationCall> meshCalls = new List<GameObjectMeshVisualsCreationCall>();

    public List<GameObjectMeshVisualsCreationCall> MeshCalls {
        get {
            return meshCalls;
        }

        set {
            meshCalls = value;
        }
    }

    public override void Init() {
        print("Visual Creation System: Init!");
    }

    public override IEnumerator Execute() {
        print("Visual Creation System: Execute!");
        for (int i = 0; i < meshCalls.Count; i++) {
            GenerateGameObjectVisuals(i);
        }
        yield break;
    }

    private void GenerateGameObjectVisuals(int entIndex) {
        print("Generating visuals!");
        GameObjectMeshVisuals tmpVis = SystemEntities[entIndex].AddEntityComponent(typeof(GameObjectMeshVisuals)) as GameObjectMeshVisuals;
        tmpVis.VisualsObj = new GameObject("MeshVisuals");
        tmpVis.VisualsObj.transform.position = SystemEntities[entIndex].transform.position;
        tmpVis.VisualsObj.transform.parent = SystemEntities[entIndex].transform;
        tmpVis.ObjMeshFilter = SystemEntities[entIndex].AddEntityComponent(typeof(MeshFilter), tmpVis.VisualsObj) as MeshFilter;
        tmpVis.ObjMeshRend = SystemEntities[entIndex].AddEntityComponent(typeof(MeshRenderer), tmpVis.VisualsObj) as MeshRenderer;
        tmpVis.ObjMeshScale = meshCalls[entIndex].VisData.VisMeshScale;
        AddVisualElementsAndScale(tmpVis, meshCalls[entIndex].VisData);
        SystemEntities[entIndex].RemoveEntityComponent(meshCalls[entIndex]);
        meshCalls.RemoveAt(entIndex);
        SystemEntities.RemoveAt(entIndex);
    }

    private void AddVisualElementsAndScale(GameObjectMeshVisuals vis, GameObjectMeshVisualsData visData) {
        vis.ObjMeshFilter.mesh = Resources.Load<Mesh>(visData.VisMeshPath);
        vis.ObjMeshRend.material = Resources.Load<Material>(visData.VisMatPath);
        vis.VisualsObj.transform.localScale = visData.VisMeshScale;
        vis.VisualsObj.transform.position += (Vector3.up * (visData.VisMeshScale.y / 2.0f));
    }
}