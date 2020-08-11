using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldGameItemSlot {
    [SerializeField]
    private GameItem worldGameItem;
    [SerializeField]
    private Vector3 itemWorldPos;


    public GameItem WorldGameItem {
        get {
            return worldGameItem;
        }

        set {
            worldGameItem = value;
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

    public WorldGameItemSlot(Vector3 pos) {
        itemWorldPos = pos;
    }

    public WorldGameItemSlot(GameItem gameItem, Vector3 pos) {
        worldGameItem = gameItem;
        itemWorldPos = pos;
    }
}