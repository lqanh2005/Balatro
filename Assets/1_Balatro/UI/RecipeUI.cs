using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RecipeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipeType;

    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text multiText;
    [SerializeField] private TMP_Text cnt;

    private RecipeData _currentData;
    public RecipeData CurrentData => _currentData;
    public void Init()
    {
        _currentData = RecipeManager.GetRecipe(recipeType);
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (_currentData == null)
        {
            _currentData = RecipeManager.GetRecipe(recipeType);
        }
        recipeNameText.text = recipeType.ToString();
        levelText.text = "lv." + _currentData.level.ToString();
        coinText.text =  _currentData.coin.ToString();
        multiText.text = _currentData.multi.ToString();
    }
    public void SetData(Recipe recipe, RecipeData data)
    {
        recipeType = recipe;
        _currentData = data;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogError("Mouse Entered Recipe UI");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.LogError("Mouse Exit Recipe UI");
    }

}
public struct RecipeDataUI
{
    public int level;
    public string recipeName;
    public int coin;
    public int multi;

    public RecipeDataUI(int level, string recipeName, int coin, int multi)
    {
        this.level = level;
        this.recipeName = recipeName;
        this.coin = coin;
        this.multi = multi; 
    }
}
