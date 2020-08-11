using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameItem : MonoComponent {
    [SerializeField]
    private GameItemData itemData;

    public GameItemData ItemData {
        get {
            return itemData;
        }

        set {
            itemData = value;
        }
    }

    //public GameItem() {
    //    itemData = new GameItemData();
    //}
}