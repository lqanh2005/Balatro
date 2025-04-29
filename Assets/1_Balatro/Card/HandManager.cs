
using BestHTTP.Extensions;
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
    public RecipeDatabaseSO recipeDatabaseSO;

    //public delegate void OnCardsPlayedEvent(List<CardBase> playedCards);
    //public event OnCardsPlayedEvent CardsPlayed;

    //private void OnCardsPlayed(List<CardBase> playedCards)
    //{
    //    CardsPlayed?.Invoke(playedCards);
    //}

    public void Init()
    {
        InitializeRecipeSystem();
        deckController.Init();
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
            card.PlayDrawAnimation(deckPos.position, handPosList[pos].position, () =>
            {
                UpdateSortPos(cardViews);
            });
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
    public void CheckRecipe()
    {

    }
    public void PlaySelectedCards()
    {
        if (seletedCards.Count > 0)
        {
            selectedCardCount = seletedCards.Count;
            List<CardBase> cardsToPlay = new List<CardBase>(seletedCards);
            float delay = 0f;
            List<Sequence> playSequences = new List<Sequence>();

            for (int i = 0; i < seletedCards.Count; i++)
            {
                CardBase cardView = seletedCards[i];

                float spread = 1.0f;
                float offsetX = (i - (seletedCards.Count - 1) / 2f) * spread;
                Vector3 targetPos = new Vector3(playPos.position.x + offsetX, playPos.position.y, 0);

                float currentDelay = delay;
                cardViews.Remove(cardView);

                var playSequence = DOTween.Sequence();
                playSequence.AppendInterval(currentDelay);
                playSequence.Append(cardView.PlayHandAnimation(targetPos));
                playSequences.Add(playSequence);

                delay += 0.15f;
            }

            DOTween.Sequence()
                .Append(playSequences.Count > 0 ? playSequences[playSequences.Count - 1] : null)
                .AppendCallback(() =>
                {
                    Recipe matchResult = RecipeChecker.GetMatchedRecipe(cardsToPlay);
                    if (matchResult != Recipe.None)
                    {
                        List<CardBase> validCards = RecipeChecker.GetCardsToScore(cardsToPlay, matchResult);
                        int currentTotal = GamePlayController.Instance.uICtrl.coin.text.ToInt32();

                        Sequence scoreSequence = DOTween.Sequence();
                        PlayScoreAnim(validCards, scoreSequence);
                        scoreSequence.OnComplete(() => {
                            foreach (var card in cardsToPlay)
                            {
                                card.PlayDiscardAnimation(discardPos.position).OnComplete(() =>
                                {
                                    deckController.DiscardCard(card.GetCardBase());
                                    SimplePool2.Despawn(card.gameObject);
                                });
                            }

                            UpdateSortPos(cardViews);
                            deckController.DrawCards(selectedCardCount);
                        });
                    }
                    else
                    {
                        foreach (var card in cardsToPlay)
                        {
                            card.PlayDiscardAnimation(discardPos.position).OnComplete(() =>
                            {
                                deckController.DiscardCard(card.GetCardBase());
                                SimplePool2.Despawn(card.gameObject);
                            });
                        }

                        UpdateSortPos(cardViews);
                        deckController.DrawCards(selectedCardCount);
                    }
                });
            seletedCards.Clear();
        }
    }

    //public void PlaySelectedCards()
    //{
    //    if (seletedCards.Count > 0)
    //    {
    //        selectedCardCount = seletedCards.Count;
    //        List<CardBase> cardsToPlay = GetSelectedCards();
    //        float delay = 0f;
    //        for (int i = 0; i < seletedCards.Count; i++)
    //        {
    //            CardBase cardView = seletedCards[i];

    //            float spread = 1.0f;
    //            float offsetX = (i - (seletedCards.Count - 1) / 2f) * spread;
    //            Vector3 targetPos = new Vector3(playPos.position.x + offsetX, playPos.position.y, 0);

    //            float currentDelay = delay;
    //            cardViews.Remove(cardView);
    //            DOVirtual.DelayedCall(currentDelay, () =>
    //            {
    //                Sequence playSequence = cardView.PlayHandAnimation(targetPos);
    //                playSequence.OnComplete(() =>
    //                {
    //                    cardView.PlayDiscardAnimation(discardPos.position).OnComplete(() =>
    //                    {
    //                        deckController.DiscardCard(cardView.GetCardBase());
    //                        SimplePool2.Despawn(cardView.gameObject);
    //                    });

    //                });
    //            });
    //            delay += 0.15f;
    //        }
    //        seletedCards.Clear();
    //        UpdateSortPos(cardViews);
    //        deckController.DrawCards(selectedCardCount);
    //    }
    //}

    public void PlayScoreAnim(List<CardBase> cardsToPlay, Sequence scoreSequence)
    {
        int totalCoin = 0;
        int currentCoin = GamePlayController.Instance.uICtrl.coin.text.ToInt32();
        int currentScore = GamePlayController.Instance.uICtrl.score.text.ToInt32();

        var coinText = GamePlayController.Instance.uICtrl.coin;
        var scoreText = GamePlayController.Instance.uICtrl.score;
        int multi = GamePlayController.Instance.uICtrl.multi.text.ToInt32();

        float scoreDelay = 0f;

        foreach (var card in cardsToPlay)
        {
            scoreSequence.AppendInterval(scoreDelay);
            scoreSequence.AppendCallback(() =>
            {
                card.PlayHoverAniamtion();
                totalCoin += card.chip;
                currentCoin += card.chip;
                TextEffectHelper.PlayScoreBounce(coinText, currentCoin);
            });
            scoreSequence.AppendInterval(0.2f);
            scoreDelay = 0.05f;
        }

        scoreSequence.AppendInterval(0.3f); // chờ hiệu ứng chip xong

        scoreSequence.AppendCallback(() =>
        {
            int finalScore = totalCoin * multi;

            // Coin giảm về 0
            DOTween.To(() => currentCoin, x =>
            {
                currentCoin = x;
                TextEffectHelper.PlayScoreBounce(coinText, currentCoin);
            }, 0, 0.5f).SetEase(Ease.OutCubic);

            // Score tăng lên
            DOTween.To(() => currentScore, x =>
            {
                currentScore = x;
                TextEffectHelper.PlayScoreBounce(scoreText, currentScore);
            }, currentScore + finalScore, 0.5f).SetEase(Ease.OutCubic);
        });
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
    private void InitializeRecipeSystem()
    {
        RecipeChecker.Init(recipeDatabaseSO);
        bool hasExistingData = PlayerPrefs.HasKey(StringHelper.RECIPT_ALL);
        if(!hasExistingData)
        {
            RecipeManager.LoadAll(recipeDatabaseSO);
        }
        else
        {
            RecipeManager.CheckAndUpdateDatabase(recipeDatabaseSO);
        }
    }
    public void Reset()
    {
        RecipeManager.ResetAllRecipe(recipeDatabaseSO);
    }

}
