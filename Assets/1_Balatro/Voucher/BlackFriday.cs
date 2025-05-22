using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/BlackFriday")]
public class BlackFriday : VoucherEffectBase
{
    public override void ApplyEffect()
    {
        GamePlayController.Instance.uICtrl.discount = 0.25f;
    }
}
