using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/TheFool")]
public class TheFool : VoucherEffectBase
{
    private int recipeId;
    private Recipe recipe;
    public override void ApplyEffect()
    {
        recipeId = Random.Range(0, RecipeDatabaseSO.Instance.defaultRecipes.Count);
        recipe = RecipeDatabaseSO.Instance.defaultRecipes[recipeId].recipe;
        RecipeManager.UpgradeRecipe(recipe);
    }
}
