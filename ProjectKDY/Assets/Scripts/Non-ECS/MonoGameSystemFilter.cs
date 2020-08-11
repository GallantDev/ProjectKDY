using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoGameSystemFilter {
    private List<Type> componentTypes = new List<Type>();
    private List<PropertyInfo> systemCompListProps = new List<PropertyInfo>();
    private MonoGameSystem gameSystem;

    public List<Type> ComponentTypes {
        get {
            return componentTypes;
        }
    }

    public MonoGameSystem GameSystem {
        get {
            return gameSystem;
        }
    }

    public List<PropertyInfo> SystemCompListProps {
        get {
            return systemCompListProps;
        }

        set {
            systemCompListProps = value;
        }
    }

    public MonoGameSystemFilter(Type systemType) {
        if (systemType.IsSubclassOf(typeof(MonoGameSystem))) {
            gameSystem = (MonoGameSystem)Activator.CreateInstance(systemType);
            componentTypes = GetSystemComponentTypes();
            systemCompListProps = GetSystemComponentListProperties();
        }
    }

    public MonoGameSystemFilter(MonoGameSystem system) {
        gameSystem = system;
        componentTypes = GetSystemComponentTypes();
        systemCompListProps = GetSystemComponentListProperties();
    }

    private List<Type> GetSystemComponentTypes() {
        if (gameSystem != null) {
            PropertyInfo[] systemProps = gameSystem.GetType().GetProperties();
            PropertyInfo tmpProp;
            List<Type> compPropTypes = new List<Type>();
            for (int i = 0; i < systemProps.Length; i++) {
                tmpProp = systemProps[i];
                if (tmpProp.PropertyType.IsGenericType && tmpProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                    if (tmpProp.PropertyType.GetGenericArguments()[0] != typeof(MonoEntity) && tmpProp.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(Component))) {
                        compPropTypes.Add(tmpProp.PropertyType.GetGenericArguments()[0]);
                    }
                    //if (tmpProp.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(MonoComponent))) {
                    //    compPropTypes.Add(tmpProp.PropertyType.GetGenericArguments()[0]);
                    //}
                }
            }
            return compPropTypes;
        }
        return null;
    }

    private List<PropertyInfo> GetSystemComponentListProperties() {
        if (gameSystem != null) {
            PropertyInfo[] systemProps = gameSystem.GetType().GetProperties();
            PropertyInfo tmpProp;
            List<PropertyInfo> compProps = new List<PropertyInfo>();
            for (int i = 0; i < systemProps.Length; i++) {
                tmpProp = systemProps[i];
                if (tmpProp.PropertyType.IsGenericType && tmpProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                    if (tmpProp.PropertyType.GetGenericArguments()[0] != typeof(MonoEntity) && tmpProp.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(Component))) {
                        compProps.Add(tmpProp);
                    }
                    //if (tmpProp.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(MonoComponent))) {
                    //    compProps.Add(tmpProp);
                    //}
                }
            }
            return compProps;
        }
        return null;
    }

    public void AddComponentToComponentListProperty(Component comp /*MonoComponent comp*/) {
        if (componentTypes.Contains(comp.GetType())) {
            for (int i = 0; i < systemCompListProps.Count; i++) {
                if (systemCompListProps[i].PropertyType.GetGenericArguments()[0] == comp.GetType()) {
                    IList compList = systemCompListProps[i].GetValue(gameSystem) as IList;
                    compList.Add(comp);
                    systemCompListProps[i].SetValue(gameSystem, compList);
                }
            }
        }
    }

    public void RemoveComponentFromComponentListProperty(Component comp /*MonoComponent comp*/) {
        if (componentTypes.Contains(comp.GetType())) {
            for (int i = 0; i < systemCompListProps.Count; i++) {
                if (systemCompListProps[i].PropertyType.GetGenericArguments()[0] == comp.GetType()) {
                    IList compList = systemCompListProps[i].GetValue(gameSystem) as IList;
                    compList.Remove(comp);
                    systemCompListProps[i].SetValue(gameSystem, compList);
                }
            }
        }
    }


        //public IList GetSystemComponentProperty(MonoComponent comp) {
        //    if (componentTypes.Contains(comp.GetType())) {
        //        for (int i = 0; i < SystemCompProps.Count; i++) {
        //            if (systemCompListProps[i].GetType().IsGenericType && systemCompListProps[i].GetType().GetGenericTypeDefinition() == typeof(List<>)) {
        //                if (systemCompListProps[i].GetType().GetGenericArguments()[0] == comp.GetType()) {
        //                    return systemCompListProps[i] as IList;
        //                }
        //            }
        //        }
        //        return null;
        //    }
        //    else {
        //        throw new Exception("Game system does not operate on given component type!");
        //    }
        //}
}