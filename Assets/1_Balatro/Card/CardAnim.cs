using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SocialPlatforms;

public class CardAnim : MonoBehaviour
{
    [Header("---------------Data---------------")]
    public RectTransform rect;
    [HideInInspector] public float flipDuration = 0.3f;
    [HideInInspector] public float moveDuration = 0.5f;
    [HideInInspector] public float scaleDuration = 0.2f;
    [HideInInspector] public float hoverScale = 1.1f;
    [HideInInspector] public float hoverLiftHeight = 0.1f;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Vector3 originalScale;
    public Sequence sequence;
    [HideInInspector] public bool isFaceUp = false;

    public void Init()
    {
        originalScale = rect.localScale;
        //Vector3 calculatedPosition = GamePlayController.Instance.playerContain.handManager.CalculateCardPosition(pos, GamePlayController.Instance.playerContain.handManager.cardViews.Count);
        
    }

    public void PlaySelectedAniamtion(bool select)
    {
        transform.DOKill();
        if (select)
        {
            rect.DOAnchorPosY(originalPosition.y + 30f, scaleDuration).SetEase(Ease.OutQuad);
            rect.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            rect.DOAnchorPosY(originalPosition.y, scaleDuration).SetEase(Ease.OutQuad);
            rect.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
        }
    }
    public void PlayDrawAnimation(Vector2 startPos, Vector2 endPos, System.Action onComplete = null)
    {
        rect.anchoredPosition = startPos;
        rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void PlaySelectedAniamtion1()
    {
        rect.DOKill();


        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOAnchorPosY(originalPosition.y + 20f, scaleDuration / 2).SetEase(Ease.OutQuad))
      .Append(rect.DOAnchorPosY(originalPosition.y, scaleDuration / 2).SetEase(Ease.InQuad));
    }
    public void PlayHoverAniamtion(bool isHovering, bool isSelected)
    {
        if (isSelected) return;
        if (isHovering)
        {
            rect.DOAnchorPosY(originalPosition.y + (hoverLiftHeight / 2f), scaleDuration / 2).SetEase(Ease.OutQuad);
            rect.DOScale(originalScale * (hoverScale - 0.05f), scaleDuration / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            rect.DOAnchorPosY(originalPosition.y, scaleDuration / 2).SetEase(Ease.OutQuad);
            rect.DOScale(originalScale, scaleDuration / 2).SetEase(Ease.OutQuad);
        }
    }
    public void PlayHoverAniamtion()
    {
        rect.DOAnchorPosY(rect.anchoredPosition.y + 100f, scaleDuration).SetEase(Ease.OutQuad); // localPosition.y + 1 → 100 pixel
        rect.DOScale(originalScale * 1.5f, scaleDuration).SetEase(Ease.OutQuad);
    }
    public Sequence PlayHandAnimation(Vector3 targetPos)
    {
        if (sequence != null) sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(rect.DOAnchorPos(targetPos, moveDuration).SetEase(Ease.OutBack));
        return sequence;
    }
    public Sequence PlayDiscardAnimation(Vector3 targetPos)
    {
        rect.localScale = Vector3.one;
        if (sequence != null) sequence.Kill();

        sequence = DOTween.Sequence();
        sequence.Append(rect.DOAnchorPos(targetPos, moveDuration).SetEase(Ease.OutQuad))
                .Join(rect.DORotate(new Vector3(0, 0, -180f), moveDuration));
        return sequence;
    }
    
}
 