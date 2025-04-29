using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager
{
    public static void SaveAll(Dictionary<Recipe, RecipeData> recipeDict)
    {
        RecipeCollection collection = new RecipeCollection();
        foreach (var kvp in recipeDict)
        {
            collection.recipes.Add(new RecipeEntry
            {
                recipeName = kvp.Key.ToString(),
                data = kvp.Value
            });
        }
        string json = JsonUtility.ToJson(collection);
        PlayerPrefs.SetString(StringHelper.RECIPT_ALL, json);
        PlayerPrefs.Save();
    }
    public static RecipeData GetRecipe(Recipe recipe)
    {
        var all = LoadAll();
        return all.TryGetValue(recipe, out var data) ? data : new RecipeData();
    }

    public static void SetRecipe(Recipe recipe, RecipeData data)
    {
        var all = LoadAll();
        all[recipe] = data;
        SaveAll(all);
    }

    public static void ResetAllRecipe(RecipeDatabaseSO dataBase)
    {
        Dictionary<Recipe, RecipeData> defaultData = new();
        foreach (var entry in dataBase.defaultRecipes)
        {
            defaultData[entry.recipe] = new RecipeData(entry.defaultCoin, entry.defaultMulti, entry.defaultLevel);
        }
        foreach(Recipe recipe in Enum.GetValues(typeof(Recipe)))
        {
            if (!defaultData.ContainsKey(recipe))
            {
                defaultData[recipe] = new RecipeData();
            }
        }
        SaveAll(defaultData);
    }
    public static Dictionary<Recipe, RecipeData> LoadAll(RecipeDatabaseSO dataBase = null)
    {
        Dictionary<Recipe, RecipeData> result = new();
        if (!PlayerPrefs.HasKey(StringHelper.RECIPT_ALL))
        {
            if (dataBase != null)
            {
                foreach (var entry in dataBase.defaultRecipes)
                {
                    result[entry.recipe] = new RecipeData(
                        entry.defaultLevel,
                        entry.defaultCoin,
                        entry.defaultMulti
                    );
                }
                foreach (Recipe recipe in Enum.GetValues(typeof(Recipe)))
                {
                    result[recipe] = new RecipeData();
                }
            }
            else
            {
                foreach (Recipe recipe in Enum.GetValues(typeof(Recipe)))
                {
                    result[recipe] = new RecipeData();
                }
            }
            SaveAll(result);
            return result;
        }
        string json = PlayerPrefs.GetString(StringHelper.RECIPT_ALL);
        RecipeCollection collection = JsonUtility.FromJson<RecipeCollection>(json);
        foreach (var entry in collection.recipes)
        {
            if (Enum.TryParse(entry.recipeName, out Recipe parsed))
            {
                result[parsed] = entry.data;
            }
        }
        return result;
    }
    public static void CheckAndUpdateDatabase(RecipeDatabaseSO dataBase)
    {
        var currentData = LoadAll();
        bool hasChanges = false;
        foreach(var entry in dataBase.defaultRecipes)
        {
            if(!currentData.ContainsKey(entry.recipe))
            {
                currentData[entry.recipe] = new RecipeData(entry.defaultLevel, entry.defaultCoin, entry.defaultMulti);
                hasChanges = true;
            }
        }
        if (hasChanges)
        {
            SaveAll(currentData);
            Debug.LogError("Database updated with new recipes.");
        }
    }
}

[System.Serializable]
public class RecipeCollection
{
    public List<RecipeEntry> recipes = new List<RecipeEntry>();
}
[System.Serializable]
public class RecipeEntry
{
    public string recipeName;
    public RecipeData data;
}

[System.Serializable]
public class RecipeData
{
    public int level;
    public int coin;
    public int multi;
    public RecipeData(int level= 1, int coin = 0, int multi = 1)
    {
        this.level = level;
        this.coin = coin;
        this.multi = multi;
    }
}
