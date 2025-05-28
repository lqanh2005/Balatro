using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;

public class PopupWin : MonoBehaviour
{
    public Button getRewardBtn;
    public Image avt;
    public TMP_Text score;
    public TMP_Text reward;
    public List<TMP_Text> textList;
    public TMP_Text profit_1;
    public TMP_Text reward_1;
    public TMP_Text profit_2;
    public TMP_Text reward_2;
    public TMP_Text finalReward;

    private int total;

    public void Init()
    {
        this.RegisterListener(EventID.END_GAME, delegate { Show(); });
        getRewardBtn.onClick.AddListener(delegate { GetReward(); GameController.Instance.musicManager.PlayClickSound(); });
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        avt = GamePlayController.Instance.uICtrl.topState.avt;
        score.text = GamePlayController.Instance.uICtrl.topState.text2.text;
        reward.text = GamePlayController.Instance.uICtrl.topState.text3.text;
        total = 0;
        total += reward.text.Length;
        finalReward.text = "";
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(371, 0.5f).SetEase(Ease.OutCubic);
        OnReward();
    }

    private void OnReward()
    {
        StartCoroutine(ShowRewardSequence());
    }
    public IEnumerator ShowRewardSequence()
    {
        profit_1.text = UseProfile.CurrentHand.ToString();

        yield return StartCoroutine(ShowText(textList[0], "Remaining Hands ($1 each)"));
        yield return StartCoroutine(ShowDollarIncrement(reward_1, UseProfile.CurrentHand));

        if (UseProfile.CurrentGold >= 5)
        {
            profit_2.text = UseProfile.CurrentGold.ToString();
            yield return StartCoroutine(ShowText(textList[1], "1 interest per $5 (5 max)"));
            yield return StartCoroutine(ShowDollarIncrement(reward_2, UseProfile.CurrentGold / 5));
        }
        total = UseProfile.CurrentHand + ((UseProfile.CurrentGold / 5)>5? 5:(UseProfile.CurrentGold/5));
        getRewardBtn.gameObject.SetActive(true);
        finalReward.text = total.ToString();
    }
    private IEnumerator ShowText(TMP_Text text, string v)
    {
        string final = "";
        char[] charArray = v.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            final += charArray[i];
            text.text = final;
            yield return Helper.GetWait(0.02f);
        }
    }
    private IEnumerator ShowDollarIncrement(TMP_Text text, int dollarAmount)
    {
        text.text = "";
        if(dollarAmount>=5) dollarAmount = 5;
        for (int i = 0; i < dollarAmount; i++)
        {
            text.text += "$";
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void GetReward()
    {
        GameController.Instance.musicManager.PlayGetGoldSound();
        UseProfile.CurrentGold += total;
        Debug.LogError("total = " + total);
        Close();
        UseProfile.SavedState = StateGame.Shopping;
        this.PostEvent(EventID.ON_SHOPPING);
    }
    public void Close()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(-400, 0.5f).SetEase(Ease.OutCubic);
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        this.RemoveListener(EventID.END_GAME, delegate { Show(); });
    }
    private void OnDestroy()
    {
        this.RemoveListener(EventID.END_GAME, delegate { Show(); });
    }
}
