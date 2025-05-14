using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;
using System;
using DG.Tweening;

public class RoundBase : MonoBehaviour
{
    public int id;
    public bool isActive;
    public GameObject panel;
    [Header("------------------------------Top------------------------------------")]
    public Button select;
    public Image image1;
    public TMP_Text text1;
    public TMP_Text selectTMP;
    [Header("------------------------------Mid------------------------------------")]
    public TMP_Text target;
    public TMP_Text reward;
    [Header("------------------------------Bottom------------------------------------")]
    public Image tagg;
    public Button skip;

    public void Init()
    {
        select.onClick.RemoveAllListeners();
        select.onClick.AddListener(delegate { SelectedRound(); });
        SetUpData();
        if (isActive)
        {
            panel.SetActive(false);
            selectTMP.text = "Chọn";
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            pos.y = -150;
            rect.anchoredPosition = pos;

        }
        else
        {
            panel.SetActive(true);
            selectTMP.text = "Sắp Tới";
            RectTransform rect = GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            pos.y = -200f;
            rect.anchoredPosition = pos;
        }
    }

    private void SetUpData()
    {
        var data = ConfigLevel.Instance.GetData(UseProfile.CurrentAnte, id);
        target.text = data.target.ToString();
        StartCoroutine(ShowDollarIncrement(reward, data.reward));
    }
    private IEnumerator ShowDollarIncrement(TMP_Text text, int dollarAmount)
    {
        text.text = "Thưởng: ";

        for (int i = 0; i < dollarAmount; i++)
        {
            text.text += "$";
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SelectedRound()
    {
        Debug.LogError("Selected Round: " + id);
        isActive = false;
        GamePlayController.Instance.uICtrl.handleSelect.gameObject.SetActive(false);
        GamePlayController.Instance.uICtrl.playCtrl.gameObject.SetActive(true);
        UseProfile.CurrentRound++;
        GamePlayController.Instance.uICtrl.topState.image1.color = image1.color;
        GamePlayController.Instance.uICtrl.topState.text1.text = text1.text;
        //GamePlayController.Instance.uICtrl.topState.image2.color = tagg.color;
        GamePlayController.Instance.uICtrl.topState.text2.text = target.text;
        GamePlayController.Instance.uICtrl.topState.text3.text = reward.text;
        GamePlayController.Instance.uICtrl.targetScore = int.Parse(target.text);
        this.PostEvent(EventID.START_GAME);
        GamePlayController.Instance.uICtrl.initLevelDone = true;
        
    }
}
