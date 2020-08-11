using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryItemProduction : MonoComponent {
    [SerializeField]
    private List<GameItemRecipe> itemRecipes = new List<GameItemRecipe>();
    [SerializeField]
    private GameItemRecipe currentRecipe = null;
    [SerializeField]
    private bool isProducing = false;

    public List<GameItemRecipe> ItemRecipes {
        get {
            return itemRecipes;
        }

        set {
            itemRecipes = value;
        }
    }

    public GameItemRecipe CurrentRecipe {
        get {
            return currentRecipe;
        }

        set {
            currentRecipe = value;
        }
    }

    public bool IsProducing {
        get {
            return isProducing;
        }

        set {
            isProducing = value;
        }
    }
}