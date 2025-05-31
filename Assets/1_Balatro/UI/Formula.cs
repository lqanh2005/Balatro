using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Formula : MonoBehaviour
{
    public RectTransform formulaPanel;
    public RectTransform canvasRect;
    public Image avt;

    public void ShowFormula(RectTransform recipeTransform, Sprite avt)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, recipeTransform.position),
            null,
            out localPoint);
        this.avt.sprite = avt;
        formulaPanel.anchoredPosition = localPoint + new Vector2(0, 180);
        formulaPanel.gameObject.SetActive(true);
    }

    public void HideFormula()
    {
        formulaPanel.gameObject.SetActive(false);
    }

}
