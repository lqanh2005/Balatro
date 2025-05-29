using BestHTTP.Extensions;
using DG.Tweening;
using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    public ShopCtrl shopCtrl;
    public HandleSelect handleSelect;
    public PlayerDataUI playerDataUI;
    public Transform playCtrl;
    public PopupWin popupWin;
    public TopState topState;
    [Header("----------Condition----------")]
    public bool initLevelDone;
    public bool isEnd;
    public int targetScore;

    [Header("----------Recipe Box----------")]  
    public TMP_Text recipe;
    public TMP_Text coin;
    public TMP_Text multi;
    public TMP_Text score;
    public TMP_Text remainCard;
    

    [Header("----------Btn----------")]
    public Button playHandBtn;
    public Button discardBtn;
    public Button runIn4;
    public Button settingBtn;

    [HideInInspector] public float discount = 1f;
    public void Init()
    {
        
        handleSelect.Init();
        playerDataUI.Init();
        popupWin.Init();
        topState.Init();
        shopCtrl.Init();
        runIn4.onClick.AddListener(delegate { RunInfor.Setup().Show(); GameController.Instance.musicManager.PlayClickSound(); GamePlayController.Instance.isLevelDone = false; });
        settingBtn.onClick.AddListener(delegate { SettingGameBox.Setup().Show(); GameController.Instance.musicManager.PlayClickSound(); });
        playHandBtn.onClick.AddListener(delegate { GamePlayController.Instance.playerContain.handManager.PlaySelectedCards(); GameController.Instance.musicManager.PlayClickSound(); });
        discardBtn.onClick.AddListener(delegate { GamePlayController.Instance.playerContain.handManager.DiscardSelectedCards(); GameController.Instance.musicManager.PlayClickSound(); });
        this.RegisterListener(EventID.CHANGE_CARD, delegate { HandleUIDeck(); });
        this.RegisterListener(EventID.START_GAME, delegate { StartGame(); });
    }

    public void HandleUIDeck()
    {
        remainCard.text = UseProfile.DrawCard.ToString() + "/" + UseProfile.CurrentCard.ToString();
    }

    private void StartGame()
    {
        isEnd = false;
        initLevelDone = false;
        playCtrl.gameObject.SetActive(true);
    }

    public void CheckWinLoseCondition()
    {
        if (!initLevelDone || isEnd) return;

        int currentScore = int.Parse(score.text);

        if (currentScore >= targetScore && UseProfile.CurrentHand > 0)
        {
            isEnd = true;
            score.text = "";
            this.PostEvent(EventID.END_GAME);
            popupWin.Show();
        }
        else if (currentScore < targetScore && UseProfile.CurrentHand == 0)
        {
            isEnd = true;
            score.text = "";
            this.PostEvent(EventID.END_GAME);
            LoseBox.Setup().Show();
        }
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.START_GAME, delegate { StartGame(); });
        this.RemoveListener(EventID.CHANGE_CARD, delegate { HandleUIDeck(); });
    }
}

public class EffectHelper
{
    public static void PlayScoreBounce(TMP_Text text, string value)
    {
        if (text == null)
            return;

        text.text = value;
        text.transform.DOKill(true);
        text.transform.DOScale(1f, 0.1f).From(1.2f).SetEase(Ease.OutQuad);
    }
    public static void PlayScoreBounce(TMP_Text textTarget, int value)
    {
        if (textTarget == null)
            return;

        textTarget.text = value.ToString();
        textTarget.transform.DOKill(true);
        textTarget.transform.DOScale(1f, 0.1f).From(1.2f).SetEase(Ease.OutBack);
    }
    public static void PlayBounce(GameObject target, float duration = 0.1f, float scaleMultiplier = 1.2f)
    {
        if (duration >= 0.1f) duration = 0.1f;
        Vector3  originalScales = new Vector3(122.022522f, 119.938721f, 45.5327835f);
        var tf = target.transform;
        Vector3 bounceScale = originalScales * scaleMultiplier;

        tf.DOKill(true);
        tf.DOScale(originalScales, duration)
          .From(bounceScale)
          .SetEase(Ease.OutBack);
        
    }
}
