using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    public int cardSlots = 2;
    public int boosterSlots = 2;
    public int voucherSlots = 1;

    [Header("References")]
    public Transform cardSlotsParent;
    public Transform boosterSlotsParent;
    public Transform voucherSlotsParent;
    public GameObject cardSlotPrefab;
    public GameObject boosterSlotPrefab;
    public GameObject voucherSlotPrefab;

    [Header("Shop Data")]
    public List<Card> availableCards = new List<Card>();
    public List<BoosterBase> availableBoosters = new List<BoosterBase>();
    public List<VoucherBase> availableVouchers = new List<VoucherBase>();
    public List<VoucherBase> purchasedVouchers = new List<VoucherBase>();

    [Header("Shop Economy")]
    public int rerollCost = 5;
    public float discountMultiplier = 1.0f;

    [Header("Card Weights")]
    public float ingredientWeight = 20f;
    public float recipetWeight = 4f;

    private List<ShopSlot> cardSlotsList = new List<ShopSlot>();
    private List<ShopSlot> boosterSlotsList = new List<ShopSlot>();
    private ShopSlot voucherSlot;

    public void Init()
    {
        CreateShopSlots();
        RestockCards();
        RestockBoosters();
        RestockVoucher();
    }
    void CreateShopSlots()
    {
        foreach (Transform child in cardSlotsParent) SimplePool2.Despawn(child.gameObject);
        foreach (Transform child in boosterSlotsParent) SimplePool2.Despawn(child.gameObject);
        foreach (Transform child in voucherSlotsParent) SimplePool2.Despawn(child.gameObject);

        cardSlotsList.Clear();
        boosterSlotsList.Clear();

        for (int i = 0; i < cardSlots; i++)
        {
            GameObject slotObj = SimplePool2.Spawn(cardSlotPrefab, cardSlotsParent.position, Quaternion.identity);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            cardSlotsList.Add(slot);
        }
        for (int i = 0; i < boosterSlots; i++)
        {
            GameObject slotObj = SimplePool2.Spawn(boosterSlotPrefab, boosterSlotsParent.position, Quaternion.identity);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            boosterSlotsList.Add(slot);
        }
        GameObject voucherObj = SimplePool2.Spawn(voucherSlotPrefab, voucherSlotsParent.position, Quaternion.identity);
        voucherSlot = voucherObj.GetComponent<ShopSlot>();
    }
    public void RestockCards()
    {
        foreach (ShopSlot slot in cardSlotsList)
        {
            if (slot.IsEmpty())
            {
                Card card = GenerateRandomCard();
                if (card != null)
                {
                    int price = CalculateCardPrice(card);
                    slot.SetItem(card, price);
                }
            }
        }
    }
    public void RestockBoosters()
    {
        //foreach (var slot in boosterSlotsList)
        //{
        //    slot.RestockBooster(availableBoosters);
        //}
    }
    public void RestockVoucher()
    {
        //if (voucherSlot != null)
        //{
        //    voucherSlot.RestockVoucher(availableVouchers);
        //}
    }
    private Card GenerateRandomCard()
    {
        float totalWeight = ingredientWeight + recipetWeight;
        float randomValue = Random.Range(0f, totalWeight);
        ItemType selectedType;

        if (randomValue < ingredientWeight)
            selectedType = ItemType.Ingredient;
        else
            selectedType = ItemType.Recipe;
        List<Card> cardsOfType = availableCards.FindAll(card => card.itemType == selectedType);

        if (cardsOfType.Count > 0)
        {
            return cardsOfType[Random.Range(0, cardsOfType.Count)];
        }

        return null;
    }
    public int CalculateCardPrice(Card card)
    {
        int price = card.baseCost;
        price = Mathf.RoundToInt(price * discountMultiplier);
        return Mathf.Max(1, price);
    }
    public void PurchaseItem(ShopSlot slot)
    {
        if (slot.currentItem == null) return;

        int price = slot.currentPrice;

        if (UseProfile.Coin >= price)
        {
            UseProfile.Coin -= price;

            // Handle item based on type
            if (slot.currentItem is Card)
            {
                Card card = slot.currentItem as Card;
                //AddCardToInventory(card);
            }
            //else if (slot.currentItem is BoosterPack)
            //{
            //    BoosterPack booster = slot.currentItem as BoosterPack;
            //    OpenBoosterPack(booster);
            //}
            //else if (slot.currentItem is Voucher)
            //{
            //    Voucher voucher = slot.currentItem as Voucher;
            //    ApplyVoucher(voucher);
            //}

            // Clear the slot
            slot.ClearSlot();
        }
    }
    void AddCardToInventory(Card card)
    {
        // Add card to player's inventory/deck
        //GamePlayController.Instance.playerContain.deckController.deck.Add(card);
    }
}
