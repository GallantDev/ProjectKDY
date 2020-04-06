using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorPath {
    private List<ConveyorPathNode> nodePath;

    public List<ConveyorPathNode> NodePath {
        get {
            return nodePath;
        }

        set {
            nodePath = value;
        }
    }

    public ConveyorPath(ConveyorPathNode pathNode) {
        nodePath = new List<ConveyorPathNode>(1);
        nodePath.Add(pathNode);
    }

    public ConveyorPath(List<ConveyorPathNode> path) {
        nodePath = new List<ConveyorPathNode>(path.Count);
        nodePath.AddRange(path);
    }
}
