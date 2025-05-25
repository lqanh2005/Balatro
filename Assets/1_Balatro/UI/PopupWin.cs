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
    public Image iamge_1;
    public Image iamge_2;
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
        getRewardBtn.onClick.AddListener(delegate { GetReward(); });
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        total = 0;
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

        yield return StartCoroutine(ShowText(textList[0], "Lượt Đánh Còn lại [$1 mỗi lượt]"));
        yield return StartCoroutine(ShowDollarIncrement(reward_1, UseProfile.CurrentHand));

        if (UseProfile.CurrentGold >= 5)
        {
            profit_2.text = UseProfile.CurrentGold.ToString();
            yield return StartCoroutine(ShowText(textList[1], "1 lợi tức cho mỗi $5 [tối đa 5]"));
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
