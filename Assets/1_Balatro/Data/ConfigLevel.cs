
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
    public DataPerLevel GetData(int ante, int level)
    {
        foreach(var item in lv)
        {
            if(item.Ante == ante)
            {
                foreach(var data in item.dataPerLevel)
                {
                    if(data.Lv == level) return data;
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class DataPerAnte
{
    public int Ante;
    public List<DataPerLevel> dataPerLevel;
}
[System.Serializable]
public class DataPerLevel
{
    public int Lv;
    public int target;
    public int reward;
    public string name;
}