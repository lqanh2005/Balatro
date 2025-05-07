using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SocialPlatforms;

public class CardAnim : MonoBehaviour
{
    [Header("---------------Data---------------")]
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
        originalScale = transform.localScale;
        //Vector3 calculatedPosition = GamePlayController.Instance.playerContain.handManager.CalculateCardPosition(pos, GamePlayController.Instance.playerContain.handManager.cardViews.Count);
        
    }

    //private void SetFaceUp(bool faceUp, bool animate = true)
    //{
    //    if (isFaceUp == faceUp) return;
    //    isFaceUp = faceUp;
    //    if (animate)
    //    {
    //        if(sequence != null)
    //        {
    //            sequence.Kill();
    //        }
    //        sequence = DOTween.Sequence();
    //        sequence.Append(transform.DOScaleX(0, flipDuration / 2)).OnComplete(() =>
    //        {
    //            if (cardImage != null)
    //            {
    //                cardImage.gameObject.SetActive(faceUp);
    //            }
    //            if (cardBackImage != null)
    //            {
    //                cardBackImage.gameObject.SetActive(!faceUp);
    //            }
    //        }).Append(transform.DOScaleX(originalScale.x, flipDuration / 2));
    //    }
    //    else
    //    {
    //        if (cardImage != null)
    //        {
    //            cardImage.gameObject.SetActive(faceUp);
    //        }
    //        if (cardBackImage != null)
    //        {
    //            cardBackImage.gameObject.SetActive(!faceUp);
    //        }
    //    }
    //}
    //public void FlipCard()
    //{
    //    SetFaceUp(!isFaceUp, true);
    //}
    public void PlaySelectedAniamtion(bool select)
    {
        transform.DOKill();
        if (select)
        {
            this.transform.DOLocalMoveY(originalPosition.y + 0.3f, scaleDuration).SetEase(Ease.OutQuad);
            this.transform.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            this.transform.DOLocalMoveY(originalPosition.y, scaleDuration).SetEase(Ease.OutQuad);
            this.transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
        }
    }
    public void PlayDrawAnimation(Vector3 startPos, Vector3 endPos, bool check, System.Action onComplete = null)
    {
        transform.position = startPos;
        if (check)
        {
            transform.DOMove(endPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete?.Invoke();
            transform.DOMove(endPos, 0.5f).SetEase(Ease.OutBack);
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
    public void PlayHoverAniamtion(bool isHovering, bool isSelected)
    {
        if (isSelected) return;
        if (isHovering)
        {
            transform.DOLocalMoveY(originalPosition.y + (hoverLiftHeight / 2), scaleDuration / 2).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale * (hoverScale - 0.05f), scaleDuration / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            transform.DOLocalMoveY(originalPosition.y, scaleDuration / 2).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale, scaleDuration / 2).SetEase(Ease.OutQuad);
        }
    }
    public void PlayHoverAniamtion()
    {
        this.transform.DOMoveY(this.transform.localPosition.y + 1f, scaleDuration).SetEase(Ease.OutQuad);
        this.transform.DOScale(originalScale * 1.5f, scaleDuration).SetEase(Ease.OutQuad);
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
    

    public void PlaySnapToPos()
    {
        transform.DOKill();
        this.transform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
    }
    
}
 