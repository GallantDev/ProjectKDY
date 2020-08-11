using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameItemUtils {
    private static string itemDatabasePath;
    private static GameItemDatabase itemDatabase;

    public static GameItemDatabase ItemDatabase {
        get {
            if (itemDatabase == null) {
                itemDatabase = new GameItemDatabase();
                itemDatabase = XmlOperations.Deserialize<GameItemDatabase>(Path.Combine(Application.dataPath, "Resources/GameItemDatabase.xml"));
            }
            return itemDatabase;
        }

        set {
            itemDatabase = value;
        }
    }

    //used to retrieve item info from database
    public static GameItemData GetItemDataFromDatabase(int id) {
        GameItemDatabase database = new GameItemDatabase();
        database = XmlOperations.Deserialize<GameItemDatabase>(Path.Combine(Application.dataPath, "Resources/GameItemDatabase.xml"));
        for (int i = 0; i < database.GameItems.Count; i++) {
            if (database.GameItems[i].Id == id) {
                return database.GameItems[i];
            }
        }
        throw new Exception("Item id does not exist!");
    }

    //used to create physical game items
    public static GameItem CreateWorldGameItem(int itemId) {
        MonoEntity tmpEnt;
        GameItem tmpItem;
        GameObjectMeshVisualsCreationCall tmpMeshCall;
        tmpEnt = MonoEngine.Instance.CreateEntity();
        tmpItem = tmpEnt.AddEntityComponent(typeof(GameItem)) as GameItem;
        tmpItem.ItemData = GetItemDataFromDatabase(itemId);
        //tmpItem.ItemStack.ItemCount = itemStackCount;
        tmpMeshCall = tmpEnt.AddEntityComponent(typeof(GameObjectMeshVisualsCreationCall)) as GameObjectMeshVisualsCreationCall;
        tmpMeshCall.VisData.VisMeshPath = tmpItem.ItemData.ItemMesh;
        tmpMeshCall.VisData.VisMatPath = tmpItem.ItemData.ItemMaterial;
        tmpMeshCall.VisData.VisMeshScale = tmpItem.ItemData.ItemMeshScale;
        return tmpItem;
    }
}