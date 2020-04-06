using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameItemSlot {
    private WorldGameItem slotItem;
    private Vector3 itemWorldPos;

    public WorldGameItem SlotItem {
        get {
            return slotItem;
        }

        set {
            slotItem = value;
        }
    }

    public Vector3 WorldPosition {
        get {
            return itemWorldPos;
        }

        set {
            itemWorldPos = value;
        }
    }

    public WorldGameItemSlot(WorldGameItem item) {
        slotItem = item;
    }

    public WorldGameItemSlot(Vector3 pos) {
        itemWorldPos = pos;
    }

    public WorldGameItemSlot(WorldGameItem item, Vector3 pos) {
        slotItem = item;
        itemWorldPos = pos;
    }
}
