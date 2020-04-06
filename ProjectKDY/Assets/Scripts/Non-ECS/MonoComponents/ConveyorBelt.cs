using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour {
    //private static Vector3 MOVE_DIRECTION;

    //TRANSFORM MUST BE LOCATED AT BOTTOM CENTER OF BELT TRANSFORM
    private Transform localTrans = null;
    [SerializeField]
    //TRANSFORM MUST BE UNIFORM
    private Transform beltTrans = null;

    private List<WorldGameItemSlot> itemSlots;

    private ConveyorBelt outputTile;

    [SerializeField]
    private GameObject testItemPrefab;

    public Transform LocalTrans {
        get {
            return localTrans;
        }

        set {
            localTrans = value;
        }
    }

    public Transform BeltTrans {
        get {
            return beltTrans;
        }

        set {
            beltTrans = value;
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

    public ConveyorBelt OutputTile {
        get {
            return outputTile;
        }

        set {
            outputTile = value;
        }
    }
}