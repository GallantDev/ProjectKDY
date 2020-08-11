using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

//DEV SCRIPT ONLY!!!
public class GameItemGenerator : MonoBehaviour {
    [SerializeField] private GameItemData generationItem;

    public void AddItemToDatabase() {
        GameItemDatabase database = new GameItemDatabase();
        database = XmlOperations.Deserialize<GameItemDatabase>(Path.Combine(Application.dataPath, "Resources/GameItemDatabase.xml"));
        generationItem.Id = GetUnreservedItemId(database);
        switch (generationItem.ItemType) {
            case GameItemTypes.BASIC_MATERIAL:
            database.BasicMaterials.Add(generationItem);
            break;
            case GameItemTypes.REFINED_MATERIAL:
            database.RefinedMaterials.Add(generationItem);
            break;
        }
        XmlOperations.Serialize(database, Path.Combine(Application.dataPath, "Resources/GameItemDatabase.xml"));
    }

    private int GetUnreservedItemId(GameItemDatabase database) {
        List<int> reservedIds = new List<int>();
        for (int i = 0; i < database.GameItems.Count; i++) {
            reservedIds.Add(database.GameItems[i].Id);
        }
        for (int x = 0; x < int.MaxValue; x++) {
            if (!reservedIds.Contains(x)) {
                return x;
            }
        }
        throw new Exception("No unreserved item id available. Game item database is full!");
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AddItemToDatabase();
        }
    }
}