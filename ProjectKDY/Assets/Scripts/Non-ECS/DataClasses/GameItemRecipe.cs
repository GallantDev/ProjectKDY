using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameItemRecipe {
    [SerializeField]
    //<int itemId, int itemCount>
    private List<GameItemStack> inputItemSets = new List<GameItemStack>();
    [SerializeField]
    private List<GameItemStack> outputItemSets = new List<GameItemStack>();
    [SerializeField]
    private float productionTime = 0.0f;
    [SerializeField]
    private int itemOverflowMult = 0;

    public List<GameItemStack> InputItemSets {
        get {
            return inputItemSets;
        }

        set {
            inputItemSets = value;
        }
    }

    public List<GameItemStack> OutputItemSets {
        get {
            return outputItemSets;
        }

        set {
            outputItemSets = value;
        }
    }

    public float ProductionTime {
        get {
            return productionTime;
        }

        set {
            productionTime = value;
        }
    }

    public int ItemOverflowMult {
        get {
            return itemOverflowMult;
        }

        set {
            itemOverflowMult = value;
        }
    }

    public bool IsInputItemType(int itemId) {
        for (int i = 0; i < inputItemSets.Count; i++) {
            if (inputItemSets[i].ItemData.Id == itemId) {
                return true;
            }
        }
        return false;
    }

    public int GetInputItemTypeLimit(int itemId) {
        for (int i = 0; i < inputItemSets.Count; i++) {
            if (inputItemSets[i].ItemData.Id == itemId) {
                return inputItemSets[i].ItemCount * itemOverflowMult;
            }
        }
        return 0;
    }

    public bool IsOutputItemType(int itemId) {
        for (int i = 0; i < outputItemSets.Count; i++) {
            if (outputItemSets[i].ItemData.Id == itemId) {
                return true;
            }
        }
        return false;
    }

    public int GetOutputItemTypeLimit(int itemId) {
        for (int i = 0; i < outputItemSets.Count; i++) {
            if (outputItemSets[i].ItemData.Id == itemId) {
                return outputItemSets[i].ItemCount * itemOverflowMult;
            }
        }
        return 0;
    }
}
