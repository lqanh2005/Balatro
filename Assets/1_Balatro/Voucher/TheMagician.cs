using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/TheMagician")]
public class TheMagician : VoucherEffectBase
{
    private int idx;
    public override void ApplyEffect()
    {
        idx = Random.Range(0, RecipeDatabaseSO.Instance.defaultRecipes.Count-1);
        RecipeManager.UpgradeRecipe(RecipeDatabaseSO.Instance.defaultRecipes[idx].recipe);
    }
}
