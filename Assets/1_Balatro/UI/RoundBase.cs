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
    public RectTransform rectTransform;
    [Header("------------------------------Top------------------------------------")]
    public Button select;
    public Image avt;
    [HideInInspector] public string text1;
    [Header("------------------------------Mid------------------------------------")]
    public TMP_Text target;
    public TMP_Text reward;

    public void Init()
    {
        select.onClick.RemoveAllListeners();
        select.onClick.AddListener(delegate { SelectedRound(); });
        SetUpData();
        if (isActive)
        {
            panel.SetActive(false);
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = -226;
            rectTransform.anchoredPosition = pos;

        }
        else
        {
            panel.SetActive(true);
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = -362;
            rectTransform.anchoredPosition = pos;
        }
    }

    private void SetUpData()
    {
        var data = ConfigLevel.Instance.GetData(UseProfile.CurrentAnte, id);
        target.text = data.target.ToString();
        text1 = data.name;
        reward.text = "Reward: ";
        for (int i = 0; i < data.reward; i++)
        {
            reward.text += "$";
        }
    }

    public void SelectedRound()
    {
        isActive = false;
        GameController.Instance.musicManager.PlayClickSound();
        GamePlayController.Instance.uICtrl.handleSelect.gameObject.SetActive(false);
        GamePlayController.Instance.uICtrl.playCtrl.gameObject.SetActive(true);
        UseProfile.CurrentRound++;
        GamePlayController.Instance.uICtrl.topState.avt = avt;
        GamePlayController.Instance.uICtrl.topState.text1.text = text1;
        GamePlayController.Instance.uICtrl.topState.text2.text = target.text;
        GamePlayController.Instance.uICtrl.topState.text3.text = reward.text;
        GamePlayController.Instance.uICtrl.targetScore = int.Parse(target.text);
        UseProfile.SavedState = StateGame.Playing;
        this.PostEvent(EventID.START_GAME);
        GamePlayController.Instance.uICtrl.initLevelDone = true;
        
    }
}
