using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CardData", menuName ="Balatro/CardData")]
public class ConfigData : SingletonScriptableObject<ConfigData>
{
    public List<CardDataList> cardLists;
    public List<Image> backCard;
}

[System.Serializable]
public class CardDataList
{
    public IngredientType cardType;
    public List<CardPerLevel> cardPerLevels;
}
[System.Serializable]
public class CardPerLevel
{
    public int level;
    public CardData cardDatas;
}

[System.Serializable]
public class CardData
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