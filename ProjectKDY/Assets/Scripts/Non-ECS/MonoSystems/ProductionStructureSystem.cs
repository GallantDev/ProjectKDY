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
        for (int x = 0; x < productionStructures[entIndex].CurrentRecipe.OutputItems.Count; x++) {
            tmpStack = GetExistingOutputItemStackFromProductionInventory(productionStructures[entIndex].CurrentRecipe.OutputItems[x].ItemData, productionInventories[entIndex]);
            if (tmpStack == null) {
                productionInventories[entIndex].OutputItemStacks.Add(new GameItemStack(productionStructures[entIndex].CurrentRecipe.OutputItems[x].ItemData, productionStructures[entIndex].CurrentRecipe.OutputItems[x].ItemCount));
            }
            else {
                tmpStack.ItemCount += productionStructures[entIndex].CurrentRecipe.OutputItems[x].ItemCount;
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

    //private void ChangeRecipe(int entIndex, int recipeIndex) {
    //    productionStructures[entIndex].CurrentRecipe = productionStructures[entIndex].ItemRecipes[recipeIndex];
    //    ClearOldRecipeItems();
    //}

    //private void ClearOldRecipeItems(int entIndex) {
    //    for (int i = 0; i < productionInventories[entIndex].InputItemStacks.Count; i++) {
    //        if (productionStructures[entIndex].CurrentRecipe.InputItemSets.IsInputItemType()) {

    //        }
    //    }
    //    for (int x = 0; x < productionInventories[entIndex].OutputItemStacks.Count; x++) {
    //        if (productionStructures[entIndex].CurrentRecipe.OutputItemSets.IsOutputItemType()) {

    //        }
    //    }
    //}
}