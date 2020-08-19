using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemProductionInventory : MonoComponent {
    [SerializeField]
    private List<GameItemStack> inputItemStacks = new List<GameItemStack>();
    [SerializeField]
    private List<GameItemStack> outputItemStacks = new List<GameItemStack>();

    public List<GameItemStack> InputItemStacks {
        get {
            return inputItemStacks;
        }

        set {
            inputItemStacks = value;
        }
    }

    public List<GameItemStack> OutputItemStacks {
        get {
            return outputItemStacks;
        }

        set {
            outputItemStacks = value;
        }
    }

    public int GetItemTypeCount(int id) {
        for (int i = 0; i < inputItemStacks.Count; i++) {
            if (inputItemStacks[i].ItemData.Id == id) {
                return inputItemStacks[i].ItemCount;
            }
        }
        return 0;
    }

    public int GetTotalItemCount() {
        int tmpInt = 0;
        for (int i = 0; i < inputItemStacks.Count; i++) {
            tmpInt += inputItemStacks[i].ItemCount;
        }
        return tmpInt;
    }
}