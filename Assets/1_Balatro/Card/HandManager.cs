
using BestHTTP.Extensions;
using DG.Tweening;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public bool isFirstDraw = true;
    public RectTransform handPos;
    public RectTransform deckPos;
    public RectTransform discardPos;
    public RectTransform playPos;


    public List<PlayingCard> cardViews =  new List<PlayingCard>();
    public List<PlayingCard> seletedCards = new List<PlayingCard>();

    public int maxSelectedCard = 5;
    private int selectedCardCount = 0;
    [SerializeField] private float cardSpacing = 2f;
    [SerializeField] private float drawCardDelay = 0.15f;
    [SerializeField] Image recipePrefab;
    public RecipeDatabaseSO recipeDatabaseSO;

    public void Init()
    {
        InitializeRecipeSystem();
    }
    public void OnCardDrawn(PlayingCard card, int handPos)
    {

        StartCoroutine(CreateCard(card, handPos, drawCardDelay * handPos));
    }
    private IEnumerator CreateCard(PlayingCard card, int pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        EffectHelper.PlayBounce(GamePlayController.Instance.playerContain.deckController.deckVisual, delay);
        CreateCardView(card, pos);
    }
    private void CreateCardView(PlayingCard card, int pos)
    {
        card.Init();
        
        Vector2 targetPos = CalculateCardPosition(pos, cardViews.Count) + handPos.anchoredPosition;
        card.cardAnim.PlayDrawAnimation(deckPos.anchoredPosition, targetPos, () =>
        {
            UpdateSortPos(cardViews);
        });
    }
    public Vector2 CalculateCardPosition(int index, int totalCards)
    {
        float cardWidth = 145f;
        float startX = -(cardWidth * (totalCards - 1))/2;

        float x = startX + index * cardWidth;
        return new Vector2(x, 0f);
    }
    public void UpdateSortPos(List<PlayingCard> sortedObjects)
    {
        if (sortedObjects.Count == 0) return;

        Sequence moveSequence = DOTween.Sequence();

        for (int i = 0; i < sortedObjects.Count; i++)
            {
            Vector2 targetPos = CalculateCardPosition(i, sortedObjects.Count) + handPos.anchoredPosition;
            sortedObjects[i].cardAnim.originalPosition = targetPos;
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
            GamePlayController.Instance.isLevelDone = false;
            UseProfile.CurrentHand--;
            //isFirstDraw = false;
            selectedCardCount = seletedCards.Count;
            List<PlayingCard> cardsToPlay = new List<PlayingCard>(seletedCards);
            float delay = 0f;
            List<Sequence> playSequences = new List<Sequence>();

            for (int i = 0; i < seletedCards.Count; i++)
            {
                GameController.Instance.musicManager.PlaySelectSound();
                PlayingCard _card = seletedCards[i];

                float spread = 150f;
                float offsetX = (i - (seletedCards.Count - 1) / 2f) * spread;
                Vector2 targetPos = new Vector2(playPos.anchoredPosition.x + offsetX, playPos.anchoredPosition.y);

                float currentDelay = delay;
                cardViews.Remove(_card);

                var playSequence = DOTween.Sequence();
                playSequence.AppendInterval(currentDelay);
                playSequence.Append(_card.cardAnim.PlayHandAnimation(targetPos));
                UpdateSortPos(cardViews);
                playSequences.Add(playSequence);

                delay += 0.15f;
            }

            DOTween.Sequence()
                .Append(playSequences.Count > 0 ? playSequences[playSequences.Count - 1] : null)
                .AppendCallback(() =>
                {
                    Recipe matchResult = RecipeChecker.GetMatchedRecipe(cardsToPlay);

                    List<PlayingCard> validCards = RecipeChecker.GetCardsToScore(cardsToPlay, matchResult);
                    int currentTotal = GamePlayController.Instance.uICtrl.coin.text.ToInt32();

                    Sequence scoreSequence = DOTween.Sequence();
                    PlayScoreAnim(validCards, scoreSequence);

                    scoreSequence.OnComplete(() =>
                    {
                        Sequence mergeSequence = DOTween.Sequence();
                        foreach (var card in cardsToPlay)
                        {
                            Vector2 randomOffset = new Vector2(Random.Range(-25f, 25f), Random.Range(-25f, 25f));
                            Vector2 explodePos = card.GetComponent<RectTransform>().anchoredPosition + randomOffset;

                            mergeSequence.Join(card.GetComponent<RectTransform>().DOAnchorPos(explodePos, 0.2f).SetEase(Ease.OutQuad));
                        }
                        mergeSequence.AppendInterval(0.05f);
                        foreach (var card in cardsToPlay)
                        {
                            mergeSequence.Join(card.GetComponent<RectTransform>().DOAnchorPos(playPos.anchoredPosition, 0.25f).SetEase(Ease.InBack));
                            
                        }
                        mergeSequence.AppendCallback(() =>
                        {
                            foreach (var card in cardsToPlay)
                            {
                                GamePlayController.Instance.playerContain.deckController.DiscardCard(card.GetCardBase());
                                card.GetComponent<RectTransform>().localScale = Vector3.one;
                                SimplePool2.Despawn(card.gameObject);
                            }

                            GameObject recipeImage = SimplePool2.Spawn(recipePrefab.gameObject);
                            recipeImage.transform.SetParent(GamePlayController.Instance.playerContain.deckController.parent.transform, false);
                            recipeImage.GetComponent<RectTransform>().anchoredPosition = playPos.anchoredPosition;
                            var sprite = RecipeCard.Instance.GetSpriteImage(matchResult);
                            recipeImage.GetComponent<Image>().sprite = sprite;
                            recipeImage.GetComponent<RectTransform>().DOAnchorPos(discardPos.anchoredPosition, 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f).OnComplete(() =>
                            {
                                SimplePool2.Despawn(recipeImage.gameObject);
                                GamePlayController.Instance.playerContain.deckController.DrawCards(selectedCardCount);
                                GamePlayController.Instance.uICtrl.CheckWinLoseCondition();
                            });
                        });
                    });
                });
            seletedCards.Clear();
            
        }
    }
    public void PlayScoreAnim(List<PlayingCard> cardsToPlay, Sequence scoreSequence)
    {
        float scoreDelay = 0f;

        foreach (var card in cardsToPlay)
        {
            scoreSequence.AppendInterval(scoreDelay);
            scoreSequence.AppendCallback(() =>
            {
                card.transform.DOMoveY(this.transform.localPosition.y + 1f, card.cardAnim.scaleDuration).SetEase(Ease.OutQuad);
                card.OnActive();
            });
            scoreSequence.AppendInterval(0.2f);
            scoreDelay = 0.05f;
        }
        
        
        
        scoreSequence.AppendInterval(0.3f);

        scoreSequence.AppendCallback(() =>
        {
            int currentScore = GamePlayController.Instance.uICtrl.score.text.ToInt32();
            GameController.Instance.musicManager.PlayMultiSound();
            int finalScore = int.Parse(GamePlayController.Instance.uICtrl.coin.text) * int.Parse(GamePlayController.Instance.uICtrl.multi.text);

            EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.recipe, int.Parse(GamePlayController.Instance.uICtrl.coin.text) * int.Parse(GamePlayController.Instance.uICtrl.multi.text));
            DOTween.To(() => finalScore, x =>
            {
                GameController.Instance.musicManager.PlayChipSound();
                GamePlayController.Instance.uICtrl.recipe.text = x.ToString();
            }, 0, 0.5f).SetEase(Ease.OutCubic);
            DOTween.To(() => currentScore, x =>
            {
                currentScore = x;
                GamePlayController.Instance.uICtrl.score.text = currentScore.ToString();
                EffectHelper.PlayScoreBounce(GamePlayController.Instance.uICtrl.score, currentScore);
            }, currentScore + finalScore, 0.5f).SetEase(Ease.OutCubic);
        }).AppendInterval(0.5f)
    .AppendCallback(() =>
    {
        RecipeChecker.UpdateUIForNoMatch();
    });
        
    }

    public void DiscardSelectedCards()
    {
        if (UseProfile.CurrentDis == 0) return;
        if (seletedCards.Count > 0)
        {
            UseProfile.CurrentDis--;
            selectedCardCount = seletedCards.Count;
            float delay = 0f;
            List<Sequence> playSequences = new List<Sequence>();
            for (int i = 0; i < seletedCards.Count; i++)
            {
                PlayingCard _card = seletedCards[i];
                float spread = 10f;
                float offsetX = (i - (seletedCards.Count - 1) / 2f) * spread;
                Vector2 targetPos = new Vector2(discardPos.anchoredPosition.x + offsetX, discardPos.anchoredPosition.y);
                float currentDelay = delay;
                cardViews.Remove(_card);
                var playSequence = DOTween.Sequence();
                playSequence.AppendInterval(currentDelay);
                playSequence.Append(_card.cardAnim.PlayDiscardAnimation(targetPos));
                playSequences.Add(playSequence);
                delay += 0.15f;
            }
            if (playSequences.Count > 0)
            {
                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        foreach (var sequence in playSequences)
                        {
                            GameController.Instance.musicManager.PlayDiscardSound();
                            sequence.Play();
                        }
                    })
                    .AppendCallback(() =>
                    {
                        UpdateSortPos(cardViews);
                        seletedCards.Clear();
                        
                    })
                    .AppendCallback(()=> GamePlayController.Instance.playerContain.deckController.DrawCards(selectedCardCount));
            }
        }
    }

    public void AnimateCardUpgrade(PlayingCard cardView)
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
