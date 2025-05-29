using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeCard", menuName = "Balatro/RecipeCard")]
public class RecipeCard : SingletonScriptableObject<RecipeCard>
{
    public List<RecipeCardUI> dataRecipe = new List<RecipeCardUI>();
    public Sprite GetSprite(Recipe recipe)
    {
        foreach (var recipeCardUI in dataRecipe)
        {
            if (recipeCardUI.name == recipe)
            {
                return recipeCardUI.avt;
            }
        }
        return null;
    }
    public Sprite GetFormula(Recipe recipe)
    {
        foreach (var recipeCardUI in dataRecipe)
        {
            if (recipeCardUI.name == recipe)
            {
                return recipeCardUI.formula;
            }
        }
        return null;
    }
    public Sprite GetSpriteImage(Recipe recipe)
    {
        foreach (var recipeCardUI in dataRecipe)
        {
            if (recipeCardUI.name == recipe)
            {
                return recipeCardUI.avt;
            }
        }
        return null;
    }
}
[System.Serializable]
public class  RecipeCardUI
{
    public Recipe name;
    public Sprite avt;
    public Sprite formula;
}
