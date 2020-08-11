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

    public List<GameItemStack> OutputItems {
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
}
