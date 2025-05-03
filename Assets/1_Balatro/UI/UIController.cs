using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button playHandBtn;
    public Button shopBtn;

    [Header("----------Recipe Box----------")]  
    public TMP_Text recipe;
    public TMP_Text coin;
    public TMP_Text multi;
    public TMP_Text score;


    public void Init()
    {
        playHandBtn.onClick.AddListener(delegate { GamePlayController.Instance.playerContain.handManager.PlaySelectedCards(); });
        shopBtn.onClick.AddListener(delegate { ShopCtrl.Setup().Show(); });
    }
}

public class TextEffectHelper
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
}
