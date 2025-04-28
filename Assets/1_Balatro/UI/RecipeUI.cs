using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject formula;

    public void OnPointerEnter(PointerEventData eventData)
    {
        formula.gameObject.SetActive(true);
        Debug.LogError("Mouse Entered Recipe UI");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        formula.gameObject.SetActive(false);
    }

}
