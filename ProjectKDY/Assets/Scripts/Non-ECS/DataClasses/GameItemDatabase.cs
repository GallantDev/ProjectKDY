using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class GameItemDatabase {
    [SerializeField] private List<GameItemData> gameItems = new List<GameItemData>();
    [SerializeField] private List<GameItemData> basicMaterials = new List<GameItemData>();
    [SerializeField] private List<GameItemData> refinedMaterials = new List<GameItemData>();

    [XmlIgnore]
    public List<GameItemData> GameItems {
        get {
            if (gameItems.Count == 0) {
                List<GameItemData> tmpItems = new List<GameItemData>();
                if (basicMaterials.Count > 0) {
                    tmpItems.AddRange(basicMaterials);
                }
                if (refinedMaterials.Count > 0) {
                    tmpItems.AddRange(refinedMaterials);
                }
                return tmpItems;
            }
            return gameItems;
        }

        set {
            gameItems = value;
        }
    }

    public List<GameItemData> BasicMaterials {
        get {
            return basicMaterials;
        }

        set {
            basicMaterials = value;
        }
    }

    public List<GameItemData> RefinedMaterials {
        get {
            return refinedMaterials;
        }

        set {
            refinedMaterials = value;
        }
    }
}