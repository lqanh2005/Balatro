using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoosterSize { Normal, Jumbo, Mega }

public class BoosterBase : CardBase
{
    public Image avt;
    public BoosterData boosterData;
    public Canvas canvas;
    public override void Init()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 10;
        OnActive();
    }

    public override void OnActive()
    {
        GachaBox.Setup(boosterData.size).Show();
    }
}