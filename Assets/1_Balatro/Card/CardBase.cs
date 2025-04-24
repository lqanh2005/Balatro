using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardBase : MonoBehaviour
{
    [Header("---------------Data---------------")]
    public int id;
    public int level;
    public CardType cardType;
    public SpriteRenderer cardImage;
    public SpriteRenderer cardBackImage;

    [Header("Animation Settings")]
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverLiftHeight = 0.1f;

    private bool isFaceUp = false;
    private bool isSelected = false;
    private bool isDrag = false;
    private bool isMouseDown = false;
    private bool isDraw = false;
    private Vector3 initialMousePos;
    public Vector3 originalPosition;
    private Vector3 originalScale;

    private Sequence sequence;

    public void Init(int pos)
    {
        //cardImage.sprite = ConfigData.Instance.cardLists[(int)cardType].cardPerLevels[this.level].cardDatas.faceImage;
        //SetFaceUp(faceUp, false);
        originalScale = transform.localScale;
        isDraw = true;
        originalPosition = GamePlayController.Instance.playerContain.handManager.handPosList[pos].localPosition;
        this.transform.SetParent(GamePlayController.Instance.playerContain.handManager.handPos, true);
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
    public void PlayDrawAnimation(Vector3 startPos, Vector3 endPos)
    {
        if(sequence != null)
        {
            sequence.Kill();
        }
        transform.position = startPos;
        transform.localScale = Vector3.zero;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale, scaleDuration))
            .Join(transform.DOMove(endPos, moveDuration).SetEase(Ease.OutBack));
    }
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
    public void PlaySelectedAniamtion1()
    {
        transform.DOKill();

        float targetY = originalPosition.y + 0.2f;
        Vector3 targetScale = originalScale * hoverScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(targetY, scaleDuration / 2).SetEase(Ease.OutQuad))
           .Append(transform.DOLocalMoveY(originalPosition.y, scaleDuration / 2).SetEase(Ease.InQuad));
    }
    public void PlayHoverAniamtion(bool isHovering)
    {
        if(isSelected) return;
        if (isHovering)
        {
            transform.DOLocalMoveY(originalPosition.y + (hoverLiftHeight/2), scaleDuration/2).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale * (hoverScale - 0.05f), scaleDuration / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            transform.DOLocalMoveY(originalPosition.y, scaleDuration / 2).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale, scaleDuration / 2).SetEase(Ease.OutQuad);
        }
    }
    public Sequence PlayHandAnimation(Vector3 targetPos)
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
        this.transform.SetParent(GamePlayController.Instance.playerContain.handManager.discardPos);
        sequence.Append(transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutBack))
            .Join(transform.DORotate(new Vector3(0,0, Random.Range(-10f, 10f)), moveDuration))
            .Join(canvasGroup.DOFade(0, moveDuration * 1.2f));
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
        sequence.Append(transform.DOMove(targetPos, moveDuration).SetEase(Ease.OutQuad))
            .Join(transform.DORotate(new Vector3(0,0,-180), moveDuration))
            .Join(canvasGroup.DOFade(0, moveDuration));
        return sequence;
    }
    // cmt
    public void PlaySpecialCard()
    {

    }
    private void OnDestroy()
    {
        DOTween.Kill(transform);
        if(sequence != null)
        {
            sequence.Kill();
        }
    }
    public CardBase GetCardBase()
    {
        return this;
    }

    public void PlaySnapToPos()
    {
        transform.DOKill();
        this.transform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
    }

    private void OnMouseDown()
    {
        if (!isDraw) return;
        isMouseDown = true;
        isDrag = false;
        initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialMousePos.z = 0;
    }

    private void OnMouseUp()
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
                    PlaySelectedAniamtion(isSelected);
                }
                else
                {
                    PlaySelectedAniamtion1();
                }
                return;
            }
            else
            {
                GamePlayController.Instance.playerContain.handManager.seletedCards.Remove(this);
                PlaySelectedAniamtion(isSelected);
                return;
            }
        }
        else
        {
            this.transform.DOLocalMove(new Vector3(originalPosition.x, originalPosition.y + (isSelected? 0.2f:0f)), 0.3f).SetEase(Ease.OutBack);
        }

        isDrag = false;
    }
    private void FixedUpdate()
    {
        if (!isDraw) return;
        if (isMouseDown)
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;

            float dragThreshold = 0.01f;

            if (!isDrag && Vector3.Distance(initialMousePos, currentMousePos) > dragThreshold)
            {
                isDrag = true;
            }

            if (isDrag)
            {
                this.transform.position = currentMousePos;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (isDrag) return;
        if (!isDraw) return;
        PlayHoverAniamtion(true);
    }
    private void OnMouseExit()
    {
        if(isDrag) return;
        if (!isDraw) return;
        PlayHoverAniamtion(false);
    }
}
 