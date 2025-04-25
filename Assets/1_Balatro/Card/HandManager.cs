
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public DeckController deckController;
    public Transform handPos;
    public Transform deckPos;
    public Transform discardPos;
    public Transform playPos;

    public List<Transform> handPosList = new List<Transform>();

    public List<CardBase> cardViews =  new List<CardBase>();
    public List<CardBase> seletedCards = new List<CardBase>();

    public int maxSelectedCard = 5;
    private int selectedCardCount = 0;
    [SerializeField] private float cardSpacing = 2f;
    [SerializeField] private float drawCardDelay = 0.15f;

    //public delegate void OnCardsPlayedEvent(List<CardBase> playedCards);
    //public event OnCardsPlayedEvent CardsPlayed;

    //private void OnCardsPlayed(List<CardBase> playedCards)
    //{
    //    CardsPlayed?.Invoke(playedCards);
    //}

    public void Init()
    {
        

    }
    public void OnCardDrawn(CardBase card, int handPos)
    {
        StartCoroutine(CreateCard(card, handPos, drawCardDelay * handPos));
    }
    private IEnumerator CreateCard(CardBase card, int pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateCardView(card, pos);
    }
    private void CreateCardView(CardBase card, int pos)
    {
        if(handPos != null)
        {
            card.Init(pos);
            //cardViews.Add(card);
            //UpdateSortPos(cardViews);
            card.PlayDrawAnimation(deckPos.position, handPosList[pos].position);
            
        }
    }

    //public void DrawToFillHand(int targetCount)
    //{
    //    int currentCount = cardViews.Count;
    //    int drawCount = targetCount - currentCount;
    //    if(drawCount > 0)
    //    {
    //        deckController.DrawCards(drawCount);
    //    }
    //}
    public List<CardBase> GetSelectedCards()
    {
        return seletedCards;
    }

    public void PlaySelectedCards()
    {
        if(seletedCards.Count > 0)
        {
            selectedCardCount = seletedCards.Count;
            List<CardBase> cardsToPlay = GetSelectedCards();
            float delay = 0f;
            for(int i = 0; i < seletedCards.Count; i++)
            {
                CardBase cardView = seletedCards[i];

                float spread = 1.0f;
                float offsetX = (i - (seletedCards.Count - 1) / 2f) * spread;
                Vector3 targetPos = new Vector3(playPos.position.x + offsetX, playPos.position.y, 0);

                float currentDelay = delay;
                    cardViews.Remove(cardView);
                DOVirtual.DelayedCall(currentDelay, () =>
                {
                    Sequence playSequence = cardView.PlayHandAnimation(targetPos);
                    playSequence.OnComplete(() =>
                    {
                        cardView.PlayDiscardAnimation(discardPos.position).OnComplete(() =>
                        {
                            deckController.DiscardCard(cardView.GetCardBase());
                            SimplePool2.Despawn(cardView.gameObject);
                        });
                        
                    });
                });
                delay += 0.15f;
            }
            seletedCards.Clear();
            //DOVirtual.DelayedCall(delay +0.5f, () =>
            //{
            //    RearrangeCards();
            //});

            UpdateSortPos(cardViews);
            deckController.DrawCards(selectedCardCount);
            //OnCardsPlayed(cardsToPlay);
        }
    }

    private void RearrangeCards()
    {
        for (int i = 0; i < cardViews.Count; i++)
        {
            if (i >= handPosList.Count) break;

            CardBase cardView = cardViews[i];
            Transform target = handPosList[i].transform;
            cardView.originalPosition = target.localPosition;
            cardView.transform.DOLocalMove(target.localPosition, 0.5f).SetEase(Ease.OutQuad);
            cardView.transform.DOLocalRotateQuaternion(target.localRotation, 0.5f).SetEase(Ease.OutQuad);
        }
    }
    public void UpdateSortPos(List<CardBase> sortedObjects)
    {
        if (sortedObjects.Count == 0) return;

        Sequence moveSequence = DOTween.Sequence();

        for (int i = 0; i < sortedObjects.Count; i++)
        {
            moveSequence.Join(sortedObjects[i].transform.DOMove(handPosList[i].position, 0.5f));
            sortedObjects[i].originalPosition = handPosList[i].localPosition;
        }
    }
    public void DiscardSelectedCards()
    {
        if(seletedCards.Count > 0)
        {
            float delay = 0f;
            foreach(CardBase cardView in seletedCards)
            {
                Vector3 targetPos = Camera.main.WorldToScreenPoint(discardPos.position);
                DOVirtual.DelayedCall(delay, () =>
                {
                    Sequence discardSequence = cardView.PlayDiscardAnimation(targetPos);
                    discardSequence.OnComplete(() =>
                    {
                        cardViews.Remove(cardView);
                        deckController.DiscardCard(cardView.GetCardBase());
                        SimplePool2.Despawn(cardView.gameObject);
                    });
                });
            }
            delay += 0.15f;
            seletedCards.Clear();
            DOVirtual.DelayedCall(delay + 0.5f, () =>
            {
                RearrangeCards();
            });
        }
    }
    public void AnimateFanCards()
    {
        float fanAngle = 0.5f;
        float fanRadius = 50;
        float centerIdx=(cardViews.Count - 1) / 2f;
        Vector3 centerPos = new Vector3(centerIdx * cardSpacing, 0, 0);
        for(int i = 0; i < cardViews.Count; i++)
        {
            CardBase cardView = cardViews[i];
            float offsetFromCenter = i - centerIdx;
            float angle = offsetFromCenter * fanAngle;
            float xPos = offsetFromCenter * cardSpacing;
            float yPos = -Mathf.Abs(offsetFromCenter) * 10f; // tạo độ cong

            Vector3 targetPos = new Vector3(xPos, yPos, 0);

            cardView.transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutQuad);
            cardView.transform.DOLocalRotate(new Vector3(0, 0, angle), 0.5f).SetEase(Ease.OutQuad);
        }
    }
    public void AnimateCardUpgrade(CardBase cardView)
    {
        Vector3 originalPos = cardView.transform.localPosition;
        Sequence upgradeSequence = DOTween.Sequence();
        upgradeSequence.Append(cardView.transform.DOLocalMoveY(originalPos.y+100f,0.5f).SetEase(Ease.OutQuad)).
            Join(cardView.transform.DOScale(Vector3.one *1.5f, 0.5f).SetEase(Ease.OutQuad)).
            Join(cardView.transform.DOLocalRotate(new Vector3(0, 0, 5f), 0.5f).SetEase(Ease.OutQuad)).
            AppendInterval(0.5f).
            Append(cardView.transform.DOLocalMove(originalPos, 0.5f).SetEase(Ease.OutQuad)).
            Join(cardView.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad)).
            Join(cardView.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad));
    }
}
