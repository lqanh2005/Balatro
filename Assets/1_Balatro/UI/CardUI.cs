using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(CanvasGroup))]
public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public int id;

    public LayoutElement layoutElement;
    public CanvasGroup canvasGroup;
    public string description;
    public GameObject image;
    public HandleLayout handleLayout;
    public RectTransform rectTransform;

    [HideInInspector] public float flipDuration = 0.3f;
    [HideInInspector] public float moveDuration = 0.3f;
    [HideInInspector] public float scaleDuration = 0.2f;
    [HideInInspector] public float hoverScale = 1.1f;
    [HideInInspector] public float hoverLiftHeight = 0.1f;
    [HideInInspector] public float originalWidth;

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutElement = GetComponent<LayoutElement>();
        canvasGroup = GetComponent<CanvasGroup>();
        handleLayout = GetComponentInParent<HandleLayout>();
        originalWidth = layoutElement.preferredWidth;
    }
    public void Hide()
    {
        SimplePool2.Despawn(this.gameObject);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        layoutElement.preferredWidth = 0;   
        canvasGroup.alpha = 0;

        DOTween.To(() => layoutElement.preferredWidth,
                   x => layoutElement.preferredWidth = x,
                   originalWidth,
                   moveDuration);
        canvasGroup.DOFade(1, moveDuration);
    }
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    Hide();
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        Show();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Hide();
    }
    public void MoveAnimation()
    {
        Vector2 targetPosition = new Vector2(0, 0);
        rectTransform.DOAnchorPos(targetPosition, 2f)
            .SetEase(Ease.OutQuad);
    }
}
