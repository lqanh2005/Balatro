using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SocialPlatforms;

public class CardView : CardBase
{
    [Header("---------------Data---------------")]
    
    public int level;
    public int chip;
    public IngredientType ingredientType;
    public SpriteRenderer cardImage;
    public SpriteRenderer cardBackImage;

    public override void Init(int pos)
    {
        base.Init(pos);
        Vector3 calculatedPosition = GamePlayController.Instance.playerContain.handManager.CalculateCardPosition(pos, GamePlayController.Instance.playerContain.handManager.cardViews.Count);
        this.chip = ConfigData.Instance.GetChip(this.ingredientType, this.level);
    }

    private void SetFaceUp(bool faceUp, bool animate = true)
    {
        if (isFaceUp == faceUp) return;
        isFaceUp = faceUp;
        if (animate)
        {
            if(sequence != null)
            {
                sequence.Kill();
            }
            sequence = DOTween.Sequence();
            sequence.Append(transform.DOScaleX(0, flipDuration / 2)).OnComplete(() =>
            {
                if (cardImage != null)
                {
                    cardImage.gameObject.SetActive(faceUp);
                }
                if (cardBackImage != null)
                {
                    cardBackImage.gameObject.SetActive(!faceUp);
                }
            }).Append(transform.DOScaleX(originalScale.x, flipDuration / 2));
        }
        else
        {
            if (cardImage != null)
            {
                cardImage.gameObject.SetActive(faceUp);
            }
            if (cardBackImage != null)
            {
                cardBackImage.gameObject.SetActive(!faceUp);
            }
        }
    }
    public void FlipCard()
    {
        SetFaceUp(!isFaceUp, true);
    }
    public void PlayDrawAnimation(Vector3 startPos, Vector3 endPos, bool check, System.Action onComplete = null)
    {
        transform.position = startPos;
        if (check)
        {
            transform.DOLocalMove(endPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
            transform.DOLocalMove(endPos, 0.5f).SetEase(Ease.OutBack);
        }
    }
    
    public void PlaySelectedAniamtion1()
    {
        transform.DOKill();

        float targetY = originalPosition.y + 0.2f;
        Vector3 targetScale = originalScale * hoverScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(targetY, scaleDuration / 2).SetEase(Ease.OutQuad))
           .Append(transform.DOLocalMoveY(originalPosition.y, scaleDuration / 2).SetEase(Ease.InQuad));
    }
    
    public Sequence PlayHandAnimation(Vector3 targetPos)
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
        this.transform.SetParent(GamePlayController.Instance.playerContain.handManager.playPos);
        sequence.Append(transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutBack))
            .Join(transform.DORotate(new Vector3(0, 0, Random.Range(-10f, 10f)), moveDuration));
        return sequence;
    }
    public Sequence PlayDiscardAnimation(Vector3 targetPos)
    {
        if(sequence != null) sequence.Kill();
        sequence = DOTween.Sequence();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
        this.transform.SetParent(GamePlayController.Instance.playerContain.handManager.discardPos);
        sequence.Append(transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutQuad))
            .Join(transform.DORotate(new Vector3(0,0,-180), moveDuration))
            .Join(canvasGroup.DOFade(0, moveDuration));
        return sequence;
    }
    // cmt
    public void PlaySpecialCard()
    {

    }
    
    public CardView GetCardBase()
    {
        return this;
    }

    public void PlaySnapToPos()
    {
        transform.DOKill();
        this.transform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
    }

    public override void OnMouseUp()
    {
        if (!isDraw) return;
        isMouseDown = false;

        if (!isDrag)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
                if (GamePlayController.Instance.playerContain.handManager.seletedCards.Count < GamePlayController.Instance.playerContain.handManager.maxSelectedCard)
                {
                    GamePlayController.Instance.playerContain.handManager.seletedCards.Add(this);
                    RecipeChecker.GetMatchedRecipe(GamePlayController.Instance.playerContain.handManager.seletedCards);
                    PlaySelectedAniamtion(isSelected);
                }
                else
                {
                    PlaySelectedAniamtion1();
                }
            }
            else
            {
                GamePlayController.Instance.playerContain.handManager.seletedCards.Remove(this);
                RecipeChecker.GetMatchedRecipe(GamePlayController.Instance.playerContain.handManager.seletedCards);
                PlaySelectedAniamtion(isSelected);
            }
        }
        else
        {
            this.transform.DOLocalMove(new Vector3(originalPosition.x, originalPosition.y + (isSelected? 0.2f:0f)), 0.3f).SetEase(Ease.OutBack);
        }
        
        isDrag = false;
    }
    
}
 