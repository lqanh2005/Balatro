using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCtrl : MonoBehaviour
{
    public Button nextBtn;
    public Button rerollBtn;
    public void Show()
    {
        this.gameObject.SetActive(true);
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(371, 0.5f).SetEase(Ease.OutCubic);
        gameObject.SetActive(true);
        for(int i = 0; i < 2; i++)
        {
            playingCardSlot[i].ResetSlot();
            boosterSlots[i].ResetSlot();
        }
        GenerateShop();
    }

    [Header("Slot Config")]
    public List<ShopSlot> playingCardSlot;
    public List<ShopSlot> boosterSlots;
    public ShopSlot voucherSlot;

    [Header("CardType Probability")]
    public Dictionary<ItemType, float> cardTypeChances = new()
    {
        { ItemType.Ingredient, 1f },
        //{ ItemType.Recipe, 0.3f }
    };

    public void GenerateShop()
    {
        foreach (var slot in playingCardSlot)
        {
            ItemType type = GetRandomCardType();
            List<CardData> pool = CardSO.Instance.GetCardDataByType(type);

            if (pool.Count > 0)
            {
                int rand = Random.Range(0, pool.Count);
                CardData selected = pool[rand];
                slot.SetupCard(selected);
            }
        }
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


    private ItemType GetRandomCardType()
    {
        float rand = Random.value;
        float cumulative = 0f;

        foreach (var pair in cardTypeChances)
        {
            cumulative += pair.Value;
            if (rand <= cumulative)
                return pair.Key;
        }

        return ItemType.Ingredient; // fallback
    }
}

public static class UIHelper
{
    public static Vector3 GetWorldPosition(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2f; // Trung tâm
    }
}