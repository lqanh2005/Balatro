using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoucherBase : CardBase
{

    public Image avt;
    public VoucherData voucherData;
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
        voucherData.Activate();
    }
}
