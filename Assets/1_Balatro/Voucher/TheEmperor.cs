using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/TheEmperor")]
public class TheEmperor : VoucherEffectBase
{
    private int voucherId1, voucherId2;
    public override void ApplyEffect()
    {
        voucherId1 = Random.Range(0, VoucherDataSO.Instance.vouchers.Count);
        do
        {
            voucherId2 = Random.Range(0, VoucherDataSO.Instance.vouchers.Count);
        }while(voucherId1 == voucherId2);
        VoucherData voucher = VoucherDataSO.Instance.vouchers[voucherId1];
        VoucherData voucher2 = VoucherDataSO.Instance.vouchers[voucherId2];
        Debug.LogError(Equals(voucher.voucherName, voucher2.voucherName));
    }

}
