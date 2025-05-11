
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Transform target;
    public CardBase currentCard;
    public Image image;
    public Button buyBtn;
    public RectTransform cardContainerCanvas;
    public bool isBooster;

    private Vector2 originalPos;
    private RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }
    public void SetupCard(CardData data)
    {
        currentCard = data.cardBase.GetComponent<CardBase>();
        //currentCard.Init();
        this.image.sprite = data.image;
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
        if (currentCard != null)
        {
            this.gameObject.SetActive(false);
            if(isBooster) GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.boosterSlots);
            else GamePlayController.Instance.uICtrl.shopCtrl.UpdateUI(GamePlayController.Instance.uICtrl.shopCtrl.playingCardSlot);
            Vector3 spawnPos = shopCardRect.GetWorldPosition();
            GameObject newCard = Instantiate(data.gameObject);
            newCard.transform.position = spawnPos;
            MoveTo(newCard, target.position, 0.5f, () =>
            {
                SimplePool2.Despawn(newCard);
            });
        }
    }
    public void MoveTo(GameObject obj, Vector3 targetPos, float duration, System.Action onComplete = null)
    {
        obj.transform.DOMove(targetPos, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => onComplete?.Invoke());
    }
}
