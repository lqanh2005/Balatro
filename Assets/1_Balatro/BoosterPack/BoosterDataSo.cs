using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoosterDataSo", menuName = "Balatro/BoosterDataSo")]
public class BoosterDataSo : SingletonScriptableObject<BoosterDataSo>
{
    public List<BoosterData> boosters = new List<BoosterData>();
    public BoosterData GetBooster(int id)
    {
        return boosters[id];
    }
}

[System.Serializable]
public class BoosterData
{
    public string packName;
    public BoosterSize size;
    public Sprite artwork;
    public int cost;
    public string description;
    public int numberOfChoices;
    public int numberOfSelections;
}