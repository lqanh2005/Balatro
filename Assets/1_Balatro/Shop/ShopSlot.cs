
using BestHTTP.Extensions;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public int id;
    public Transform target;
    public CardBase currentCard;
    public Image image;
    public TMP_Text coin;
    public Button buyBtn;
    public RectTransform cardContainerCanvas;
    public bool isBooster;
    public int gold;

    private Vector2 originalPos;
    private RectTransform rectTransform;
    private int voucherID;
    private int boosterID;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }
    public void SetupCard(CardDataSO data, bool isBought = false)
    {
        this.gameObject.SetActive(!isBought);
        this.gold = (int)(data.cost * GamePlayController.Instance.uICtrl.discount);
        this.coin.text = "$"+ gold.ToString();
        this.image.sprite = data.faceImage;
        buyBtn.onClick.AddListener(() =>
        {
            BuyCard(currentCard, cardContainerCanvas);
            
        });
    }
    public void SetupCard(int id, bool isBought = false)
    {
        this.gameObject.SetActive(!isBought);
        voucherID = id;
        VoucherData voucherData = VoucherDataSO.Instance.GetVoucher(id);
        this.gold = (int)(voucherData.cost * GamePlayController.Instance.uICtrl.discount);
        this.coin.text = "$" + gold.ToString();
        this.image.sprite = voucherData.artwork;
        buyBtn.onClick.AddListener(() =>
        {
            BuyCard(currentCard, cardContainerCanvas);
            
        });
    }
    public void SetupBooster(int id, bool isBought = false)
    {
        this.gameObject.SetActive(!isBought);
        boosterID = id;
        BoosterData boosterData = BoosterDataSo.Instance.GetBooster(id);
        this.gold = (int)(boosterData.cost * GamePlayController.Instance.uICtrl.discount);
        this.coin.text = "$" + gold.ToString();
        this.image.sprite = boosterData.artwork;
        buyBtn.onClick.AddListener(() =>
        {
            BuyCard(currentCard, cardContainerCanvas);
        });
    }

    public void ResetSlot()
    {
        this.gameObject.SetActive(true);
        this.rectTransform.anchoredPosition = originalPos;
    }
    public CardBase GetCard()
    {
        return currentCard;
    }

    public void BuyCard(CardBase data, RectTransform shopCardRect)
    {
        GameController.Instance.musicManager.PlayClickSound();
        GameObject newCard = null;
        Vector3 spawnPos = Vector3.zero;
        if (UseProfile.CurrentGold < this.gold)
        {
            return;
        }
        UseProfile.CurrentGold -= this.gold;
        GameController.Instance.musicManager.PlayReduceGoldSound();
        switch (data)
        {
            case PlayingCard playingCard:
                GamePlayController.Instance.playerContain.deckController.AddCardToDeck(playingCard, playingCard.id, GamePlayController.Instance.playerContain.deckController.deckDict[playingCard.id].level);

                UseProfile.CurrentCard += 1;
                GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.playingCardSlot);
                spawnPos = shopCardRect.GetWorldPosition();
                newCard = SimplePool2.Spawn(data.gameObject);
                newCard.transform.position = spawnPos;
                MoveTo(newCard, target.position, 0.5f, () =>
                {
                    SimplePool2.Despawn(newCard);
                });
                break;
            case VoucherBase voucher:
                GamePlayController.Instance.uICtrl.shopCtrl.Close();
                voucher.voucherData = VoucherDataSO.Instance.GetVoucher(voucherID);
                newCard = SimplePool2.Spawn(data.gameObject);
                VoucherBase vb = newCard.GetComponent<VoucherBase>();
                vb.avt.sprite = voucher.voucherData.artwork;
                RectTransform avtRect = vb.avt.GetComponent<RectTransform>();
                avtRect.position = shopCardRect.position;
                avtRect.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    newCard.GetComponent<VoucherBase>().Init();
                    GamePlayController.Instance.uICtrl.shopCtrl.Show();
                    SimplePool2.Despawn(newCard);
                });
                
                break;
            case BoosterBase booster:
                GameController.Instance.musicManager.PlayOpenPackSound();
                GamePlayController.Instance.uICtrl.shopCtrl.Close();
                booster.boosterData = BoosterDataSo.Instance.GetBooster(boosterID);
                GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.boosterSlots);
                newCard = SimplePool2.Spawn(data.gameObject);
                BoosterBase vb1 = newCard.GetComponent<BoosterBase>();
                vb1.avt.sprite = booster.boosterData.artwork;
                RectTransform avtRect1 = vb1.avt.GetComponent<RectTransform>();
                avtRect1.position = shopCardRect.position;
                avtRect1.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    newCard.GetComponent<BoosterBase>().Init();
                    SimplePool2.Despawn(newCard);
                });
                break;
        }
        this.gameObject.SetActive(false);
        GamePlayController.Instance.uICtrl.shopCtrl.purchaseList[id].isPurchased = true;
        GamePlayController.Instance.uICtrl.shopCtrl.SavePurchaseList(GamePlayController.Instance.uICtrl.shopCtrl.purchaseList);
        
        

    }
    public void MoveTo(GameObject obj, Vector3 targetPos, float duration, System.Action onComplete = null)
    {
        obj.transform.DOMove(targetPos, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }
}
