using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class GameItemData {
    [SerializeField]
    private int id;
    [SerializeField]
    private string itemName;
    [SerializeField]
    private string itemDescription;
    [SerializeField]
    private GameItemTypes itemType;
    [SerializeField]
    private string itemMesh;
    [SerializeField]
    private SerializableVector3 itemMeshScale;
    [SerializeField]
    private string itemMaterial;
    [SerializeField]
    private string itemSprite;

    public int Id {
        get {
            return id;
        }

        set {
            id = value;
        }
    }

    public string ItemName {
        get {
            return itemName;
        }

        set {
            itemName = value;
        }
    }

    public string ItemDescription {
        get {
            return itemDescription;
        }

        set {
            itemDescription = value;
        }
    }

    public GameItemTypes ItemType {
        get {
            return itemType;
        }

        set {
            itemType = value;
        }
    }

    public string ItemMesh {
        get {
            return itemMesh;
        }

        set {
            itemMesh = value;
        }
    }

    public SerializableVector3 ItemMeshScale {
        get {
            return itemMeshScale;
        }

        set {
            itemMeshScale = value;
        }
    }

    public string ItemMaterial {
        get {
            return itemMaterial;
        }

        set {
            itemMaterial = value;
        }
    }

    public string ItemSprite {
        get {
            return itemSprite;
        }

        set {
            itemSprite = value;
        }
    }
}
