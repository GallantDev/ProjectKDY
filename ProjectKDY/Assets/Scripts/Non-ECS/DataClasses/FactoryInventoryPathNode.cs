using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInventoryPathNode {
    private MonoEntity conveyorEnt = null;
    private int distFromEnd = 0;
    private float angleDiff = 0.0f;

    public MonoEntity ConveyorEnt {
        get {
            return conveyorEnt;
        }

        set {
            conveyorEnt = value;
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

    public FactoryInventoryPathNode(MonoEntity ent, int dist, float angleDifference) {
        conveyorEnt = ent;
        distFromEnd = dist;
        angleDiff = angleDifference;
    }
}
