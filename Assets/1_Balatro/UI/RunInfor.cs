using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class RunInfor : BaseBox
{
    public static RunInfor _instance;
    public List<RecipeUI> recipeUIs;
    public Button back;
    //public Button 
    public RecipeDatabaseSO recipeDatabaseSO;
    public Formula formula;
    public static RunInfor Setup()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<RunInfor>(PathPrefabs.Run_Infor));
            _instance.Init();
        }
        _instance.InitState();
        return _instance;
    }
    public void Init()
    {

        foreach(var item in recipeUIs)
        {
            item.Init();
        }
        back.onClick.AddListener(delegate { this.Close(); GameController.Instance.musicManager.PlayClickSound(); GamePlayController.Instance.isLevelDone = true; });
    }
    public void InitState()
    {
        RecipeManager.LoadAll(recipeDatabaseSO);
        LoadAllRecipeData();
    }

    public void LoadAllRecipeData()
    {
        Dictionary<Recipe, RecipeData> allRecipes = RecipeManager.LoadAll(recipeDatabaseSO);

        UpdateAllRecipeUIs(allRecipes);
    }
    private void UpdateAllRecipeUIs(Dictionary<Recipe, RecipeData> allRecipes)
    {
        foreach (var recipeUI in recipeUIs)
        {
            if (allRecipes.ContainsKey(recipeUI.recipeType))
            {
                recipeUI.SetData(recipeUI.recipeType, allRecipes[recipeUI.recipeType]);
                recipeUI.UpdateUI();
            }
        }
    }
}
