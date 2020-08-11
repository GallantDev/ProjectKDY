using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInventoryPath {
    private List<FactoryInventoryPathNode> nodePath;

    public List<FactoryInventoryPathNode> NodePath {
        get {
            return nodePath;
        }

        set {
            nodePath = value;
        }
    }

    public FactoryInventoryPath(FactoryInventoryPathNode pathNode) {
        nodePath = new List<FactoryInventoryPathNode>(1);
        nodePath.Add(pathNode);
    }

    public FactoryInventoryPath(List<FactoryInventoryPathNode> path) {
        nodePath = new List<FactoryInventoryPathNode>(path.Count);
        nodePath.AddRange(path);
    }
}