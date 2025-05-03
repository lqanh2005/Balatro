using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCtrl : BaseBox
{
    public static ShopCtrl instance;
    public List<HandleLayout> handleLayouts;
    public CardUI voucherUI;
    public Canvas canvas;
    public static ShopCtrl Setup(ButtonShopType param = ButtonShopType.Gift, bool isSaveBox = false, Action actionOpenBoxSave = null)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<ShopCtrl>(PathPrefabs.SHOP_CTRL));
            instance.Init(param);
        }

        instance.InitState();
        return instance;
    }

    public Button nextBtn;
    public Button rerollBtn;
    private void Init(ButtonShopType param)
    {
        nextBtn.onClick.AddListener( delegate { this.Close(); });
        
    }
    private void InitState()
    {
        foreach(HandleLayout _item in handleLayouts)
        {
            _item.Init();
        }
    }
}
