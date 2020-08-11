using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonoEntity : MonoBehaviour {
    [SerializeField]
    private int id = 0;
    private List<Component> entityComponents = new List<Component>();
    //private List<Component> unityComponents = new List<Component>();
   // private List<MonoComponent> monoComponents = new List<MonoComponent>();
    public event EventHandler<EntityEventArgs> ComponentAdded;
    public event EventHandler<EntityEventArgs> ComponentRemoved;

    public int Id {
        get {
            return id;
        }

        set {
            id = value;
        }
    }

    public List<Component> EntityComponents {
        get {
            return entityComponents;
        }

        set {
            entityComponents = value;
        }
    }

    //public List<Component> UnityComponents {
    //    get {
    //        return unityComponents;
    //    }

    //    set {
    //        unityComponents = value;
    //    }
    //}

    //public List<MonoComponent> MonoComponents {
    //    get {
    //        return monoComponents;
    //    }

    //    set {
    //        monoComponents = value;
    //    }
    //}

    public bool HasEntityComponent(Type compType) {
        if (GetEntityComponent(compType) == null) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool HasEntityComponents(List<Type> compTypes) {
        for (int i = 0; i < compTypes.Count; i++) {
            if (!HasEntityComponent(compTypes[i])) {
                return false;
            }
        }
        return true;
    }

    //public bool HasUnityComponent(Type compType) {
    //    if (GetUnityComponent(compType) == null) {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //}

    //public bool HasUnityComponents(List<Type> compTypes) {
    //    for (int i = 0; i < compTypes.Count; i++) {
    //        if (!HasUnityComponent(compTypes[i])) {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public bool HasMonoComponent(Type compType) {
    //    if (GetMonoComponent(compType) == null) {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //}

    //public bool HasMonoComponents(List<Type> compTypes) {
    //    for (int i = 0; i < compTypes.Count; i++) {
    //        if (GetMonoComponent(compTypes[i]) == null) {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    public bool IsValidEntityComponentType(Type compType) {
        if (compType == null) {
            throw new Exception("Can't check component validity! Type given was null!");
        }
        if (compType.IsSubclassOf(typeof(Component))) {
            return true;
        }
        else {
            return false;
        }
    }

    public void CacheEntityComponents() {
        List<Component> tmpComps = new List<Component>();
        tmpComps.AddRange(GetComponentsInChildren<Component>());
        for (int i = 0; i < tmpComps.Count; i++) {
            entityComponents.Add(tmpComps[i]);
            if (ComponentAdded != null) {
                ComponentAdded(this, new EntityEventArgs(this));
            }
        }
    }

    public Component AddEntityComponent(Type compType) {
        Component tmpComp;
        if (IsValidEntityComponentType(compType)) {
            tmpComp = gameObject.AddComponent(compType);
            entityComponents.Add(tmpComp);
            if (ComponentAdded != null) {
                ComponentAdded(this, new EntityEventArgs(this));
            }
            return tmpComp;
            //if (compType.IsSubclassOf(typeof(MonoComponent))) {
            //    AddMonoComponent(compType);
            //}
            //else {
            //    AddUnityComponent(compType);
            //}
        }
        else {
            throw new Exception("Can't add component to entity! Type given was not a valid component type!");
        }
    }

    public Component AddEntityComponent(Type compType, GameObject altObj) {
        Component tmpComp;
        if (altObj != null) {
            if (IsValidEntityComponentType(compType)) {
                tmpComp = altObj.AddComponent(compType);
                entityComponents.Add(tmpComp);
                if (ComponentAdded != null) {
                    ComponentAdded(this, new EntityEventArgs(this));
                }
                return tmpComp;
                //if (compType.IsSubclassOf(typeof(MonoComponent))) {
                //    AddMonoComponent(compType);
                //}
                //else {
                //    AddUnityComponent(compType);
                //}
            }
            else {
                throw new Exception("Can't add component to entity! Type given was not a valid component type!");
            }
        }
        else {
            throw new Exception("Can't add component to entity! Alternative GameObject given was null!");
        }
    }

    public void AddEntityComponents(List<Type> compTypes) {
        for (int i = 0; i < compTypes.Count; i++) {
            AddEntityComponent(compTypes[i]);
        }
    }

    public void RemoveEntityComponent(Component comp) {
        if (HasEntityComponent(comp.GetType())) {
            if (ComponentRemoved != null) {
                ComponentRemoved(this, new EntityEventArgs(this));
            }
            entityComponents.Remove(comp);
            Destroy(comp);
            //if (comp.GetType().IsSubclassOf(typeof(MonoComponent))) {
            //    RemoveMonoComponent(comp as MonoComponent);
            //}
            //else {
            //    RemoveUnityComponent(comp);
            //}
        }
        else {
            print("Entity " + id + " does not possess a " + comp.GetType() + " component!");
            throw new Exception("Can't remove component from entity! Entity does not possess given component type!");
        }
    }

    public void RemoveEntityComponents(List<Component> entComps) {
        for (int i = 0; i < entComps.Count; i++) {
            RemoveEntityComponent(entComps[i]);
        }
    }

    public Component GetEntityComponent(Type compType) {
        //if (HasEntityComponent(compType)) {
        //    //if (compType.IsSubclassOf(typeof(MonoComponent))) {
        //    //    return GetMonoComponent(compType);
        //    //}
        //    //else {
        //    //    return GetUnityComponent(compType);
        //    //}
        //}
        Component tmpComp = GetComponentInChildren(compType);
        if (compType != null) {
            return tmpComp;
        }
        else {
            throw new Exception("Can't get component from entity! Type given was not a valid component type!");
        }
    }

    // Pre unified component code
    //private void AddUnityComponent(Type compType) {
    //    Component tmpComp = gameObject.AddComponent(compType);
    //    unityComponents.Add(tmpComp);
    //    if (ComponentAdded != null) {
    //        ComponentAdded(this, new EntityEventArgs(this));
    //    }
    //}

    //private void AddUnityComponents(List<Type> compTypes) {
    //    for (int i = 0; i < compTypes.Count; i++) {
    //        AddUnityComponent(compTypes[i]);
    //    }
    //}

    //private void RemoveUnityComponent(Component comp) {
    //    if (ComponentRemoved != null) {
    //        ComponentRemoved(this, new EntityEventArgs(this));
    //    }
    //    unityComponents.Remove(comp);
    //}

    //private void RemoveUnityComponents(List<Component> comps) {
    //    for (int i = 0; i < comps.Count; i++) {
    //        RemoveUnityComponent(comps[i]);
    //    }
    //}

    //private Component GetUnityComponent(Type compType) {
    //    for (int i = 0; i < unityComponents.Count; i++) {
    //        if (unityComponents[i].GetType() == compType) {
    //            return unityComponents[i];
    //        }
    //    }
    //    return null;
    //}

    //private void AddMonoComponent(Type compType) {
    //    MonoComponent monoComp = gameObject.AddComponent(compType) as MonoComponent;
    //    //MonoComponent monoComp = GetComponent(compType) as MonoComponent;
    //    monoComponents.Add(monoComp);
    //    if (ComponentAdded != null) {
    //        ComponentAdded(this, new EntityEventArgs(this));
    //    }
    //}

    //private void AddMonoComponents(List<Type> compTypes) {
    //    for (int i = 0; i < compTypes.Count; i++) {
    //        AddMonoComponent(compTypes[i]);
    //    }
    //}

    //private void RemoveMonoComponent(MonoComponent comp) {
    //    if (ComponentRemoved != null) {
    //        ComponentRemoved(this, new EntityEventArgs(this));
    //    }
    //    monoComponents.Remove(comp);
    //    Destroy(comp);
    //}

    //private void RemoveMonoComponents(List<MonoComponent> comps) {
    //    for (int i = 0; i < comps.Count; i++) {
    //        RemoveMonoComponent(comps[i]);
    //    }
    //}

    //private MonoComponent GetMonoComponent(Type compType) {
    //    for (int i = 0; i < monoComponents.Count; i++) {
    //        if (monoComponents[i].GetType() == compType) {
    //            return monoComponents[i];
    //        }
    //    }
    //    return null;
    //}
}