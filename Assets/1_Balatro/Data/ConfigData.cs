using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ConfigData", menuName = "Balatro/ConfigData")]
public class ConfigData : SingletonScriptableObject<ConfigData>
{
    public List<CardDataList> cardLists;
    public List<Sprite> backCard;
    public TooltipAttribute tooltipAttribute;

    public int GetChip(IngredientType ingredientType, int level)
    {
        foreach (var cardList in cardLists)
        {
            if (cardList.ingredientType == ingredientType)
            {
                foreach (var cardPerLevel in cardList.cardPerLevels)
                {
                    if (cardPerLevel.level == level)
                    {
                        return cardPerLevel.cardDatas.chipBonus;
                    }
                }
            }
        }
        return 0;
    }
    public Sprite GetFaceCard(IngredientType ingredientType, int level)
    {
        foreach (var cardList in cardLists)
        {
            if (cardList.ingredientType == ingredientType)
            {
                foreach (var cardPerLevel in cardList.cardPerLevels)
                {
                    if (cardPerLevel.level == level)
                    {
                        return cardPerLevel.cardDatas.faceImage;
                    }
                }
            }
        }
        return null;
    }
}

[System.Serializable]
public class CardDataList
{
    public IngredientType ingredientType;
    public List<CardPerLevel> cardPerLevels;
}
[System.Serializable]
public class CardPerLevel
{
    public int level;
    public CardDataSO cardDatas;
}

[System.Serializable]
public class CardDataSO
{
    public int id;
    public int chipBonus;
    public Sprite faceImage;
}

public class CardEnhance
{
    public string name;
    public float multiplierBonus = 1f;
    public int chipBonus = 0;
}