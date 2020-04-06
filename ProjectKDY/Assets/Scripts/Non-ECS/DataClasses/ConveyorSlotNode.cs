using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSlotNode {
    private ConveyorBelt conveyor;
    private WorldGameItemSlot itemSlot;

    public ConveyorBelt Conveyor {
        get {
            return conveyor;
        }

        set {
            conveyor = value;
        }
    }

    public WorldGameItemSlot ItemSlot {
        get {
            return itemSlot;
        }

        set {
            itemSlot = value;
        }
    }
}
