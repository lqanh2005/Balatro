using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeChecker
{
    public static RecipeDatabaseSO recipeDatabase { get; private set; }

    public static void Init(RecipeDatabaseSO database)
    {
        recipeDatabase = database;
    }

    public static Recipe GetMatchedRecipe(List<CardBase> selectedCards)
    {
        List<IngredientType> selectedIngredients = selectedCards
            .Select(card => card.ingredientType)
            .ToList();

        var matchedEntry = recipeDatabase.FindMatchingRecipe(selectedIngredients);
        if(matchedEntry != null)
        {
            UpdateUI(matchedEntry.recipe);
            return matchedEntry.recipe;
        }
        UpdateUIForNoMatch();
        return recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].recipe; ;
    }
    private static void UpdateUI(Recipe recipe)
    {
        RecipeData data = RecipeManager.GetRecipe(recipe);
        GamePlayController.Instance.uICtrl.recipe.text = recipe.ToString();
        GamePlayController.Instance.uICtrl.coin.text = data.coin.ToString();
        GamePlayController.Instance.uICtrl.multi.text = data.multi.ToString();
    }
    private static void UpdateUIForNoMatch()
    {
        GamePlayController.Instance.uICtrl.recipe.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].recipe.ToString();
        GamePlayController.Instance.uICtrl.coin.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].defaultCoin.ToString();
        GamePlayController.Instance.uICtrl.multi.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].defaultMulti.ToString();
    }

    //private static bool IsMatch(List<IngredientType> required, List<IngredientType> selected)
    //{
    //    if (required.Count != selected.Count)
    //        return false;

    //    var requiredCopy = new List<IngredientType>(required);
    //    foreach (var ingredient in selected)
    //    {
    //        if (!requiredCopy.Remove(ingredient))
    //            return false;
    //    }

    //    return requiredCopy.Count == 0;
    //}
}
