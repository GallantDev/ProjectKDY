using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemSlottedInventory : MonoComponent {

    [SerializeField]
    private bool isInitialized = false;
    [SerializeField]
    private List<WorldGameItemSlot> itemSlots = new List<WorldGameItemSlot>();

    public bool IsInitialized {
        get {
            return isInitialized;
        }

        set {
            isInitialized = value;
        }
    }

    public List<WorldGameItemSlot> ItemSlots {
        get {
            return itemSlots;
        }

        set {
            itemSlots = value;
        }
    }
}