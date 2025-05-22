using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaBox : BaseBox
{
    public static GachaBox _instance;
    public static GachaBox Setup(BoosterSize boosterSize)
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<GachaBox>(PathPrefabs.GACHA_BOX));
            _instance.Init();
        }
        _instance.InitState(boosterSize);
        return _instance;
    }
    
    public GachaNormal gachaNormal;
    public GachaJumbo gachaJumbo;
    public GachaMega gachaMega;

    private void InitState(BoosterSize boosterSize)
    {
        switch (boosterSize)
        {
            case BoosterSize.Normal:
                gachaNormal.Init();
                gachaJumbo.gameObject.SetActive(false);
                gachaMega.gameObject.SetActive(false);
                break;
            case BoosterSize.Jumbo:
                gachaNormal.gameObject.SetActive(false);
                gachaJumbo.Init();
                gachaMega.gameObject.SetActive(false);
                break;
            case BoosterSize.Mega:
                gachaNormal.gameObject.SetActive(false);
                gachaJumbo.gameObject.SetActive(false);
                gachaMega.Init();
                break;
        }
    }

    private void Init()
    {
        
    }
}
