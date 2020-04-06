using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorPathNode {
    private ConveyorBelt conveyor = null;
    private int distFromEnd = 0;
    private float angleDiff = 0.0f;

    public ConveyorBelt Conveyor {
        get {
            return conveyor;
        }

        set {
            conveyor = value;
        }
    }

    public int DistFromEnd {
        get {
            return distFromEnd;
        }
        set {
            distFromEnd = value;
        }
    }

    public float AngleDiff {
        get {
            return angleDiff;
        }

        set {
            angleDiff = value;
        }
    }

    public ConveyorPathNode(ConveyorBelt belt, int dist, float angleDifference) {
        conveyor = belt;
        distFromEnd = dist;
        angleDiff = angleDifference;
    }
}
