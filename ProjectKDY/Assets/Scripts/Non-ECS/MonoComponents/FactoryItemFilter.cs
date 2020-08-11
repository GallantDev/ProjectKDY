using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemFilter : MonoComponent {
    [SerializeField]
    private bool canBeAddedTo = false;
    [SerializeField]
    private bool canBeRemovedFrom = false;
    [SerializeField]
    private List<GameItemStack> allowedItemStackDatas = new List<GameItemStack>();
    [SerializeField]
    private int maxItemCount = 0;

    public bool CanBeAddedTo {
        get {
            return canBeAddedTo;
        }

        set {
            canBeAddedTo = value;
        }
    }

    public bool CanBeRemovedFrom {
        get {
            return canBeRemovedFrom;
        }

        set {
            canBeRemovedFrom = value;
        }
    }

    public List<GameItemStack> AllowedItemStackDatas {
        get {
            return allowedItemStackDatas;
        }

        set {
            allowedItemStackDatas = value;
        }
    }

    public int MaxItemCount {
        get {
            return maxItemCount;
        }

        set {
            maxItemCount = value;
        }
    }
}