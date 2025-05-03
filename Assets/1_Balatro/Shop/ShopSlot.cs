using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI nameText;
    public Button buyButton;
    public GameObject emptyIndicator;

    [HideInInspector] public object currentItem;
    [HideInInspector] public int currentPrice;
    private ShopManager shopManager;

    public void Init()
    {
        shopManager = FindObjectOfType<ShopManager>();
        buyButton.onClick.AddListener(OnBuyClicked);
        ClearSlot();
    }
    public void SetItem(object item, int price)
    {
        currentItem = item;
        currentPrice = price;

        if (item is Card)
        {
            Card card = item as Card;
            itemImage.sprite = card.artwork;
            nameText.text = card.cardName;
        }
        else if (item is BoosterBase)
        {
            BoosterBase booster = item as BoosterBase;
            itemImage.sprite = booster.artwork;
            nameText.text = booster.packName;
        }
        else if (item is Voucher)
        {
            VoucherBase voucher = item as VoucherBase;
            itemImage.sprite = voucher.artwork;
            nameText.text = voucher.voucherName;
        }

        priceText.text = "$" + price.ToString();
        emptyIndicator.SetActive(false);
        itemImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);
    }
    public bool IsEmpty()
    {
        return currentItem == null;
    }
    public void ClearSlot()
    {
        currentItem = null;
        currentPrice = 0;
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        nameText.text = "";
        priceText.text = "";
        emptyIndicator.SetActive(true);
    }

    private void OnBuyClicked()
    {
        throw new NotImplementedException();
    }
}
