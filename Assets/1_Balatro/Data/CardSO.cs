using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Balatro/CardSO")]
public class CardSO : SingletonScriptableObject<CardSO>
{
    public List<CardDataShop> cardDataShops = new List<CardDataShop>();
    
    public List<CardData> GetCardDataByType(ItemType type)
    {
        foreach (var cardDataShop in cardDataShops)
        {
            if (cardDataShop.itemType == type)
            {
                return cardDataShop.cards;
            }
        }
        return new List<CardData>();
    }
}
[System.Serializable]
public class CardDataShop
{
    public ItemType itemType;
    public List<CardData> cards = new List<CardData>();
}
[System.Serializable]
public class CardData
{
    public string cardName;
    public string description;
    public Sprite image;
    public int baseCost;
    public GameObject cardBase;
}
