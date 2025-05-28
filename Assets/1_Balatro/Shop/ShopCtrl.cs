using DG.Tweening;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ShopCtrl : MonoBehaviour
{
    public Button nextBtn;
    public Button rerollBtn;
    public RectTransform shopPanel;
    public List<PurchaseEntry> purchaseList;

    public void Init()
    {
        this.RegisterListener(EventID.ON_SHOPPING, delegate { Show(); });
        nextBtn.onClick.AddListener(delegate { HandleNext(); });
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        shopPanel.DOAnchorPosY(371, 0.5f).SetEase(Ease.OutCubic);
        if (!UseProfile.NeedCheckShop)
        {
            GenerateShop();
            UseProfile.NeedCheckShop = true;
        }
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        shopPanel.DOAnchorPosY(-400, 0.5f).SetEase(Ease.OutCubic);
    }

    public void HandleNext()
    {
        this.Close();
        GameController.Instance.musicManager.PlayClickSound();
        UseProfile.NeedCheckShop = false;
        UseProfile.SavedState = StateGame.SelectRound;
        this.PostEvent(EventID.ON_SELECT_ROUND);
    }
    [Header("Slot Config")]
    public List<ShopSlot> playingCardSlot;
    public List<ShopSlot> boosterSlots;
    public ShopSlot voucherSlot;


    public void GenerateShop()
    {
        int idx;
        for (int i = 0; i < 2; i++)
        {
            playingCardSlot[i].ResetSlot();
            boosterSlots[i].ResetSlot();
        }
        voucherSlot.ResetSlot();
        purchaseList.Clear();
        foreach (var slot in playingCardSlot)
        {
            idx = Random.Range(0, 6);
            int level = Random.Range(0, 2);
            slot.SetupCard(ConfigData.Instance.cardLists[idx].cardPerLevels[level].cardDatas);
            purchaseList.Add(new PurchaseEntry
            {
                idItem = idx,
                level = level,
                isPurchased = false
            });
        }
        idx = Random.Range(0, 7);
        voucherSlot.SetupCard(idx);
        purchaseList.Add(new PurchaseEntry
        {
            idItem = idx,
            isPurchased = false
        });
        foreach (var slot in boosterSlots)
        {
            int i = Random.Range(0, 2);
            slot.SetupBooster(i);
            purchaseList.Add(new PurchaseEntry
            {
                idItem = i,
                isPurchased = false
            });
        }
        SavePurchaseList(purchaseList);
    }
    public void UpdateUI(List<ShopSlot> param)
    {
        foreach (var slot in param)
        {
            if (slot.gameObject.activeSelf)
            {
                RectTransform rt = slot.GetComponent<RectTransform>();
                rt.DOAnchorPosX(0f, 0.3f).SetEase(Ease.OutCubic);
            }
        }
    }
    public void ContinueShopping()
    {
        purchaseList = LoadPurchaseList();
        for (int i = 0; i < 2; i++)
        {
            playingCardSlot[i].SetupCard(ConfigData.Instance.cardLists[purchaseList[i].idItem].cardPerLevels[purchaseList[i].level].cardDatas, purchaseList[playingCardSlot[i].id].isPurchased);
            boosterSlots[i].SetupBooster(purchaseList[boosterSlots[i].id].idItem, purchaseList[boosterSlots[i].id].isPurchased);
        }
        UpdateUI(playingCardSlot);
        UpdateUI(boosterSlots);
        voucherSlot.SetupCard(purchaseList[voucherSlot.id].idItem, purchaseList[voucherSlot.id].isPurchased);
    }
    public void SavePurchaseList(List<PurchaseEntry> list)
    {
        PurchaseListWrapper wrapper = new PurchaseListWrapper { entries = list };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("PurchasedList", json);
        PlayerPrefs.Save();
    }
    public List<PurchaseEntry> LoadPurchaseList()
    {
        string json = PlayerPrefs.GetString("PurchasedList", "");
        if (string.IsNullOrEmpty(json)) return new List<PurchaseEntry>();

        PurchaseListWrapper wrapper = JsonUtility.FromJson<PurchaseListWrapper>(json);
        return wrapper.entries ?? new List<PurchaseEntry>();
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.ON_SHOPPING, delegate { Show(); });
    }
}

public static class UIHelper
{
    public static Vector3 GetWorldPosition(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2f;
    }
}
[System.Serializable]
public class PurchaseListWrapper
{
    public List<PurchaseEntry> entries;
}
[System.Serializable]
public class PurchaseEntry
{
    public int idItem;
    public int level;
    public bool isPurchased;
}