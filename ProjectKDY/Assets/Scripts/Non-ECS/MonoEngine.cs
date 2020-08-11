using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoEngine : MonoBehaviour {
    private static MonoEngine instance;
    [SerializeField]
    private List<MonoEntity> monoEntities = new List<MonoEntity>();
    [SerializeField]
    private List<int> reservedMonoEntityIds = new List<int>();
    private List<int> availableMonoEntityIds = new List<int>();
    private List<MonoGameSystemFilter> gameSystemFilters = new List<MonoGameSystemFilter>();
    [SerializeField]
    private float gameSystemTickLength = 0.0f;

    public event EventHandler<EntityEventArgs> MonoEntityAdded;
    public event EventHandler<EntityEventArgs> MonoEntityRemoved;
    public event EventHandler<SystemEventArgs> GameSystemAdded;
    public event EventHandler<SystemEventArgs> GameSystemRemoved;

    public static MonoEngine Instance {
        get {
            return instance;
        }

        set {
            instance = value;
        }
    }

    private void Start() {
        Init();
        StartCoroutine(TickGameSystems());
    }

    private void Init() {
        if (instance != null) {
            Destroy(this);
        }
        else {
            instance = this;
        }
        InitMonoEntityList();
        InitGameSystemList();
    }

    #region Subscriber Methods
    //properly implement subscriber functions
    #endregion

    private void InitMonoEntityList() {
        List<MonoEntity> ents = new List<MonoEntity>();
        ents.AddRange(FindObjectsOfType<MonoEntity>());
        //print("Number of existing entities: " + ents.Count);
        for (int i = 0; i < ents.Count; i++) {
            //print("Adding existing entity: " + i);
            AddEntity(ents[i]);
        }
    }

    private void InitGameSystemList() {
        gameSystemFilters = new List<MonoGameSystemFilter>();
        List<MonoGameSystem> systems = new List<MonoGameSystem>();
        systems.AddRange(FindObjectsOfType<MonoGameSystem>());
        for (int i = 0; i < systems.Count; i++) {
            AddSystem(systems[i]);
        }
    }

    private bool IsAvailableMonoEntityID() {
        if (reservedMonoEntityIds.Count != int.MaxValue) {
            return true;
        }
        else {
            return false;
        }
    }

    private bool IsUniqueGameSystem(Type systemType) {
        if (systemType.IsSubclassOf(typeof(MonoGameSystem))) {
            for (int i = 0; i < gameSystemFilters.Count; i++) {
                if (gameSystemFilters[i].GameSystem.GetType() == systemType) {
                    return false;
                }
            }
            return true;
        }
        else {
            throw new Exception("Can't check if system type is unique! Provided type is not derived from MonoGameSystem!");
        }
    }
    
    void AddEntity(MonoEntity ent) {
        int tmpIndex = GetAvailableEntityId();
        ent.Id = tmpIndex;
        monoEntities.Add(ent);
        ent.CacheEntityComponents();
        ent.ComponentAdded += AddEntityToSystems;
        ent.ComponentRemoved += RemoveEntityFromSystems;
        AddEntityToSystems(this, new EntityEventArgs(ent));
        //ent.MonoComponents.AddRange(ent.GetComponents<MonoComponent>());
        //AddEntityToSystems(this, new EntityEventArgs(ent));
        //if (MonoEntityAdded != null) {
        //    MonoEntityAdded(this, new EntityEventArgs(ent));
        //}
    }

    void RemoveEntity(MonoEntity ent) {
        if (MonoEntityRemoved != null) {
            MonoEntityRemoved(this, new EntityEventArgs(ent));
        }
        monoEntities.Remove(ent);
        Destroy(ent);
    }

    public MonoEntity CreateEntity() {
        if (IsAvailableMonoEntityID()) {
            GameObject tmpObj = new GameObject();
            MonoEntity ent = tmpObj.AddComponent(typeof(MonoEntity)) as MonoEntity;
            AddEntity(ent);
            return ent;
        }
        throw new Exception("No new entities can be created! Entity Database is full!");
    }

    public MonoEntity CreateEntity(Type compType) {
        if (IsAvailableMonoEntityID()) {
            GameObject tmpObj = new GameObject();
            MonoEntity ent = tmpObj.AddComponent(typeof(MonoEntity)) as MonoEntity;
            ent.AddEntityComponent(compType);
            AddEntity(ent);
            return ent;
        }
        throw new Exception("No new entities can be created! Entity Database is full!");
    }

    public MonoEntity CreateEntity(List<Type> compTypes) {
        print("Creating ENT!");
        if (IsAvailableMonoEntityID()) {
            GameObject tmpObj = new GameObject();
            MonoEntity ent = tmpObj.AddComponent(typeof(MonoEntity)) as MonoEntity;
            ent.AddEntityComponents(compTypes);
            AddEntity(ent);
            return ent;
        }
        throw new Exception("No new entities can be created! Entity Database is full!");
    }

    public MonoEntity CreateEntity(string prefabPath) {
        //Vector3 tmpSpawnPoint = Vector3.zero;
        GameObject tmpObj = Instantiate(Resources.Load<GameObject>(prefabPath));
        if (tmpObj == null) {
            throw new Exception("Provided prefab path is not valid!");
        }
        MonoEntity ent = tmpObj.GetComponent<MonoEntity>();
        if (ent == null) {
            Destroy(tmpObj);
            throw new Exception("Provided prefab is not an entity!");
        }
        AddEntity(ent);
        return ent;
    }

    private void AddEntityToSystems(object sender, EntityEventArgs entArgs) {
       // print("AddEntityToSystems!");
        for (int i = 0; i < gameSystemFilters.Count; i++) {
            if (!gameSystemFilters[i].GameSystem.SystemEntities.Contains(entArgs.Entity) && entArgs.Entity.HasEntityComponents(gameSystemFilters[i].ComponentTypes)) {
                gameSystemFilters[i].GameSystem.SystemEntities.Add(entArgs.Entity);
                for (int x = 0; x < gameSystemFilters[i].ComponentTypes.Count; x++) {
                    gameSystemFilters[i].AddComponentToComponentListProperty(entArgs.Entity.GetEntityComponent(gameSystemFilters[i].ComponentTypes[x]));
                }
            }
        }
    }

    private void RemoveEntityFromSystems(object sender, EntityEventArgs entArgs) {
        for (int i = 0; i < gameSystemFilters.Count; i++) {
            if (gameSystemFilters[i].GameSystem.SystemEntities.Contains(entArgs.Entity) && !entArgs.Entity.HasEntityComponents(gameSystemFilters[i].ComponentTypes)) {
                for (int x = 0; x < gameSystemFilters[i].ComponentTypes.Count; x++) {
                    if (entArgs.Entity.HasEntityComponent(gameSystemFilters[i].ComponentTypes[x])) {
                        gameSystemFilters[i].RemoveComponentFromComponentListProperty(entArgs.Entity.GetEntityComponent(gameSystemFilters[i].ComponentTypes[x]));
                    }
                }
                gameSystemFilters[i].GameSystem.SystemEntities.Remove(entArgs.Entity);
            }
        }
    }

    void AddSystem(MonoGameSystem system) {
        gameSystemFilters.Add(new MonoGameSystemFilter(system));
        system.Init();
        for (int i = 0; i < monoEntities.Count; i++) {
            AddEntityToSystems(this, new EntityEventArgs(monoEntities[i]));
        }
        if (GameSystemAdded != null) {
            GameSystemAdded(this, new SystemEventArgs());
        }
    }

    void AddSystem(Type systemType) {
        if (systemType.IsSubclassOf(typeof(MonoGameSystem))) {
            if (IsUniqueGameSystem(systemType)) {
                MonoGameSystemFilter filter = new MonoGameSystemFilter(systemType);
                for (int i = 0; i < monoEntities.Count; i++) {
                    AddEntityToSystems(this, new EntityEventArgs(monoEntities[i]));
                }
                if (GameSystemAdded != null) {
                    GameSystemAdded(this, new SystemEventArgs());
                }
            }
        }
        else {
            throw new Exception("Failed to add game system! Given Type was not derived from MonoGameSystem!");
        }
    }

    void RemoveSystem(MonoGameSystem system) {
        for (int i = 0; i < monoEntities.Count; i++) {
            RemoveEntityFromSystems(this, new EntityEventArgs(monoEntities[i]));
        }
        if (GameSystemRemoved != null) {
            GameSystemRemoved(this, new SystemEventArgs());
        }
        for (int i = gameSystemFilters.Count; i > 0; i--) {
            if (gameSystemFilters[i].GameSystem == system) {
                gameSystemFilters.RemoveAt(i);
                return;
            }
        }
    }

    int GetAvailableEntityId() {
        //print("Number of reserved Ids: " + reservedMonoEntityIds.Count);
        int tmpId;
        if (availableMonoEntityIds.Count > 0) {
            tmpId = availableMonoEntityIds[0];
            availableMonoEntityIds.Remove(tmpId);
            reservedMonoEntityIds.Add(tmpId);
            return tmpId;
        }
        else {
            for (int i = 0; i < int.MaxValue; i++) {
                if (!reservedMonoEntityIds.Contains(i)) {
                    tmpId = i;
                    reservedMonoEntityIds.Add(i);
                    return tmpId;
                }
            }
        }
        throw new Exception("No available entity ids! Entity database is full!");
    }

    private IEnumerator TickGameSystems() {
        yield return new WaitForSeconds(gameSystemTickLength);
        for (int i = 0; i < gameSystemFilters.Count; i++) {
            gameSystemFilters[i].GameSystem.Tick();
        }
        StartCoroutine(TickGameSystems());
        yield break;
    }
}