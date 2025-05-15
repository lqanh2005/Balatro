using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RecipeDataUI recipeData;
    public void Init()
    {

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
