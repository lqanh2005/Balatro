using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoucherBase", menuName = "Balatro/VoucherBase")]
public class VoucherBase : ScriptableObject
{
    public string voucherName;
    public string description;
    public Sprite artwork;
    public int cost = 10;
    public bool isDependant;
    public VoucherBase requiredVoucher; // Voucher cần phải có trước

    // Implement voucher effect
    public virtual void ApplyEffect(ShopManager shopManager, GamePlayController gameManager) { }
}
