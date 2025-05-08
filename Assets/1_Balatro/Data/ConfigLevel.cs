using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConfigLevel", menuName = "Balatro/ConfigLevel")]
public class ConfigLevel : SingletonScriptableObject<ConfigLevel>
{
    public List<DataPerAnte> lv = new List<DataPerAnte>();

    private void OnValidate()
    {
        for (int i = 0; i < lv.Count; i++)
        {
            lv[i].Ante = i + 1;
        }
    }
    public int GetPlay(int ante, int round)
    {
        foreach(var item in lv)
        {
            if (item.Ante == ante)
            {
                foreach (var data in item.dataPerLevel)
                {
                    if (data.Lv == round)
                    {
                        return data.playHand;
                    }
                }
            }
        }
        return 0;
    }
    public int GetDis(int ante, int round)
    {
        foreach (var item in lv)
        {
            if (item.Ante == ante)
            {
                foreach (var data in item.dataPerLevel)
                {
                    if (data.Lv == round)
                    {
                        return data.discard;
                    }
                }
            }
        }
        return 0;
    }
}
[System.Serializable]
public class DataPerAnte
{
    public int Ante;
    [TableList]
    public List<DataPerLevel> dataPerLevel;
}
[System.Serializable]
public class DataPerLevel
{
    public int Lv;
    public int target;
    public int playHand;
    public int discard;
    public int reward;
}