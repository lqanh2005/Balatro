
using BestHTTP.Extensions;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public DeckController deckController;
    public bool isFirstDraw = true;
    public Transform handPos;
    public Transform deckPos;
    public Transform discardPos;
    public Transform playPos;


    public List<CardBase> cardViews =  new List<CardBase>();
    public List<CardBase> seletedCards = new List<CardBase>();

    public int maxSelectedCard = 5;
    private int selectedCardCount = 0;
    [SerializeField] private float cardSpacing = 2f;
    [SerializeField] private float drawCardDelay = 0.15f;
    public RecipeDatabaseSO recipeDatabaseSO;

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
        card.Init(pos);

        Vector3 targetPos = CalculateCardPosition(pos, cardViews.Count) + handPos.position;
        card.PlayDrawAnimation(deckPos.position, targetPos, isFirstDraw, () =>
        {
            UpdateSortPos(cardViews);
        });
    }
    public Vector3 CalculateCardPosition(int index, int totalCards)
    {
        float spacing = 1.2f;
        float startX = -(spacing * (totalCards - 1)) / 2f;
        float x = startX + index * spacing;

        float y = -Mathf.Abs(index - (totalCards - 1) / 2f) * 0.1f;

        return new Vector3(x, y, 0);
    }
    public void UpdateSortPos(List<CardBase> sortedObjects)
    {
        if (sortedObjects.Count == 0) return;

        Sequence moveSequence = DOTween.Sequence();

        for (int i = 0; i < sortedObjects.Count; i++)
            {
            Vector3 targetPos = CalculateCardPosition(i, sortedObjects.Count) + handPos.position;
            sortedObjects[i].originalPosition = targetPos;
            moveSequence.Join(
            sortedObjects[i].transform
                .DOLocalMove(targetPos, 0.5f)
                .SetEase(Ease.OutQuad)
        );
        }
    }
    public void PlaySelectedCards()
    {
        if (seletedCards.Count > 0)
        {
            isFirstDraw = false;
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
                UpdateSortPos(cardViews);
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

        scoreSequence.AppendInterval(0.3f);

        scoreSequence.AppendCallback(() =>
        {
            int finalScore = totalCoin * multi;
            DOTween.To(() => currentCoin, x =>
            {
                currentCoin = x;
                TextEffectHelper.PlayScoreBounce(coinText, currentCoin);
            }, 0, 0.5f).SetEase(Ease.OutCubic);
            DOTween.To(() => currentScore, x =>
            {
                currentScore = x;
                TextEffectHelper.PlayScoreBounce(scoreText, currentScore);
            }, currentScore + finalScore, 0.5f).SetEase(Ease.OutCubic);
        });
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
                UpdateSortPos(cardViews);
            });
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
