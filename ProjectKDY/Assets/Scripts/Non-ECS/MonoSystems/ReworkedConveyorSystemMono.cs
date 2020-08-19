using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedConveyorSystemMono : MonoGameSystem {
    private List<Transform> transforms = new List<Transform>();
    private List<GameObjectMeshVisuals> meshVisuals = new List<GameObjectMeshVisuals>();
    private List<FactoryTileRaycastOrigin> raycastOrigins = new List<FactoryTileRaycastOrigin>();
    private List<FactoryItemMovement> itemMovements = new List<FactoryItemMovement>();
    private List<FactoryItemFilter> itemFilters = new List<FactoryItemFilter>();

    private List<FactoryInventoryPath> conveyorPaths = new List<FactoryInventoryPath>();

    [SerializeField]
    //MUST BE LESS THAN ROW COUNT
    private int columnCount = 0;
    [SerializeField]
    //MUST BE MORE THAN COLUMN COUNT
    private int rowCount = 0;

    [SerializeField]
    private GameObject testItemPrefab;

    #region Slot Layout Auto Fields
    //Width = X Axis; Height = Y Axis; Length = Z Axis;
    private float slotWidth = 0.0f;
    private float slotHeight = 0.0f;
    private float slotLength = 0.0f;

    private float halfSlotWidth = 0.0f;
    private float halfSlotHeight = 0.0f;
    private float halfSlotLength = 0.0f;

    private float halfBeltWidth = 0.0f;
    private float beltHeight = 0.0f;
    private float halfBeltHeight = 0.0f;
    private float halfBeltLength = 0.0f;

    private List<float> rowDistances;
    private List<float> columnDistances;
    #endregion

    #region Properties
    public List<Transform> Transforms {
        get {
            return transforms;
        }

        set {
            transforms = value;
        }
    }

    public List<GameObjectMeshVisuals> MeshVisuals {
        get {
            return meshVisuals;
        }

        set {
            meshVisuals = value;
        }
    }

    public List<FactoryTileRaycastOrigin> RaycastOrigins {
        get {
            return raycastOrigins;
        }

        set {
            raycastOrigins = value;
        }
    }

    public List<FactoryItemMovement> ItemMovements {
        get {
            return itemMovements;
        }

        set {
            itemMovements = value;
        }
    }

    public List<FactoryItemFilter> ItemFilters {
        get {
            return itemFilters;
        }

        set {
            itemFilters = value;
        }
    }
    #endregion

    public override void Init() {
        print("Conveyor System: Init!");
        //InitializeConveyorBelts();
        //SpawnRandomItemsOnAllSlotLayouts();
        //conveyorPaths = new List<ConveyorPath>();
        //print("Number of belts: " + SystemEntities.Count);
        //GenerateConveyorPaths();
        //print("Number of paths: " + conveyorPaths.Count);
        //for (int x = 0; x < conveyorPaths.Count; x++) {
        //    print("Conveyor Path " + x + " has " + conveyorPaths[x].NodePath.Count + " nodes!");
        //}
    }

    public override IEnumerator Execute() {
        print("Conveyor System: Execute!");
        //print("Ent count: " + SystemEntities.Count);
        InitializeNewSlotLayouts();
        MoveFactoryItems();
        yield break;
    }

    #region Init Functions
    private void InitializeNewSlotLayouts() {
        FactoryItemSlottedInventory slotInv;
        bool hasNewEntity = false;
        for (int i = 0; i < SystemEntities.Count; i++) {
            itemMovements[i].OutputConveyorEnt = GetOutputConveyorEntity(i);
            if (ItemMovements[i].OutputConveyorEnt != null) {
                ItemMovements[i].HasOutputInventory = true;
            }
            slotInv = SystemEntities[i].GetComponent<FactoryItemSlottedInventory>();
            if (slotInv != null && !slotInv.IsInitialized) {
                AutomaticallySetSlotLayout(i, slotInv);
                SpawnRandomItemsOnSlotLayout(slotInv);
                slotInv.IsInitialized = true;
                hasNewEntity = true;
            }
        }
        if (hasNewEntity) {
            GenerateConveyorPaths();
        }
    }

    ////Initializes all data related to conveyor belt
    //private void InitializeSlottedInventories(int entIndex) {
    //    FactoryItemSlottedInventory slotInv = SystemEntities[entIndex].GetComponent<FactoryItemSlottedInventory>();
    //    if (slotInv != null) {
    //        InitializeSlotLayout(entIndex, slotInv);
    //    }
    //}

    ////Initializes the layout of slots
    private void AutomaticallySetSlotLayout(int entIndex, FactoryItemSlottedInventory slotInv) {
        if (UniformTransformTest(entIndex) == false) {
            throw new Exception("Conveyor belt visual transform is not uniform!");
        }
        if (SlotLayoutTest() == false) {
            throw new Exception("Conveyor belt slot layout not valid! - (row count must be >= column count)");
        }
        halfBeltWidth = meshVisuals[entIndex].ObjMeshScale.x / 2.0f;
        beltHeight = meshVisuals[entIndex].ObjMeshScale.y;
        halfBeltHeight = meshVisuals[entIndex].ObjMeshScale.y / 2.0f;
        halfBeltLength = meshVisuals[entIndex].ObjMeshScale.z / 2.0f;

        slotWidth = meshVisuals[entIndex].ObjMeshScale.x / columnCount;
        halfSlotWidth = slotWidth / 2.0f;
        slotHeight = slotWidth;
        halfSlotHeight = slotHeight / 2.0f;
        slotLength = meshVisuals[entIndex].ObjMeshScale.z / rowCount;
        halfSlotLength = slotLength / 2.0f;

        rowDistances = new List<float>(rowCount);
        for (int i = 0; i < rowCount; i++) {
            rowDistances.Add(0.0f);
        }
        columnDistances = new List<float>(columnCount);
        for (int i = 0; i < columnCount; i++) {
            columnDistances.Add(0.0f);
        }

        Vector3 startPos;
        Vector3 colPos;
        Vector3 rowPos;

        FactoryItemSlottedInventory tmpSlotLayout = SystemEntities[entIndex].GetComponent<FactoryItemSlottedInventory>();
        tmpSlotLayout.ItemSlots = new List<WorldGameItemSlot>();

        int halfIndexResult;
        int halfIndexRemainder;

        int gridDiff;
        int gridDiffDivResult;
        int gridDiffDivRemainder;

        if (rowCount > columnCount) {
            //row position logic
            //create top to bottom
            for (int x = 0; x < rowCount; x++) {
                rowDistances[x] = (((float)(rowCount - 1) / 2) * slotLength) - (x * slotLength);
                //print(rowDistances[x]);
            }
            //column position logic
            for (int y = 0; y < columnCount; y++) {
                if (columnCount == 1) {
                    columnDistances[y] = rowDistances[(rowCount - 1) / 2];
                }
                else if (columnCount == 2) {
                    columnDistances[y] = rowDistances[y * (rowCount - 1)];
                }
                else {
                    gridDiff = rowCount - columnCount;
                    gridDiffDivResult = gridDiff / (columnCount - 1);
                    gridDiffDivRemainder = gridDiff % (columnCount - 1);
                    //even spacing case
                    if (gridDiffDivRemainder == 0) {
                        columnDistances[y] = rowDistances[y * (gridDiffDivResult + 1)];
                    }
                    //impossible even spacing case
                    else {
                        halfIndexResult = y / 2;
                        halfIndexRemainder = y % 2;
                        if (halfIndexRemainder == 0) {
                            //place at left
                            columnDistances[halfIndexResult] = rowDistances[halfIndexResult];
                        }
                        else {
                            //place at right
                            columnDistances[(columnCount - 1) - halfIndexResult] = rowDistances[(rowCount - 1) - halfIndexResult];
                        }
                    }
                }
            }
        }
        //perfect symmetry case
        else if (rowCount == columnCount) {
            for (int x = 0; x < rowCount; x++) {
                rowDistances[x] = (((float)(rowCount - 1) / 2) * slotLength) - (x * slotLength);
            }
            for (int y = 0; y < columnCount; y++) {
                columnDistances[y] = rowDistances[y];
            }
        }
        //adds item slots to list based on column and row positions
        startPos = transforms[entIndex].position + (transforms[entIndex].up * beltHeight);
        //Instantiate(testItemPrefab, startPos, belt.BeltTrans.rotation);
        for (int i = 0; i < columnCount; i++) {
            colPos = startPos + (-transforms[entIndex].right * columnDistances[i]);
            for (int z = 0; z < rowCount; z++) {
                rowPos = colPos + (transforms[entIndex].forward * rowDistances[z]);
                tmpSlotLayout.ItemSlots.Add(new WorldGameItemSlot(rowPos));
                //ItemInventories[entIndex].ItemStacks.Add(new GameItemStack());
            }
        }
    }

    //Checks for scale uniformity of conveyor belt transform
    private bool UniformTransformTest(int entIndex) {
        if (meshVisuals[entIndex].ObjMeshScale.x == meshVisuals[entIndex].ObjMeshScale.y && meshVisuals[entIndex].ObjMeshScale.x == meshVisuals[entIndex].ObjMeshScale.z) {
            return true;
        }
        return false;
    }

    public bool SlotLayoutTest() {
        if (rowCount >= columnCount) {
            return true;
        }
        else {
            return false;
        }
    }
    #endregion

    #region Conveyor Helper Functions
    //CAN BE MOVED - FactoryItemSlottedInventory
    //used in SlotLayout cases
    //Checks if destination slot is on same conveyor belt
    private bool DestinationSlotLocalityTest(int slotIndex) {
        //slot at top of row
        if (slotIndex % rowCount == 0) {
            return false;
        }
        else {
            return true;
        }
    }

    //private bool isSlottedInventory(int entIndex) {
    //    if (SystemEntities[entIndex].GetEntityComponent(typeof(FactoryItemSlottedInventory)) == null) {
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //}

    //private bool isStorageInventory(int entIndex) {
    //    FactoryItemStorageInventory tmpInv = SystemEntities[entIndex].GetComponent<FactoryItemStorageInventory>();
    //    if (tmpInv != null) {
    //        return true;
    //    }
    //    else {
    //        return false;
    //    }
    //}

    //private bool isProductionInventory(int entIndex) {
    //    FactoryItemProductionInventory tmpInv = SystemEntities[entIndex].GetComponent<FactoryItemProductionInventory>();
    //    if (tmpInv != null) {
    //        return true;
    //    }
    //    else {
    //        return false;
    //    }
    //}

    //used in all cases
    //Returns the angle difference of two different belts
    private float GetTileAngleDifference(int entIndex1, int entIndex2) {
        return Vector3.Angle(transforms[entIndex1].forward, transforms[entIndex2].forward);
    }

    //used in all cases
    //returns the linked output conveyor belt
    private MonoEntity GetOutputConveyorEntity(int entIndex) {
        RaycastHit hitinfo;
        Vector3 rayorigin = transforms[entIndex].position + (transforms[entIndex].up * (meshVisuals[entIndex].ObjMeshScale.y / 2.0f));
        Vector3 raydir = transforms[entIndex].forward;
        if (Physics.Raycast(rayorigin, raydir, out hitinfo, transform.localScale.z)) {
            MonoEntity tmpConvEnt = hitinfo.collider.GetComponent<MonoEntity>();
            if (tmpConvEnt != null && SystemEntities.Contains(tmpConvEnt)) {
                //print("found output inventory!");
                return tmpConvEnt;
            }
        }
        return null;
    }

    //used in all cases
    //Returns the linked input conveyor belts
    private List<MonoEntity> GetInputConveyorEntities(int entIndex) {
        List<MonoEntity> tmpConvEnts = new List<MonoEntity>();
        MonoEntity tmpConvEnt;
        List<Vector3> beltRayDirections = new List<Vector3>();
        Vector3 beltCenterPos;
        //Vector3 tmpPos;
        RaycastHit hitInfo;
        beltRayDirections.Add(-transforms[entIndex].forward);
        beltRayDirections.Add(-transforms[entIndex].right);
        beltRayDirections.Add(transforms[entIndex].right);
        beltCenterPos = transforms[entIndex].position + (transforms[entIndex].up * (meshVisuals[entIndex].ObjMeshScale.y / 2.0f));
        for (int i = 0; i < beltRayDirections.Count; i++) {
            if (Physics.Raycast(beltCenterPos, beltRayDirections[i], out hitInfo, (meshVisuals[entIndex].ObjMeshScale.z / 2.0f))) {
                tmpConvEnt = hitInfo.collider.GetComponent<MonoEntity>();
                if (tmpConvEnt != null && SystemEntities.Contains(tmpConvEnt)) {
                    if (GetOutputConveyorEntity(GetSystemEntityIndex(tmpConvEnt)) == SystemEntities[entIndex]) {
                        tmpConvEnts.Add(tmpConvEnt);
                    }
                }
            }
        }
        if (tmpConvEnts.Count > 0) {
            return tmpConvEnts;
        }
        else {
            return null;
        }
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    //used in single slot and layout to layout cases
    public int GetDestinationItemSlotIndex(int slotIndex, float angleDiff) {
        //print("Slot Index: " + slotIndex + ", Angle Diff: " + angleDiff);
        int slotCount = rowCount * columnCount;
        //WORKING
        if (columnCount == 1) {
            return (slotCount - 1) / 2;
        }
        else {
            //WORKING
            //same count case
            if (columnCount == rowCount) {
                return (slotIndex / rowCount);
            }
            else {
                if (angleDiff == 0.0f) {
                    return (slotIndex + (rowCount - 1));
                }
                //WORKING
                //check for even gap case
                if (columnCount > 2 && (rowCount - columnCount) % (columnCount - 1) == 0) {
                    if (angleDiff == -90.0f) {
                        return (slotIndex / rowCount) * ((rowCount - columnCount) / (columnCount - 1) + 1);
                    }
                    else {
                        return (slotCount - 1) - ((slotIndex / rowCount) * ((rowCount - columnCount) / (columnCount - 1) + 1));
                    }
                }
                //WORKING
                //outside only case
                else {
                    if (slotIndex / rowCount < columnCount / 2) {
                        if (angleDiff == -90.0f) {
                            return (slotIndex / rowCount);
                        }
                        else {
                            return (slotCount - 1) - (slotIndex / rowCount);
                        }
                    }
                    else {
                        if (angleDiff == -90.0f) {
                            return (slotIndex / rowCount) + (rowCount - columnCount);
                        }
                        else {
                            return (slotCount - 1) - ((slotIndex / rowCount) + (rowCount - columnCount));
                        }
                    }
                }
            }
        }
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    private WorldGameItemSlot GetDestinationItemSlotFromLocalSlottedInventory(FactoryItemSlottedInventory inputLayout, int slotIndex) {
        WorldGameItemSlot tmpSlot = null;
        if (DestinationSlotLocalityTest(slotIndex) == true) {
            tmpSlot = inputLayout.ItemSlots[slotIndex - 1];
            if (tmpSlot.WorldGameItem != null) {
                return null;
            }
            else {
                //print("Moving local from slot " + slotIndex + " to slot dest " + (slotIndex - 1));
                return tmpSlot;
            }
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    //used in layoyut to layout case
    //Returns the destination slot - moving slot layout to slot layout
    private WorldGameItemSlot GetDestinationItemSlotFromSlottedInventory(int inputEntIndex, int slotIndex, int outputEntIndex, FactoryItemSlottedInventory outputLayout) {
        WorldGameItemSlot tmpSlot = null;
        float angleDiff = Mathf.RoundToInt(Vector3.SignedAngle(transforms[inputEntIndex].forward, transforms[outputEntIndex].forward, Vector3.up));
        //print("Dest Slot Index: " + GetDestinationItemSlotIndex(slotIndex, angleDiff));
        tmpSlot = outputLayout.ItemSlots[GetDestinationItemSlotIndex(slotIndex, angleDiff)];
        if (tmpSlot.WorldGameItem == null) {
            return tmpSlot;
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    //no change needed
    //used in virtual to physical case
    //Returns the destination slot, moving virtual to physical
    public WorldGameItemSlot GetDestinationItemSlotFromSlotlessInventory(int inputEntIndex, int outputSlotIndex, int outputEntIndex) {
        FactoryItemSlottedInventory tmpSlotLayout = SystemEntities[outputEntIndex].GetComponent<FactoryItemSlottedInventory>();
        WorldGameItemSlot tmpSlot = null;
        float angleDiff = Mathf.RoundToInt(Vector3.SignedAngle(transforms[inputEntIndex].forward, transforms[outputEntIndex].forward, Vector3.up));
        print("Input slot index: " + outputSlotIndex + ", angle diff: " + angleDiff + "!");
        //tmpSlot = tmpSlotLayout.ItemSlots[GetDestinationItemSlotIndex(inputSlotIndex, angleDiff)];
        tmpSlot = tmpSlotLayout.ItemSlots[outputSlotIndex];
        if (tmpSlot.WorldGameItem == null) {
            return tmpSlot;
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    //no change needed
    //used by slotless inventories to move to slotted inventories
    private List<int> GetOpenEntrySlotIndices(int inputEntIndex, int outputEntIndex, FactoryItemSlottedInventory outputLayout) {
        List<int> tmpSlotIndices = new List<int>();
        int tmpSlotIndex;
        int[] entryIndices = new int[columnCount];
        for (int i = 0; i < columnCount; i++) {
            entryIndices[i] = i * rowCount;
            tmpSlotIndex = GetDestinationItemSlotIndex(entryIndices[i], Mathf.RoundToInt(Vector3.SignedAngle(transforms[inputEntIndex].forward, transforms[outputEntIndex].forward, Vector3.up)));
            if (outputLayout.ItemSlots[tmpSlotIndex].WorldGameItem == null) {
                tmpSlotIndices.Add(tmpSlotIndex);
            }
        }
        return tmpSlotIndices;
    }

    //CAN BE MOVED - FactoryItemFilter
    private GameItemStack GetExistingItemStackFromItemFilter(int itemId, FactoryItemFilter filter) {
        for (int i = 0; i < filter.AllowedItemStackDatas.Count; i++) {
            if (itemId == filter.AllowedItemStackDatas[i].ItemData.Id) {
                return filter.AllowedItemStackDatas[i];
            }
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private GameItemStack GetExistingItemStackFromProductionInventory(GameItemData itemData, FactoryItemProductionInventory prodInv) {
        for (int i = 0; i < prodInv.InputItemStacks.Count; i++) {
            if (itemData.Id == prodInv.InputItemStacks[i].ItemData.Id) {
                return prodInv.InputItemStacks[i];
            }
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private GameItemStack GetExistingItemStackFromStorageInventory(GameItemData itemData, FactoryItemStorageInventory storageInv) {
        for (int i = 0; i < storageInv.ItemStacks.Count; i++) {
            if (itemData.Id == storageInv.ItemStacks[i].ItemData.Id) {
                return storageInv.ItemStacks[i];
            }
        }
        return null;
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private void AddItemToProductionInventory(GameItemData inputItem, FactoryItemProductionInventory prodInv) {
        GameItemStack tmpStack = null;
        tmpStack = GetExistingItemStackFromProductionInventory(inputItem, prodInv);
        if (tmpStack == null) {
            prodInv.InputItemStacks.Add(new GameItemStack(inputItem, 1));
        }
        else {
            tmpStack.ItemCount++;
        }
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private void AddItemStackToProductionInventory(GameItemStack inputItemStack, FactoryItemProductionInventory prodInv) {
        GameItemStack tmpStack = null;
        tmpStack = GetExistingItemStackFromProductionInventory(inputItemStack.ItemData, prodInv);
        if (tmpStack == null) {
            prodInv.InputItemStacks.Add(inputItemStack);
        }
        else {
            tmpStack.ItemCount += inputItemStack.ItemCount;
        }
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private void AddItemToStorageInventory(GameItemData inputItem, FactoryItemStorageInventory storageInv) {
        GameItemStack tmpStack = null;
        tmpStack = GetExistingItemStackFromStorageInventory(inputItem, storageInv);
        if (tmpStack == null) {
            storageInv.ItemStacks.Add(new GameItemStack(inputItem, 1));
        }
        else {
            tmpStack.ItemCount++;
        }
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private void AddItemStackToStorageInventory(GameItemStack inputItemStack, FactoryItemStorageInventory storageInv) {
        GameItemStack tmpStack = null;
        tmpStack = GetExistingItemStackFromStorageInventory(inputItemStack.ItemData, storageInv);
        if (tmpStack == null) {
            storageInv.ItemStacks.Add(inputItemStack);
        }
        else {
            tmpStack.ItemCount += inputItemStack.ItemCount;
        }
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private void RemoveItemFromProductionInventory(GameItemData inputItem, FactoryItemProductionInventory outputProdInv) {
        for (int i = 0; i < outputProdInv.OutputItemStacks.Count; i++) {
            if (outputProdInv.OutputItemStacks[i].ItemData.Id == inputItem.Id) {
                outputProdInv.OutputItemStacks[i].ItemCount--;
                if (outputProdInv.OutputItemStacks[i].ItemCount == 0) {
                    outputProdInv.OutputItemStacks.RemoveAt(i);
                }
                return;
            }
        }
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private void RemoveItemStackFromProductionInventory(GameItemStack inputItemStack, FactoryItemProductionInventory outputProdInv) {
        for (int i = 0; i < outputProdInv.OutputItemStacks.Count; i++) {
            if (outputProdInv.OutputItemStacks[i].ItemData.Id == inputItemStack.ItemData.Id) {
                outputProdInv.OutputItemStacks[i].ItemCount -= inputItemStack.ItemCount;
                if (outputProdInv.OutputItemStacks[i].ItemCount == 0) {
                    outputProdInv.OutputItemStacks.RemoveAt(i);
                }
                return;
            }
        }
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private void RemoveItemFromStorageInventory(GameItemData inputItem, FactoryItemStorageInventory outputStorageInv) {
        for (int i = 0; i < outputStorageInv.ItemStacks.Count; i++) {
            if (outputStorageInv.ItemStacks[i].ItemData.Id == inputItem.Id) {
                outputStorageInv.ItemStacks[i].ItemCount--;
                if (outputStorageInv.ItemStacks[i].ItemCount == 0) {
                    outputStorageInv.ItemStacks.RemoveAt(i);
                }
                return;
            }
        }
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private void RemoveItemStackFromStorageInventory(GameItemStack inputItemStack, FactoryItemStorageInventory outputStorageInv) {
        for (int i = 0; i < outputStorageInv.ItemStacks.Count; i++) {
            if (outputStorageInv.ItemStacks[i].ItemData.Id == inputItemStack.ItemData.Id) {
                outputStorageInv.ItemStacks[i].ItemCount -= inputItemStack.ItemCount;
                if (outputStorageInv.ItemStacks[i].ItemCount == 0) {
                    outputStorageInv.ItemStacks.RemoveAt(i);
                }
                return;
            }
        }
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    private int GetTotalItemCount(FactoryItemSlottedInventory slotInv) {
        int numOfItems = 0;
        for (int i = 0; i < slotInv.ItemSlots.Count; i++) {
            if (slotInv.ItemSlots[i].WorldGameItem != null) {
                numOfItems++;
            }
        }
        return numOfItems;
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    public int GetItemCount(FactoryItemSlottedInventory slotInv, int itemId) {
        int numOfItems = 0;
        for (int i = 0; i < slotInv.ItemSlots.Count; i++) {
            if (slotInv.ItemSlots[i].WorldGameItem.ItemData.Id == itemId) {
                numOfItems++;
            }
        }
        return numOfItems;
    }

    //private int GetAvailableItemSpaceCount(int entIndex, FactoryItemSlottedInventory slotInv, int itemId) {
    //    if (itemFilters[entIndex].AllowedItemStackDatas.ContainsKey(itemId)) {
    //        return itemFilters[entIndex].AllowedItemStackDatas[itemId] - GetItemCount(slotInv, itemId);
    //    }
    //    else {
    //        return 0;
    //    }
    //}

    //CAN BE MOVED - FactoryItemProductionInventory
    //Returns amount of input items
    private int GetTotalItemCount(FactoryItemProductionInventory prodInv) {
        int itemCount = 0;
        for (int i = 0; i < prodInv.InputItemStacks.Count; i++) {
            itemCount += prodInv.InputItemStacks[i].ItemCount;
        }
        return itemCount;
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    //Returns amount of input items of type
    public int GetItemCount(FactoryItemProductionInventory prodInv, int itemId) {
        for (int i = 0; i < prodInv.InputItemStacks.Count; i++) {
            if (prodInv.InputItemStacks[i].ItemData.Id == itemId) {
                return prodInv.InputItemStacks[i].ItemCount;
            }
        }
        return 0;
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private int GetTotalItemCount(FactoryItemStorageInventory storageInv) {
        int numOfItems = 0;
        for (int i = 0; i < storageInv.ItemStacks.Count; i++) {
            numOfItems += storageInv.ItemStacks[i].ItemCount;
        }
        return numOfItems;
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    public int GetItemCount(FactoryItemStorageInventory storageInv, int itemId) {
        int numOfItems = 0;
        for (int i = 0; i < storageInv.ItemStacks.Count; i++) {
            if (storageInv.ItemStacks[i].ItemData.Id == itemId) {
                numOfItems = storageInv.ItemStacks[i].ItemCount;
                break;
            }
        }
        return numOfItems;
    }

    private bool CanTransfer(int inputEntIndex, int outputEntIndex) {
        if (itemFilters[inputEntIndex].CanBeRemovedFrom && itemFilters[outputEntIndex].CanBeAddedTo) {
            return true;
        }
        else {
            return false;
        }
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    private bool InventoryIsFull(int invEntIndex, FactoryItemSlottedInventory slotInv) {
        if (GetTotalItemCount(slotInv) < itemFilters[invEntIndex].MaxItemCount) {
            return true;
        }
        else {
            return false;
        }
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    private bool InventoryIsFull(int invEntIndex, FactoryItemProductionInventory prodInv) {
        if (GetTotalItemCount(prodInv) < itemFilters[invEntIndex].MaxItemCount) {
            return true;
        }
        else {
            return false;
        }
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    private bool InventoryIsFull(int invEntIndex, FactoryItemStorageInventory storageInv) {
        if (GetTotalItemCount(storageInv) < itemFilters[invEntIndex].MaxItemCount) {
            return true;
        }
        else {
            return false;
        }
    }

    //CAN BE MOVED - FactoryItemSlottedInventory
    //treat as all item types allowed if AllowedItemStackDatas.Count == 0
    private bool InventoryHasSpaceForItemType(int entIndex, FactoryItemSlottedInventory slotInv, int id, int count) {
        GameItemStack tmpStack = null;
        if (itemFilters[entIndex].AllowedItemStackDatas.Count > 0) {
            tmpStack = GetExistingItemStackFromItemFilter(id, itemFilters[entIndex]);
            if (tmpStack != null) {
                if (GetItemCount(slotInv, id) + count <= tmpStack.ItemCount) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else {
            if (GetTotalItemCount(slotInv) + count <= itemFilters[entIndex].MaxItemCount) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }

    //CAN BE MOVED - FactoryItemProductionInventory
    //treat as all item types allowed if AllowedItemStackDatas.Count == 0
    private bool InventoryHasSpaceForItemType(int entIndex, FactoryItemProductionInventory prodInv, int id, int count) {
        GameItemStack tmpStack = null;
        if (itemFilters[entIndex].AllowedItemStackDatas.Count > 0) {
            tmpStack = GetExistingItemStackFromItemFilter(id, itemFilters[entIndex]);
            if (tmpStack != null) {
                if (GetItemCount(prodInv, id) + count <= tmpStack.ItemCount) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else {
            if (GetTotalItemCount(prodInv) + count <= itemFilters[entIndex].MaxItemCount) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }

    //CAN BE MOVED - FactoryItemStorageInventory
    //treat as all item types allowed if AllowedItemStackDatas.Count == 0
    private bool InventoryHasSpaceForItemType(int entIndex, FactoryItemStorageInventory storageInv, int id, int count) {
        GameItemStack tmpStack = null;
        if (itemFilters[entIndex].AllowedItemStackDatas.Count > 0) {
            tmpStack = GetExistingItemStackFromItemFilter(id, itemFilters[entIndex]);
            if (tmpStack != null) {
                if (GetItemCount(storageInv, id) + count <= tmpStack.ItemCount) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else {
            if (GetTotalItemCount(storageInv) + count <= itemFilters[entIndex].MaxItemCount) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }
    #endregion

    //READY
    private void MoveSlottedInventoryToSlottedInventory(int inputIndex, FactoryItemSlottedInventory inputSlotInv, int outputIndex, FactoryItemSlottedInventory outputSlotInv) {
        WorldGameItemSlot destSlot = null;
        //print("Moving slotted to slotted!");
        for (int y = 0; y < inputSlotInv.ItemSlots.Count; y++) {
            //print("Conveyor Path - " + i + ", Conveyor Node - " + x + ", Conveyor Slot - " + y + ", Slot Item - " + conveyorPaths[i].NodePath[x].Conveyor.ItemSlots[i].SlotItem);
            if (inputSlotInv.ItemSlots[y].WorldGameItem != null) {
                //print("Input Index: " + tmpInputIndex + ", Slot Index: " + y + ", OutputIndex: " + tmpOutputIndex + ", Ouput Layout: " + tmpOutputLayout);
                destSlot = null;
                if (DestinationSlotLocalityTest(y)) {
                    destSlot = GetDestinationItemSlotFromLocalSlottedInventory(inputSlotInv, y);
                }
                else if (outputSlotInv != inputSlotInv && CanTransfer(inputIndex, outputIndex)) {
                    if (InventoryHasSpaceForItemType(outputIndex, outputSlotInv, inputSlotInv.ItemSlots[y].WorldGameItem.ItemData.Id, 1)) {
                        destSlot = GetDestinationItemSlotFromSlottedInventory(inputIndex, y, outputIndex, outputSlotInv);
                    }
                }
                MovePhysicalItem(inputSlotInv.ItemSlots[y], destSlot);
            }
        }
    }

    //READY
    private void MoveSlottedInventoryToProductionInventory(int inputIndex, FactoryItemSlottedInventory inputSlotInv, int outputIndex, FactoryItemProductionInventory outputProdInv) {
        WorldGameItemSlot destSlot = null;
        for (int y = 0; y < inputSlotInv.ItemSlots.Count; y++) {
            if (inputSlotInv.ItemSlots[y].WorldGameItem != null) {
                if (DestinationSlotLocalityTest(y)) {
                    destSlot = GetDestinationItemSlotFromLocalSlottedInventory(inputSlotInv, y);
                    MovePhysicalItem(inputSlotInv.ItemSlots[y], destSlot);
                }
                else if (CanTransfer(inputIndex, outputIndex)) {
                    if (InventoryHasSpaceForItemType(outputIndex, outputProdInv, inputSlotInv.ItemSlots[y].WorldGameItem.ItemData.Id, 1)) {
                        AddItemToProductionInventory(inputSlotInv.ItemSlots[y].WorldGameItem.ItemData, outputProdInv);
                        Destroy(inputSlotInv.ItemSlots[y].WorldGameItem.gameObject);
                        inputSlotInv.ItemSlots[y].WorldGameItem = null;
                    }
                }
            }
        }
    }

    //READY
    private void MoveSlottedInventoryToStorageInventory(int inputEntIndex, FactoryItemSlottedInventory inputSlotInv, int outputEntIndex, FactoryItemStorageInventory outputStorageInv) {
        WorldGameItemSlot destSlot = null;
        for (int y = 0; y < inputSlotInv.ItemSlots.Count; y++) {
            if (inputSlotInv.ItemSlots[y].WorldGameItem != null) {
                if (DestinationSlotLocalityTest(y)) {
                    destSlot = GetDestinationItemSlotFromLocalSlottedInventory(inputSlotInv, y);
                    MovePhysicalItem(inputSlotInv.ItemSlots[y], destSlot);
                }
                else if (CanTransfer(inputEntIndex, outputEntIndex)) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputStorageInv, inputSlotInv.ItemSlots[y].WorldGameItem.ItemData.Id, 1)) {
                        AddItemToStorageInventory(inputSlotInv.ItemSlots[y].WorldGameItem.ItemData, outputStorageInv);
                        Destroy(inputSlotInv.ItemSlots[y].WorldGameItem.gameObject);
                        inputSlotInv.ItemSlots[y].WorldGameItem = null;
                    }
                }
            }
        }
    }

    private void MovePhysicalItem(WorldGameItemSlot inputSlot, WorldGameItemSlot destSlot) {
        //print("Input slot: " + inputSlot + ", Dest slot: " + destSlot);
        if (destSlot != null && destSlot != inputSlot) {
            //print("Input slot pos: " + inputSlot.WorldPosition + ", Dest slot pos: " + destSlot.WorldPosition);
            destSlot.WorldGameItem = inputSlot.WorldGameItem;
            destSlot.WorldGameItem.transform.position = destSlot.WorldPosition;
            inputSlot.WorldGameItem = null;
        }
    }

    //READY
    private void MoveProductionInventoryToSlottedInventory(int inputEntIndex, FactoryItemProductionInventory inputProdInv, int outputEntIndex, FactoryItemSlottedInventory outputSlotInv) {
        List<int> tmpEntryIndices = GetOpenEntrySlotIndices(inputEntIndex, outputEntIndex, outputSlotInv);
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < tmpEntryIndices.Count; i++) {
                for (int d = 0; d < inputProdInv.OutputItemStacks.Count; d++) {
                    if (InventoryHasSpaceForItemType(inputEntIndex, inputProdInv, inputProdInv.OutputItemStacks[d].ItemData.Id, 1)) {
                        outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldGameItem = GameItemUtils.CreateWorldGameItem(inputProdInv.OutputItemStacks[d].ItemData.Id);
                        outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldGameItem.transform.position = outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldPosition;
                        RemoveItemFromProductionInventory(inputProdInv.OutputItemStacks[d].ItemData, inputProdInv);
                        break;
                    }
                }
            }
        }
    }

    //READY
    private void MoveProductionInventoryToProductionInventory(int inputEntIndex, FactoryItemProductionInventory inputProdInv, int outputEntIndex, FactoryItemProductionInventory outputProdInv) {
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < columnCount; i++) {
                for (int x = 0; x < inputProdInv.OutputItemStacks.Count; x++) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputProdInv, inputProdInv.OutputItemStacks[x].ItemData.Id, 1)) {
                        AddItemToProductionInventory(inputProdInv.OutputItemStacks[x].ItemData, outputProdInv);
                        RemoveItemFromProductionInventory(inputProdInv.OutputItemStacks[x].ItemData, inputProdInv);
                        break;
                    }
                }
            }
        }
    }

    //READY
    private void MoveProductionInventoryToStorageInventory(int inputEntIndex, FactoryItemProductionInventory inputProdInv, int outputEntIndex, FactoryItemStorageInventory outputStorageInv) {
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < columnCount; i++) {
                for (int x = 0; x < inputProdInv.OutputItemStacks.Count; x++) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputStorageInv, inputProdInv.OutputItemStacks[x].ItemData.Id, 1)) {
                        AddItemToStorageInventory(inputProdInv.OutputItemStacks[x].ItemData, outputStorageInv);
                        RemoveItemFromProductionInventory(inputProdInv.OutputItemStacks[x].ItemData, inputProdInv);
                        break;
                    }
                }
            }
        }
    }

    //READY
    private void MoveStorageInventoryToSlottedInventory(int inputEntIndex, FactoryItemStorageInventory inputStorageInv, int outputEntIndex, FactoryItemSlottedInventory outputSlotInv) {
        List<int> tmpEntryIndices = GetOpenEntrySlotIndices(inputEntIndex, outputEntIndex, outputSlotInv);
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < tmpEntryIndices.Count; i++) {
                for (int x = 0; x < inputStorageInv.ItemStacks.Count; x++) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputSlotInv, inputStorageInv.ItemStacks[x].ItemData.Id, 1)) {
                        outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldGameItem = GameItemUtils.CreateWorldGameItem(inputStorageInv.ItemStacks[x].ItemData.Id);
                        outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldGameItem.transform.position = outputSlotInv.ItemSlots[tmpEntryIndices[i]].WorldPosition;
                        RemoveItemFromStorageInventory(inputStorageInv.ItemStacks[x].ItemData, inputStorageInv);
                        break;
                    }
                }
            }
        }
    }

    //READY
    private void MoveStorageInventoryToProductionInventory(int inputEntIndex, FactoryItemStorageInventory inputStorageInv, int outputEntIndex, FactoryItemProductionInventory outputProdInv) {
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < columnCount; i++) {
                for (int x = 0; x < inputStorageInv.ItemStacks.Count; x++) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputProdInv, inputStorageInv.ItemStacks[x].ItemData.Id, 1)) {
                        AddItemToProductionInventory(inputStorageInv.ItemStacks[x].ItemData, outputProdInv);
                        RemoveItemFromStorageInventory(inputStorageInv.ItemStacks[x].ItemData, inputStorageInv);
                        break;
                    }
                }
            }
        }
    }

    //READY
    private void MoveStorageInventoryToStorageInventory(int inputEntIndex, FactoryItemStorageInventory inputStorageInv, int outputEntIndex, FactoryItemStorageInventory outputStorageInv) {
        if (CanTransfer(inputEntIndex, outputEntIndex)) {
            for (int i = 0; i < columnCount; i++) {
                for (int x = 0; x < inputStorageInv.ItemStacks.Count; x++) {
                    if (InventoryHasSpaceForItemType(outputEntIndex, outputStorageInv, inputStorageInv.ItemStacks[x].ItemData.Id, 1)) {
                        AddItemToStorageInventory(inputStorageInv.ItemStacks[x].ItemData, outputStorageInv);
                        RemoveItemFromStorageInventory(inputStorageInv.ItemStacks[x].ItemData, inputStorageInv);
                        break;
                    }
                }
            }
        }
    }

    //Generates conveyor paths to process belt item movements in order
    private void GenerateConveyorPaths() {
        List<MonoEntity> tmpPathEnds = new List<MonoEntity>();
        List<FactoryInventoryPathNode> tmpPathNodes = new List<FactoryInventoryPathNode>();
        List<FactoryInventoryPathNode> tmpSidePathNodes = new List<FactoryInventoryPathNode>();
        List<MonoEntity> tmpTiles = new List<MonoEntity>();
        List<MonoEntity> tmpOldInputTiles = new List<MonoEntity>();
        List<MonoEntity> tmpNewInputTiles = new List<MonoEntity>();
        float tmpAngleDiff;
        int tmpDist;
        for (int i = 0; i < itemMovements.Count; i++) {
            if (itemMovements[i].HasOutputInventory == false) {
                tmpPathEnds.Add(SystemEntities[i]);
            }
        }
        for (int x = 0; x < tmpPathEnds.Count; x++) {
            tmpPathNodes.Clear();
            tmpNewInputTiles = new List<MonoEntity>();
            tmpDist = 0;
            //Add first path node (path end tile) manually
            tmpPathNodes.Add(new FactoryInventoryPathNode(tmpPathEnds[x], tmpDist, 0.0f));
            tmpDist++;
            //get actual entity 
            tmpTiles = GetInputConveyorEntities(GetSystemEntityIndex(tmpPathEnds[x]));
            if (tmpTiles != null) {
                tmpOldInputTiles.AddRange(tmpTiles);
            }
            while (tmpNewInputTiles != null) {
                if (tmpOldInputTiles.Count == 0) {
                    tmpNewInputTiles = null;
                    break;
                }
                else {
                    for (int y = 0; y < tmpOldInputTiles.Count; y++) {
                        //print("Conveyor Path End" + x + ", Node " + (tmpPathNodes.Count + y));
                        tmpAngleDiff = GetTileAngleDifference(GetSystemEntityIndex(tmpOldInputTiles[y]), GetSystemEntityIndex(GetOutputConveyorEntity(GetSystemEntityIndex(tmpOldInputTiles[y]))));
                        if (tmpAngleDiff == 0.0f) {
                            tmpPathNodes.Add(new FactoryInventoryPathNode(tmpOldInputTiles[y], tmpDist, tmpAngleDiff));
                        }
                        else {
                            tmpSidePathNodes.Add(new FactoryInventoryPathNode(tmpOldInputTiles[y], tmpDist, tmpAngleDiff));
                        }
                        tmpTiles = GetInputConveyorEntities(GetSystemEntityIndex(tmpOldInputTiles[y]));
                        if (tmpTiles != null) {
                            tmpNewInputTiles.AddRange(tmpTiles);
                        }
                    }
                    tmpPathNodes.AddRange(tmpSidePathNodes);
                    tmpSidePathNodes.Clear();
                    tmpOldInputTiles.Clear();
                    tmpOldInputTiles.AddRange(tmpNewInputTiles);
                    tmpNewInputTiles.Clear();
                    tmpDist++;
                }
            }
            conveyorPaths.Add(new FactoryInventoryPath(tmpPathNodes));
        }
    }

    //Moves items along all factory inventories on a path by path basis
    private void MoveFactoryItems() {
        FactoryItemSlottedInventory tmpInputSlotInv = null;
        FactoryItemSlottedInventory tmpOutputSlotInv = null;
        FactoryItemProductionInventory tmpInputProdInv = null;
        FactoryItemProductionInventory tmpOutputProdInv = null;
        FactoryItemStorageInventory tmpInputStorageInv = null;
        FactoryItemStorageInventory tmpOutputStorageInv = null;
        int tmpInputIndex;
        int tmpOutputIndex;
        List<int> tmpEntryIndices = new List<int>();
        for (int i = 0; i < conveyorPaths.Count; i++) {
            for (int x = 0; x < conveyorPaths[i].NodePath.Count; x++) {
                tmpInputSlotInv = null;
                tmpOutputSlotInv = null;
                tmpInputIndex = -1;
                tmpOutputIndex = -1;
                tmpInputIndex = GetSystemEntityIndex(conveyorPaths[i].NodePath[x].ConveyorEnt);
                tmpInputSlotInv = SystemEntities[tmpInputIndex].GetComponent<FactoryItemSlottedInventory>();
                tmpInputProdInv = SystemEntities[tmpInputIndex].GetComponent<FactoryItemProductionInventory>();
                tmpInputStorageInv = SystemEntities[tmpInputIndex].GetComponent<FactoryItemStorageInventory>();
                if (itemMovements[tmpInputIndex].HasOutputInventory) {
                    tmpOutputIndex = GetSystemEntityIndex(itemMovements[tmpInputIndex].OutputConveyorEnt);
                    tmpOutputSlotInv = SystemEntities[tmpOutputIndex].GetComponent<FactoryItemSlottedInventory>();
                    tmpOutputProdInv = SystemEntities[tmpOutputIndex].GetComponent<FactoryItemProductionInventory>();
                    tmpOutputStorageInv = SystemEntities[tmpOutputIndex].GetComponent<FactoryItemStorageInventory>();
                    if (tmpInputProdInv != null) {
                        if (tmpOutputSlotInv != null) {
                            MoveProductionInventoryToSlottedInventory(tmpInputIndex, tmpInputProdInv, tmpOutputIndex, tmpOutputSlotInv);
                            continue;
                        }
                        else if (tmpOutputProdInv != null) {
                            MoveProductionInventoryToProductionInventory(tmpInputIndex, tmpInputProdInv, tmpOutputIndex, tmpOutputProdInv);
                            continue;
                        }
                        else if (tmpOutputStorageInv != null) {
                            MoveProductionInventoryToStorageInventory(tmpInputIndex, tmpInputProdInv, tmpOutputIndex, tmpOutputStorageInv);
                            continue;
                        }
                    }
                    else if (tmpInputStorageInv != null) {
                        if (tmpOutputSlotInv != null) {
                            MoveStorageInventoryToSlottedInventory(tmpInputIndex, tmpInputStorageInv, tmpOutputIndex, tmpOutputSlotInv);
                            continue;
                        }
                        else if (tmpOutputProdInv != null) {
                            MoveStorageInventoryToProductionInventory(tmpInputIndex, tmpInputStorageInv, tmpOutputIndex, tmpOutputProdInv);
                            continue;
                        }
                        else if (tmpOutputStorageInv != null) {
                            MoveStorageInventoryToStorageInventory(tmpInputIndex, tmpInputStorageInv, tmpOutputIndex, tmpOutputStorageInv);
                            continue;
                        }
                    }
                }
                else {
                    tmpOutputIndex = tmpInputIndex;
                }
                if (tmpInputSlotInv != null) {
                    if (tmpOutputProdInv != null) {
                        MoveSlottedInventoryToProductionInventory(tmpInputIndex, tmpInputSlotInv, tmpOutputIndex, tmpOutputProdInv);
                    }
                    else if (tmpOutputStorageInv != null) {
                        MoveSlottedInventoryToStorageInventory(tmpInputIndex, tmpInputSlotInv, tmpOutputIndex, tmpOutputStorageInv);
                    }
                    else {
                        if (tmpOutputSlotInv == null) {
                            tmpOutputSlotInv = tmpInputSlotInv;
                        }
                        MoveSlottedInventoryToSlottedInventory(tmpInputIndex, tmpInputSlotInv, tmpOutputIndex, tmpOutputSlotInv);
                    }
                }
            }
        }
    }

    #region Debug Functions
    private void SpawnRandomItemsOnAllSlotLayouts() {
        FactoryItemSlottedInventory tmpLayout;
        for (int i = 0; i < SystemEntities.Count; i++) {
            tmpLayout = SystemEntities[i].GetComponent<FactoryItemSlottedInventory>();
            if (tmpLayout != null) {
                SpawnRandomItemsOnSlotLayout(tmpLayout);
            }
        }
    }

    //DEBUG FUNCTION - Spawns test items in slots at set percentage
    private void SpawnRandomItemsOnSlotLayout(FactoryItemSlottedInventory slotLayout) {
        GameObject tmpObj;
        GameItem tmpItem;
        int spawnPercent = 35;
        //Debug.Log("Number of slots on belt: " + belt.ItemSlots.Count + "!");
        for (int i = 0; i < slotLayout.ItemSlots.Count; i++) {
            if (UnityEngine.Random.Range(0, 100) < spawnPercent) {
                tmpItem = testItemPrefab.GetComponent<GameItem>();
                if (tmpItem != null) {
                    //Debug.Log("Slot Position: " + itemSlots[i].WorldPosition);
                    tmpObj = Instantiate(testItemPrefab, slotLayout.ItemSlots[i].WorldPosition, slotLayout.transform.rotation);
                    slotLayout.ItemSlots[i].WorldGameItem = tmpObj.GetComponent<GameItem>();
                }
                else {
                    Debug.LogError("Test GameItem is null!");
                }
            }
        }
    }
    #endregion
}