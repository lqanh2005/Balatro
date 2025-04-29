using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Balatro/RecipeData", fileName = "RecipeData")]
public class RecipeDatabaseSO : ScriptableObject
{
    public int dataBaseVersion = 1;
    public List<RecipeDefaultEntry> defaultRecipes = new List<RecipeDefaultEntry>();
    public RecipeDefaultEntry GetRecipeEntry(Recipe recipe)
    {
        return defaultRecipes.Find(entry => entry.recipe == recipe);
    }
    public RecipeDefaultEntry FindMatchingRecipe(List<IngredientType> ingredients)
    {
        return defaultRecipes.Find(entry => IsMatch(entry.ingredients, ingredients));
    }
    private bool IsMatch(List<IngredientType> required, List<IngredientType> selected)
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
    public List<IngredientType> GetCardsToScore(Recipe recipe)
    {
        foreach(var entry in defaultRecipes)
        {
            if(recipe == entry.recipe)
            {
                return entry.ingredients;
            }
        }
        return null;
    }
}

[System.Serializable]
public class RecipeDefaultEntry
{
    public Recipe recipe;
    public string recipeName;
    public List<IngredientType> ingredients;
    public int defaultCoin = 0;
    public int defaultMulti = 1;
    public int defaultLevel = 1;
}