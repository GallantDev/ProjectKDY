using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemMovement : MonoComponent {
    [SerializeField]
    private int moveDist = 0;
    private MonoEntity outputConveyorEnt;
    [SerializeField]
    private bool hasOutputInventory = false;

    public int MoveDist {
        get {
            return moveDist;
        }

        set {
            moveDist = value;
        }
    }

    public MonoEntity OutputConveyorEnt {
        get {
            return outputConveyorEnt;
        }

        set {
            outputConveyorEnt = value;
        }
    }

    public bool HasOutputInventory {
        get {
            return hasOutputInventory;
        }

        set {
            hasOutputInventory = value;
        }
    }
}