using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoUtils {
    //public static MonoEntity GetEntity(List<MonoComponent> compList) {
    //    List<MonoEntity> entities = new List<MonoEntity>();
    //    entities.AddRange(GameObject.FindObjectsOfType<MonoEntity>());
    //    for (int i = 0; i < entities.Count; i++) {
    //        for (int x = 0; x < compList.Count; x++) {
    //            if (!entities[i].MonoComponents.Contains(compList[x])) {
    //                break;
    //            }
    //            if (x == compList.Count - 1) {
    //                return entities[i];
    //            }
    //        }
    //    }
    //    throw new System.Exception("No entity with given components exists!");
    //}

    //public static List<MonoEntity> GetEntities(List<MonoComponent> compList) {
    //    List<MonoEntity> allEntities = new List<MonoEntity>();
    //    List<MonoEntity> matchingEntities = new List<MonoEntity>();
    //    allEntities.AddRange(GameObject.FindObjectsOfType<MonoEntity>());
    //    for (int i = 0; i < allEntities.Count; i++) {
    //        for (int x = 0; x < compList.Count; x++) {
    //            if (!allEntities[i].MonoComponents.Contains(compList[x])) {
    //                break;
    //            }
    //            if (x == compList.Count - 1) {
    //                matchingEntities.Add(allEntities[i]);
    //            }
    //        }
    //    }
    //    if (matchingEntities.Count > 0) {
    //        return matchingEntities;
    //    }
    //    else {
    //        throw new System.Exception("No entities with given components exist!");
    //    }
    //}

    //public static MonoEntity GetEntity(int id) {
    //    List<MonoEntity> allEntities = new List<MonoEntity>();
    //    allEntities.AddRange(GameObject.FindObjectsOfType<MonoEntity>());
    //    for (int i = 0; i < allEntities.Count; i++) {
    //        if (allEntities[i].Id == id) {
    //            return allEntities[i];
    //        }
    //    }
    //    throw new System.Exception("No entity with given id exists!");
    //}

    //public static List<MonoEntity> GetEntities(List<int> ids) {
    //    List<MonoEntity> allEntities = new List<MonoEntity>();
    //    List<MonoEntity> matchingEntities = new List<MonoEntity>();
    //    allEntities.AddRange(GameObject.FindObjectsOfType<MonoEntity>());
    //    for (int i = 0; i < allEntities.Count; i++) {
    //        for (int x = 0; x < ids.Count; x++) {
    //            if (allEntities[i].Id == ids[x]) {
    //                matchingEntities.Add(allEntities[i]);
    //                break;
    //            }
    //        }
    //    }
    //    if (matchingEntities.Count > 0) {
    //        return matchingEntities;
    //    }
    //    else {
    //        throw new System.Exception("No entities with given ids exist!");
    //    }
    //}
}