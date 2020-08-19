using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructureSystem : MonoGameSystem {
    private List<FactoryItemProduction> productionStructures = new List<FactoryItemProduction>();
    private List<FactoryItemProductionInventory> productionInventories = new List<FactoryItemProductionInventory>();

    public List<FactoryItemProduction> ProductionStructures {
        get {
            return productionStructures;
        }

        set {
            productionStructures = value;
        }
    }

    public List<FactoryItemProductionInventory> ProductionInventories {
        get {
            return productionInventories;
        }

        set {
            productionInventories = value;
        }
    }

    public override void Init() {
        print("Production System: Init!");
    }

    public override IEnumerator Execute() {
        print("Production System: Execute!");
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    for (int i = 0; i < SystemEntities.Count; i++) {
        //        ChangeRecipe(i, 1);
        //    }
        //}
        for (int i = 0; i < SystemEntities.Count; i++) {
            CheckForProductionRequirements(i);
        }
        yield break;
    }

    private void CheckForProductionRequirements(int entIndex) {
        GameItemStack tmpStack = null;
        for (int i = 0; i < productionStructures[entIndex].CurrentRecipe.InputItemSets.Count; i++) {
            tmpStack = GetExistingInputItemStackFromProductionInventory(productionStructures[entIndex].CurrentRecipe.InputItemSets[i].ItemData, productionInventories[entIndex]);
            if (tmpStack == null || tmpStack.ItemCount < productionStructures[entIndex].CurrentRecipe.InputItemSets[i].ItemCount) {
                return;
            }
        }
        if (!productionStructures[entIndex].IsProducing) {
            StartCoroutine(ProduceItem(entIndex));
        }
    }

    private IEnumerator ProduceItem(int entIndex) {
        GameItemStack tmpStack = null;
        productionStructures[entIndex].IsProducing = true;
        yield return new WaitForSeconds(productionStructures[entIndex].CurrentRecipe.ProductionTime);
        for (int i = 0; i < productionStructures[entIndex].CurrentRecipe.InputItemSets.Count; i++) {
            RemoveInputItemStackFromProductionInventory(productionStructures[entIndex].CurrentRecipe.InputItemSets[i], productionInventories[entIndex]);
        }
        for (int x = 0; x < productionStructures[entIndex].CurrentRecipe.OutputItemSets.Count; x++) {
            tmpStack = GetExistingOutputItemStackFromProductionInventory(productionStructures[entIndex].CurrentRecipe.OutputItemSets[x].ItemData, productionInventories[entIndex]);
            if (tmpStack == null) {
                productionInventories[entIndex].OutputItemStacks.Add(new GameItemStack(productionStructures[entIndex].CurrentRecipe.OutputItemSets[x].ItemData, productionStructures[entIndex].CurrentRecipe.OutputItemSets[x].ItemCount));
            }
            else {
                tmpStack.ItemCount += productionStructures[entIndex].CurrentRecipe.OutputItemSets[x].ItemCount;
            }
        }
        productionStructures[entIndex].IsProducing = false;
        yield break;
    }

    private GameItemStack GetExistingInputItemStackFromProductionInventory(GameItemData itemData, FactoryItemProductionInventory prodInv) {
        for (int i = 0; i < prodInv.InputItemStacks.Count; i++) {
            if (itemData.Id == prodInv.InputItemStacks[i].ItemData.Id) {
                return prodInv.InputItemStacks[i];
            }
        }
        return null;
    }

    private GameItemStack GetExistingOutputItemStackFromProductionInventory(GameItemData itemData, FactoryItemProductionInventory prodInv) {
        for (int i = 0; i < prodInv.OutputItemStacks.Count; i++) {
            if (itemData.Id == prodInv.OutputItemStacks[i].ItemData.Id) {
                return prodInv.OutputItemStacks[i];
            }
        }
        return null;
    }

    private void RemoveInputItemStackFromProductionInventory(GameItemStack inputItemStack, FactoryItemProductionInventory outputProdInv) {
        for (int i = 0; i < outputProdInv.InputItemStacks.Count; i++) {
            if (outputProdInv.InputItemStacks[i].ItemData.Id == inputItemStack.ItemData.Id) {
                outputProdInv.InputItemStacks[i].ItemCount -= inputItemStack.ItemCount;
                if (outputProdInv.InputItemStacks[i].ItemCount == 0) {
                    outputProdInv.InputItemStacks.RemoveAt(i);
                }
                return;
            }
        }
    }

    private void ChangeRecipe(int entIndex, int recipeIndex) {
        productionStructures[entIndex].CurrentRecipe = productionStructures[entIndex].ItemRecipes[recipeIndex];
        ClearOldRecipeItems(entIndex);
    }

    private void ClearOldRecipeItems(int entindex) {
        List<GameItemStack> inputStacksToRemove = new List<GameItemStack>();
        List<GameItemStack> outputStacksToRemove = new List<GameItemStack>();
        //clear old input items
        for (int i = 0; i < productionInventories[entindex].InputItemStacks.Count; i++) {
            if (productionStructures[entindex].CurrentRecipe.IsInputItemType(productionInventories[entindex].InputItemStacks[i].ItemData.Id)) {
                //remove items if over the overflow limit
                if (productionInventories[entindex].InputItemStacks[i].ItemCount > productionStructures[entindex].CurrentRecipe.GetInputItemTypeLimit(productionInventories[entindex].InputItemStacks[i].ItemData.Id)) {
                    productionInventories[entindex].InputItemStacks[i].ItemCount = productionStructures[entindex].CurrentRecipe.GetInputItemTypeLimit(productionInventories[entindex].InputItemStacks[i].ItemData.Id);
                }
            }
            else {
                //remove items stack entirely
                inputStacksToRemove.Add(productionInventories[entindex].InputItemStacks[i]);
            }
        }
        //clear old output items
        for (int x = 0; x < productionInventories[entindex].OutputItemStacks.Count; x++) {
            if (productionStructures[entindex].CurrentRecipe.IsOutputItemType(productionInventories[entindex].OutputItemStacks[x].ItemData.Id)) {
                //remove items if over the overflow limit
            }
            else {
                //remove items stack entirely
                outputStacksToRemove.Add(productionInventories[entindex].OutputItemStacks[x]);
            }
        }
        //remove input stacks
        productionInventories[entindex].InputItemStacks.RemoveAll(tmpStack => inputStacksToRemove.Contains(tmpStack));
        //remove output stacks
        productionInventories[entindex].OutputItemStacks.RemoveAll(tmpStack => outputStacksToRemove.Contains(tmpStack));
    }
}