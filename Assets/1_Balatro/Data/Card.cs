using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Balatro/Card")]
public class Card : SingletonScriptableObject<Card>
{
    public string cardName;
    public string description;
    public Sprite artwork;
    public ItemType itemType;
    public int baseCost;

    // Card specific effects and properties
    public virtual void ApplyEffect() { }
}
