using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoucherDataSO", menuName = "Balatro/VoucherDataSO")]
public class VoucherDataSO : ScriptableObject
{
    /// This is a ScriptableObject that holds a list of VoucherSO objects
    public List<VoucherSO> vouchers = new List<VoucherSO>();
}

[System.Serializable]
public class VoucherSO
{
    public string voucherName;
    public string description;
    public Sprite artwork;
    public int cost = 10;
    public bool isDependant;
    public VoucherBase requiredVoucher;
}
