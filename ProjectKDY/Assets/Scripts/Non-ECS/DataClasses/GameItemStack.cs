using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameItemStack {
    [SerializeField]
    private GameItemData itemData;
    [SerializeField]
    private int itemCount;

    public GameItemData ItemData {
        get {
            return itemData;
        }

        set {
            itemData = value;
        }
    }

    public int ItemCount {
        get {
            return itemCount;
        }

        set {
            itemCount = value;
        }
    }

    public GameItemStack() {

    }

    public GameItemStack(GameItemData gItem, int count) {
        itemData = gItem;
        itemCount = count;
    }
}