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
    public int id;

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
    
    public int chipBonus;
    public Sprite faceImage;
    public int cost;
}
