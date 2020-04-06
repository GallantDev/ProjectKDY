using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSystemMono : MonoGameSystem {
    private List<ConveyorBelt> conveyorBelts;
    private List<ConveyorPath> conveyorPaths;

    [SerializeField]
    //MUST BE LESS THAN ROW COUNT
    private int columnCount = 0;
    [SerializeField]
    //MUST BE MORE THAN COLUMN COUNT
    private int rowCount = 0;

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

    [SerializeField]
    private GameObject testItemPrefab = null;

    public override void Init() {
        conveyorBelts = new List<ConveyorBelt>();
        conveyorBelts.AddRange(FindObjectsOfType<ConveyorBelt>());
        for (int i = 0; i < conveyorBelts.Count; i++) {
            InitializeConveyorBelt(conveyorBelts[i]);
            SpawnRandomItems(conveyorBelts[i]);
        }
        conveyorPaths = new List<ConveyorPath>();
        print("Number of belts: " + conveyorBelts.Count);
        GenerateConveyorPaths();
        print("Number of paths: " + conveyorPaths.Count);
        for (int x = 0; x < conveyorPaths.Count; x++) {
            print("Conveyor Path " + x + " has " + conveyorPaths[x].NodePath.Count + " nodes!");
        }
    }

    public override IEnumerator Execute() {
        //move items on all conveyor belts
        print("Conveyor System: Execute");
        MoveConveyorItems();
        yield break;
    }

    //Initializes all data related to conveyor belt
    private void InitializeConveyorBelt(ConveyorBelt belt) {
        belt.LocalTrans = belt.transform;
        belt.OutputTile = GetOutputBelt(belt);
        InitializeConveyorBeltSlots(belt);
    }

    //Initializes the layout of conveyor slots)
    private void InitializeConveyorBeltSlots(ConveyorBelt belt) {
        if (UniformTransformTest(belt) == false) {
            throw new Exception("Conveyor belt visual transform is not uniform!");
        }
        if (SlotLayoutTest() == false) {
            throw new Exception("Conveyor belt slot layout not valid! - (row count must be >= column count)");
        }

        halfBeltWidth = belt.BeltTrans.localScale.x / 2.0f;
        beltHeight = belt.BeltTrans.localScale.y;
        halfBeltHeight = belt.BeltTrans.localScale.y / 2.0f;
        halfBeltLength = belt.BeltTrans.localScale.z / 2.0f;

        slotWidth = belt.BeltTrans.localScale.x / columnCount;
        halfSlotWidth = slotWidth / 2.0f;
        slotHeight = slotWidth;
        halfSlotHeight = slotHeight / 2.0f;
        slotLength = belt.BeltTrans.localScale.z / rowCount;
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

        belt.ItemSlots = new List<WorldGameItemSlot>();

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
        startPos = belt.LocalTrans.position + (belt.LocalTrans.up * beltHeight);
        //Instantiate(testItemPrefab, startPos, belt.BeltTrans.rotation);
        for (int i = 0; i < columnCount; i++) {
            colPos = startPos + (-belt.BeltTrans.right * columnDistances[i]);
            for (int z = 0; z < rowCount; z++) {
                rowPos = colPos + (belt.BeltTrans.forward * rowDistances[z]);
                belt.ItemSlots.Add(new WorldGameItemSlot(rowPos));
            }
        }
    }

    //Checks for scale uniformity of conveyor belt transform
    private bool UniformTransformTest(ConveyorBelt belt) {
        if (belt.BeltTrans.localScale.x == belt.BeltTrans.localScale.y && belt.BeltTrans.localScale.x == belt.BeltTrans.localScale.z) {
            if (belt.BeltTrans.localScale.y == belt.BeltTrans.localScale.z) {
                return true;
            }
        }
        return false;
    }

    //Checks for valid slot layouts (rowCount >= columnCount)
    private bool SlotLayoutTest() {
        if (rowCount >= columnCount) {
            return true;
        }
        return false;
    }

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

    //Returns the angle difference of two different belts
    private float GetTileAngleDifference(ConveyorBelt belt1, ConveyorBelt belt2) {
        return Vector3.Angle(belt1.LocalTrans.forward, belt2.LocalTrans.forward);
    }

    //Returns the linked output conveyor belt
    private ConveyorBelt GetOutputBelt(ConveyorBelt belt) {
        RaycastHit hitInfo;
        Vector3 rayOrigin = belt.LocalTrans.position + (belt.LocalTrans.up * (belt.BeltTrans.localScale.y / 2.0f));
        Vector3 rayDir = belt.LocalTrans.forward;
        if (Physics.Raycast(rayOrigin, rayDir, out hitInfo, transform.localScale.z)) {
            ConveyorBelt tmpTile = hitInfo.collider.GetComponent<ConveyorBelt>();
            if (tmpTile != null) {
                return tmpTile;
            }
        }
        return null;
    }

    //Returns the linked input conveyor belts
    private List<ConveyorBelt> GetInputBelts(ConveyorBelt belt) {
        List<ConveyorBelt> tmpTiles = new List<ConveyorBelt>();
        ConveyorBelt tmpTile;
        List<Vector3> beltRayDirections = new List<Vector3>();
        Vector3 beltCenterPos;
        Vector3 tmpPos;
        RaycastHit hitInfo;
        beltRayDirections.Add(-belt.LocalTrans.forward);
        beltRayDirections.Add(-belt.LocalTrans.right);
        beltRayDirections.Add(belt.LocalTrans.right);
        beltCenterPos = belt.LocalTrans.position + (belt.LocalTrans.up * (belt.BeltTrans.localScale.y / 2.0f));
        for (int i = 0; i < beltRayDirections.Count; i++) {
            if (Physics.Raycast(beltCenterPos, beltRayDirections[i], out hitInfo, (belt.transform.localScale.magnitude / 3.0f))) {
                tmpTile = hitInfo.collider.GetComponent<ConveyorBelt>();
                if (tmpTile != null) {
                    if (GetOutputBelt(tmpTile) == belt) {
                        tmpTiles.Add(tmpTile);
                    }
                }
            }
        }
        if (tmpTiles.Count > 0) {
            return tmpTiles;
        }
        else {
            return null;
        }
    }

    //Returns the destination slot
    private WorldGameItemSlot GetDestinationItemSlot(ConveyorBelt belt, int slotIndex) {
        WorldGameItemSlot tmpSlot = null;
        if (DestinationSlotLocalityTest(slotIndex) == true) {
            tmpSlot = belt.ItemSlots[slotIndex - 1];
            if (tmpSlot.SlotItem != null) {
                return null;
            }
            else {
                return tmpSlot;
            }
        }
        else {
            //check neighboring conveyor belt for dest slot
            if (belt.OutputTile != null) {
                float angleDiff = Mathf.RoundToInt(Vector3.SignedAngle(belt.LocalTrans.forward, belt.OutputTile.LocalTrans.forward, Vector3.up));
                if (angleDiff == 0) {
                    //print("Forward (No Angle)!");
                    tmpSlot = belt.OutputTile.ItemSlots[slotIndex + (rowCount - 1)];
                }
                else if (angleDiff == -90.0f) {
                    //print("Left Angle!");
                    //print("From local slot " + slotIndex + " to external slot " + GetDestinationItemSlotIndexLeftAngle(slotIndex));
                    tmpSlot = belt.OutputTile.ItemSlots[GetDestinationItemSlotIndexLeftAngle(slotIndex)];
                }
                else if (angleDiff == 90.0f) {
                    //print("Right Angle!");
                    print("From local slot " + slotIndex + " to external slot " + GetDestinationItemSlotIndexRightAngle(slotIndex));
                    tmpSlot = belt.OutputTile.ItemSlots[GetDestinationItemSlotIndexRightAngle(slotIndex)];
                }
                if (tmpSlot.SlotItem == null) {
                    return tmpSlot;
                }
            }
        }
        return null;
    }

    //CHECK IF ALL CASES ARE WORKING!
    //Used for left angle belt transfers
    private int GetDestinationItemSlotIndexLeftAngle(int slotIndex) {
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
                //WORKING
                //check for even gap case
                if (columnCount > 2 && (rowCount - columnCount) % (columnCount - 1) == 0) {
                    return (slotIndex / rowCount) * ((rowCount - columnCount) / (columnCount - 1) + 1);
                }
                //WORKING
                //outside only case
                else {
                    if (slotIndex / rowCount < columnCount / 2) {
                        return (slotIndex / rowCount);
                    }
                    else {
                        return (slotIndex / rowCount) + (rowCount - columnCount);
                    }
                }
            }
        }
    }

    //Used for right angle belt transfers
    private int GetDestinationItemSlotIndexRightAngle(int slotIndex) {
        int slotCount = rowCount * columnCount;
        //WORKING
        if (columnCount == 1) {
            return (slotCount - 1) / 2;
        }
        else {
            //WORKING
            //same count case
            if (columnCount == rowCount) {
                return (slotCount - 1) - (slotIndex / rowCount);
            }
            else {
                //WORKING
                //check for even gap case
                if (columnCount > 2 && (rowCount - columnCount) % (columnCount - 1) == 0) {
                    return (slotCount - 1) - ((slotIndex / rowCount) * ((rowCount - columnCount) / (columnCount - 1) + 1));
                }
                //WORKING
                //outside only case
                else {
                    if (slotIndex / rowCount < columnCount / 2) {
                        return (slotCount - 1) - (slotIndex / rowCount);
                    }
                    else {
                        return (slotCount - 1) - ((slotIndex / rowCount) + (rowCount - columnCount));
                    }
                }
            }
        }
    }

    //Generates conveyor paths to process belt item movements in order
    private void GenerateConveyorPaths() {
        List<ConveyorBelt> tmpPathEnds = new List<ConveyorBelt>();
        List<ConveyorPathNode> tmpPathNodes = new List<ConveyorPathNode>();
        List<ConveyorPathNode> tmpSidePathNodes = new List<ConveyorPathNode>();
        List<ConveyorBelt> tmpTiles = new List<ConveyorBelt>();
        List<ConveyorBelt> tmpOldInputTiles = new List<ConveyorBelt>();
        List<ConveyorBelt> tmpNewInputTiles = new List<ConveyorBelt>();
        float tmpAngleDiff;
        int tmpDist;
        for (int i = 0; i < conveyorBelts.Count; i++) {
            if (conveyorBelts[i].OutputTile == null) {
                tmpPathEnds.Add(conveyorBelts[i]);
            }
        }
        for (int x = 0; x < tmpPathEnds.Count; x++) {
            tmpPathNodes.Clear();
            tmpNewInputTiles = new List<ConveyorBelt>();
            tmpDist = 0;
            //Add first path node (path end tile) manually
            tmpPathNodes.Add(new ConveyorPathNode(tmpPathEnds[x], tmpDist, 0.0f));
            tmpDist++;
            tmpTiles = GetInputBelts(tmpPathEnds[x]);
            if (tmpTiles != null) {
                tmpOldInputTiles.AddRange(GetInputBelts(tmpPathEnds[x]));
            }
            while (tmpNewInputTiles != null) {
                if (tmpOldInputTiles.Count == 0) {
                    tmpNewInputTiles = null;
                    break;
                }
                else {
                    for (int y = 0; y < tmpOldInputTiles.Count; y++) {
                        //print("Conveyor Path End" + x + ", Node " + (tmpPathNodes.Count + y));
                        tmpAngleDiff = GetTileAngleDifference(tmpOldInputTiles[y], GetOutputBelt(tmpOldInputTiles[y]));
                        if (tmpAngleDiff == 0.0f) {
                            tmpPathNodes.Add(new ConveyorPathNode(tmpOldInputTiles[y], tmpDist, tmpAngleDiff));
                        }
                        else {
                            tmpSidePathNodes.Add(new ConveyorPathNode(tmpOldInputTiles[y], tmpDist, tmpAngleDiff));
                        }
                        tmpTiles = GetInputBelts(tmpOldInputTiles[y]);
                        if (tmpTiles != null) {
                            tmpNewInputTiles.AddRange(GetInputBelts(tmpOldInputTiles[y]));
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
            conveyorPaths.Add(new ConveyorPath(tmpPathNodes));
        }
    }

    //Moves items along all conveyors on a path by path basis
    private void MoveConveyorItems() {
        WorldGameItemSlot destSlot;
        for (int i = 0; i < conveyorPaths.Count; i++) {
            for (int x = 0; x < conveyorPaths[i].NodePath.Count; x++) {
                //conveyorPaths[i].NodePath[x].Conveyor
                for (int y = 0; y < conveyorPaths[i].NodePath[x].Conveyor.ItemSlots.Count; y++) {
                    //print("Conveyor Path - " + i + ", Conveyor Node - " + x + ", Conveyor Slot - " + y + ", Slot Item - " + conveyorPaths[i].NodePath[x].Conveyor.ItemSlots[i].SlotItem);
                    if (conveyorPaths[i].NodePath[x].Conveyor.ItemSlots[y].SlotItem != null) {
                        destSlot = GetDestinationItemSlot(conveyorPaths[i].NodePath[x].Conveyor, y);
                        if (destSlot != null) {
                            destSlot.SlotItem = conveyorPaths[i].NodePath[x].Conveyor.ItemSlots[y].SlotItem;
                            destSlot.SlotItem.transform.position = destSlot.WorldPosition;
                            conveyorPaths[i].NodePath[x].Conveyor.ItemSlots[y].SlotItem = null;
                        }
                    }
                }
            }
        }
    }

    //DEBUG FUNCTION - Spawns test items in slots at set percentage
    private void SpawnRandomItems(ConveyorBelt belt) {
        GameObject tmpObj;
        WorldGameItem tmpItem;
        int spawnPercent = 35;
        //Debug.Log("Number of slots on belt: " + belt.ItemSlots.Count + "!");
        for (int i = 0; i < belt.ItemSlots.Count; i++) {
            if (UnityEngine.Random.Range(0, 100) < spawnPercent) {
                tmpItem = testItemPrefab.GetComponent<WorldGameItem>();
                if (tmpItem != null) {
                    //Debug.Log("Slot Position: " + itemSlots[i].WorldPosition);
                    tmpObj = Instantiate(testItemPrefab, belt.ItemSlots[i].WorldPosition, belt.BeltTrans.rotation);
                    belt.ItemSlots[i].SlotItem = tmpObj.GetComponent<WorldGameItem>();
                }
                else {
                    Debug.LogError("Test GameItem is null!");
                }
            }
        }
    }
}