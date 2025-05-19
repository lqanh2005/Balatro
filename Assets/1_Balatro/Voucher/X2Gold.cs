using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/X2Gold")]
public class X2Gold : VoucherEffectBase
{
    private int bonus;
    public override void ApplyEffect()
    {
        bonus = UseProfile.CurrentGold;
        if (bonus * 2 >= 40)
        {
            UseProfile.CurrentGold += 20;
        }
        else
        {
            UseProfile.CurrentGold += bonus * 2;
        }
    }

}
