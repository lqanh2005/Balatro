using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBase : MonoBehaviour
{
    public int id;


    public string description;
    public GameObject showText;

    [HideInInspector] public float flipDuration = 0.3f;
    [HideInInspector] public float moveDuration = 0.5f;
    [HideInInspector] public float scaleDuration = 0.2f;
    [HideInInspector] public float hoverScale = 1.1f;
    [HideInInspector] public float hoverLiftHeight = 0.1f;



    [HideInInspector] public bool isFaceUp = false;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool isDrag = false;
    [HideInInspector] public bool isMouseDown = false;
    [HideInInspector] public bool isDraw = false;
    [HideInInspector] public Vector3 initialMousePos;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Vector3 originalScale;

    public Sequence sequence;

    public virtual void Init(int pos)
    {
        originalScale = transform.localScale;
        isDraw = true;
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
    public void PlayHoverAniamtion(bool isHovering)
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
    public void FixedUpdate()
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

    public void OnMouseDown()
    {
        if (!isDraw) return;
        isMouseDown = true;
        isDrag = false;
        initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialMousePos.z = 0;

    }
    public void OnMouseEnter()
    {
        if (isDrag) return;
        if (!isDraw) return;
        PlayHoverAniamtion(true);
        showText.gameObject.SetActive(true);
    }
    public void OnMouseExit()
    {
        if (isDrag) return;
        if (!isDraw) return;
        PlayHoverAniamtion(false);
        showText.gameObject.SetActive(false);
    }
    public virtual void OnMouseUp()
    {

    }
    public void OnDestroy()
    {
        DOTween.Kill(transform);
        if (sequence != null)
        {
            sequence.Kill();
        }
    }
}
