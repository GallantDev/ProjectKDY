using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemCreationCall : MonoComponent {
    [SerializeField] private GameItemData itemData;

    public GameItemData ItemData {
        get {
            return itemData;
        }

        set {
            itemData = value;
        }
    }
}