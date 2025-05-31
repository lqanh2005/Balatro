using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher;

public class HandleSelect : MonoBehaviour
{
    public List<RoundBase> rounds = new List<RoundBase>();
    [HideInInspector] public int currentRound;
    public RectTransform rect;

    public void Init()
    {
        this.RegisterListener(EventID.ON_SELECT_ROUND, delegate { OnSelected(); });
        
    }
    public void OnSelected()
    {
        this.gameObject.SetActive(true);
        GameController.Instance.musicManager.PlayCreatDeckSound();
        currentRound = UseProfile.CurrentRound;
        rounds[currentRound].isActive=true;
        rect.DOAnchorPosY(GamePlayController.Instance.uICtrl.target.anchoredPosition.y+170f, 0.5f).SetEase(Ease.OutBack);
        foreach (var round in rounds)
        {
            round.Init();
        }
    }
    public void Hide()
    {
        rect.DOAnchorPosY(-(GamePlayController.Instance.uICtrl.target.anchoredPosition.y+150), 0.5f).SetEase(Ease.InBack);
        this.gameObject.SetActive(false);
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.ON_SELECT_ROUND, delegate { OnSelected(); });
    }
}
