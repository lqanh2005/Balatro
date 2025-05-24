
using BestHTTP.Extensions;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Transform target;
    public CardBase currentCard;
    public Image image;
    public TMP_Text coin;
    public Button buyBtn;
    public RectTransform cardContainerCanvas;
    public bool isBooster;

    private Vector2 originalPos;
    private RectTransform rectTransform;
    private int voucherID;
    private int boosterID;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }
    public void SetupCard(CardData data)
    {
        currentCard = data.cardBase.GetComponent<CardBase>();
        
        this.coin.text = "$"+ ((int)(data.baseCost * GamePlayController.Instance.uICtrl.discount)).ToString();
        this.image.sprite = data.image;
        buyBtn.onClick.AddListener(() =>
        {
            BuyCard(currentCard, cardContainerCanvas);
        });
    }
    public void SetupCard(int id)
    {
        voucherID = id;
        VoucherData voucherData = VoucherDataSO.Instance.GetVoucher(id);
        this.coin.text = "$" + ((int)(voucherData.cost * GamePlayController.Instance.uICtrl.discount)).ToString();
        this.image.sprite = voucherData.artwork;
        buyBtn.onClick.AddListener(() =>
        {
            BuyCard(currentCard, cardContainerCanvas);
        });
    }
    public void SetupBooster(int id)
    {
        boosterID = id;
        BoosterData boosterData = BoosterDataSo.Instance.GetBooster(id);
        this.coin.text = "$" + ((int)(boosterData.cost * GamePlayController.Instance.uICtrl.discount)).ToString();
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
        GameObject newCard = null;
        Vector3 spawnPos = Vector3.zero;
        if (UseProfile.CurrentGold < this.coin.text.ToInt32())
        {
            return;
        }
            UseProfile.CurrentGold -= this.coin.text.ToInt32();
        switch (data)
        {
            case PlayingCard playingCard:
                GamePlayController.Instance.playerContain.deckController.AddCardToDeck(playingCard, playingCard.id);
                UseProfile.CurrentCard += 1;
                GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.playingCardSlot);
                spawnPos = shopCardRect.GetWorldPosition();
                newCard = Instantiate(data.gameObject);
                newCard.transform.position = spawnPos;
                MoveTo(newCard, target.position, 0.5f, () =>
                {
                    SimplePool2.Despawn(newCard);
                });
                break;
            case VoucherBase voucher:
                GamePlayController.Instance.uICtrl.shopCtrl.Close();
                voucher.voucherData = VoucherDataSO.Instance.GetVoucher(voucherID);
                newCard = Instantiate(data.gameObject);
                VoucherBase vb = newCard.GetComponent<VoucherBase>();
                vb.avt.sprite = voucher.voucherData.artwork;
                RectTransform avtRect = vb.avt.GetComponent<RectTransform>();
                avtRect.position = shopCardRect.position;
                avtRect.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    SimplePool2.Despawn(newCard);
                    newCard.GetComponent<VoucherBase>().Init();
                    GamePlayController.Instance.uICtrl.shopCtrl.Show();
                });
                
                break;
            case BoosterBase booster:
                GamePlayController.Instance.uICtrl.shopCtrl.Close();
                booster.boosterData = BoosterDataSo.Instance.GetBooster(boosterID);
                newCard = Instantiate(data.gameObject);
                BoosterBase vb1 = newCard.GetComponent<BoosterBase>();
                vb1.avt.sprite = booster.boosterData.artwork;
                RectTransform avtRect1 = vb1.avt.GetComponent<RectTransform>();
                avtRect1.position = shopCardRect.position;
                avtRect1.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    SimplePool2.Despawn(newCard);
                    newCard.GetComponent<BoosterBase>().Init();
                });
                break;
        }
        if(data as PlayingCard)
        {
            PlayingCard _card = data.GetComponent<PlayingCard>();
            GamePlayController.Instance.playerContain.deckController.AddCardToDeck(_card, _card.id);
            UseProfile.CurrentCard += 1;
        }
        if(data is VoucherBase)
        {

        }
        this.gameObject.SetActive(false);
            if(isBooster) GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.boosterSlots);
           
        
    }
    public void MoveTo(GameObject obj, Vector3 targetPos, float duration, System.Action onComplete = null)
    {
        obj.transform.DOMove(targetPos, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }
}
