using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoosterType { Standard, Recipe, Celestial, Spectral, Buffoon }
public enum BoosterSize { Normal, Jumbo, Mega }

[CreateAssetMenu(fileName = "BoosterBase", menuName = "Balatro/BoosterBase")]
public class BoosterBase : SingletonScriptableObject<BoosterBase>
{
    public string packName;
    public BoosterType type;
    public BoosterSize size;
    public Sprite artwork;
    public int cost;
    public int numberOfChoices;
    public int numberOfSelections;

    // Các tham số về phân phối thẻ trong booster
    public ItemType[] possibleCardTypes;
    public float[] cardTypeWeights;
}