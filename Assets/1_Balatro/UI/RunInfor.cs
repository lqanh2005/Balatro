using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class RunInfor : BaseBox
{
    private static RunInfor _instance;
    public List<RecipeUI> recipeUIs;
    public Button back;

    public static RunInfor Setup()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<RunInfor>(PathPrefabs.Run_Infor));
            _instance.Init();
        }
        _instance.InitState();
        return _instance;
    }
    public void Init()
    {
        foreach(var item in recipeUIs)
        {
            item.Init();
        }
        back.onClick.AddListener(delegate { this.Close(); });
    }
    public void InitState()
    {
        Debug.LogError("Show");
    }
}
