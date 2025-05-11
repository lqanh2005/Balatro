using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher;
using System;
using TMPro;
using UnityEngine.UI;

public class TopState : MonoBehaviour
{
    public GameObject dataUI;
    public GameObject textUI;
    public GameObject shopUI;

    public Image image1;
    public TMP_Text text1;
    public Image image2;
    public TMP_Text text2;
    public TMP_Text text3;

    public void Init()
    {
        this.RegisterListener(EventID.ON_SELECT_ROUND, delegate { Close(shopUI, () => Show(textUI)); });
        this.RegisterListener(EventID.START_GAME, delegate { Close(textUI, () => Show(dataUI)); });
        this.RegisterListener(EventID.END_GAME, delegate { Close(dataUI, () => Show(shopUI)); });
    }
    public void Show(GameObject param)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(-290, 0.5f).SetEase(Ease.OutCubic);
        param.SetActive(true);
    }
    public void Close(GameObject param, Action onComplete = null)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(100, 0.5f).SetEase(Ease.OutCubic).
            SetEase(Ease.OutCubic).OnComplete(() =>
            {
                param.SetActive(false);
                onComplete?.Invoke();
            });
    }
    public void OnDisable()
    {
        this.RemoveListener(EventID.ON_SELECT_ROUND, delegate { Close(shopUI); Show(textUI); });
        this.RemoveListener(EventID.START_GAME, delegate { Close(textUI); Show(dataUI); });
        this.RemoveListener(EventID.END_GAME, delegate { Close(dataUI); Show(shopUI); });
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.ON_SELECT_ROUND, delegate { Close(shopUI); Show(textUI); });
        this.RemoveListener(EventID.START_GAME, delegate { Close(textUI); Show(dataUI); });
        this.RemoveListener(EventID.END_GAME, delegate { Close(dataUI); Show(shopUI); });
    }
}
