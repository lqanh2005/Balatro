using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeChecker
{
    public static RecipeDatabaseSO recipeDatabase { get; private set; }
    private static Recipe currentRecipe = Recipe.None;


    public static void Init(RecipeDatabaseSO database)
    {
        recipeDatabase = database;
    }

    public static Recipe GetMatchedRecipe(List<PlayingCard> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count == 0)
        {
            currentRecipe = Recipe.None;
            UpdateUIForNoMatch();
            return Recipe.None;
        }

        List<IngredientType> selectedIngredients = selectedCards
            .Select(card => card.ingredientType)
            .ToList();
        var matchedEntry = recipeDatabase.FindMatchingRecipe(selectedIngredients);

        if (matchedEntry != null)
        {
            currentRecipe = matchedEntry.recipe;
            UpdateUI(currentRecipe, matchedEntry.recipeName);
            return currentRecipe;
        }
        else if (selectedCards.Count < 3)
        {
            currentRecipe = Recipe.Ingredient;
            UpdateUI(currentRecipe, "Mậu Thầu");
            return currentRecipe;
        }
        if (currentRecipe != Recipe.None)
        {
            UpdateUI(currentRecipe, "Mậu Thầu");
            return currentRecipe;
        }
        UpdateUIForNoMatch();
        return Recipe.None;
    }
    public static List<PlayingCard> GetCardsToScore(List<PlayingCard> selectedCards, Recipe recipe)
    {
        if (recipe == Recipe.None || selectedCards == null)
            return null;
        if (recipe == Recipe.Ingredient)
        {
            var maxCard = selectedCards.OrderByDescending(card => card.chip).FirstOrDefault();
            return maxCard != null ? new List<PlayingCard> { maxCard } : null;
        }
        var recipeEntry = recipeDatabase.GetRecipeEntry(recipe);
        if (recipeEntry == null)
            return null;

        var ingredients = new List<IngredientType>(recipeEntry.ingredients);
        var cardsToScore = new List<PlayingCard>();

        foreach (var card in selectedCards)
        {
            if (ingredients.Contains(card.ingredientType))
            {
                cardsToScore.Add(card);
                ingredients.Remove(card.ingredientType);
            }
        }

        return cardsToScore;
    }

    private static void UpdateUI(Recipe recipe, string name)
    {
        RecipeData data = RecipeManager.GetRecipe(recipe);
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.recipe, name + " lv." + data.level);
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.coin, data.coin);
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.multi, data.multi);
    }
    private static void UpdateUIForNoMatch()
    {
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.recipe, null);
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.coin, 0);
        EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.multi, 0);
    }

}
