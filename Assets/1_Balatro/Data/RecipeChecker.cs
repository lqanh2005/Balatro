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

    public static Recipe? GetMatchedRecipe(List<CardBase> selectedCards)
    {
        List<IngredientType> selectedIngredients = selectedCards
            .Select(card => card.ingredientType)
            .ToList();

        foreach (var recipe in recipeDatabase.defaultRecipes)
        {
            if (IsMatch(recipe.ingredients, selectedIngredients))
            {
                GamePlayController.Instance.uICtrl.recipe.text = recipe.recipe.ToString();
                GamePlayController.Instance.uICtrl.coin.text = recipe.coin.ToString();
                GamePlayController.Instance.uICtrl.multi.text = recipe.multi.ToString();
                return recipe.recipe;
            }
        }
        GamePlayController.Instance.uICtrl.recipe.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].recipe.ToString();
        GamePlayController.Instance.uICtrl.coin.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].coin.ToString();
        GamePlayController.Instance.uICtrl.multi.text = recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].multi.ToString();
        return recipeDatabase.defaultRecipes[recipeDatabase.defaultRecipes.Count - 1].recipe;
    }

    private static bool IsMatch(List<IngredientType> required, List<IngredientType> selected)
    {
        if (required.Count != selected.Count)
            return false;

        var requiredCopy = new List<IngredientType>(required);
        foreach (var ingredient in selected)
        {
            if (!requiredCopy.Remove(ingredient))
                return false;
        }

        return requiredCopy.Count == 0;
    }
}
