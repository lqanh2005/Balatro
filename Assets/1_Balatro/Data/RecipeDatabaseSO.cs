using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Balatro/RecipeData", fileName = "RecipeData")]
public class RecipeDatabaseSO : ScriptableObject
{
    public List<RecipeDefaultEntry> defaultRecipes = new List<RecipeDefaultEntry>();
}

[System.Serializable]
public class RecipeDefaultEntry
{
    public Recipe recipe;
    public List<IngredientType> ingredients;
    public int coin = 0;
    public int multi = 1;
    public int level = 1;
}