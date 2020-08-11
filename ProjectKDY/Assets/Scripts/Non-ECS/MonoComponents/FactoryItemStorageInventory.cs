using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemStorageInventory : MonoComponent {
    [SerializeField]
    private List<GameItemStack> itemStacks = new List<GameItemStack>();

    public List<GameItemStack> ItemStacks {
        get {
            return itemStacks;
        }

        set {
            itemStacks = value;
        }
    }
}