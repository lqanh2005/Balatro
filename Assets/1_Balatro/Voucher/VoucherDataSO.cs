using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoucherDataSO", menuName = "Balatro/VoucherDataSO")]
public class VoucherDataSO : SingletonScriptableObject<VoucherDataSO>
{
    public List<VoucherData> vouchers = new List<VoucherData>();
    public VoucherData GetVoucher(int id)
    {
        return vouchers[id];
    }
}

[System.Serializable]
public class VoucherData
{
    public string voucherName;
    public string description;
    public Sprite artwork;
    public int cost = 10;
    public VoucherEffectBase effect;

    public void Activate()
    {
        effect?.ApplyEffect();
    }
}
