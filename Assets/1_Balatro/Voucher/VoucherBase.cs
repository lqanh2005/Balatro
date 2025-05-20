using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoucherBase : CardBase
{

    public Image avt;
    public VoucherData voucherData;
    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActive()
    {
        voucherData.Activate();
    }
}
