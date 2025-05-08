using BestHTTP.Extensions;
using DG.Tweening;
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
    public bool shopUp;

    [Header("----------Recipe Box----------")]  
    public TMP_Text recipe;
    public TMP_Text coin;
    public TMP_Text multi;
    public TMP_Text score;
    public VoucherSO voucherSO;
    
    [Header("----------Btn----------")]
    public Button playHandBtn;
    public Button discardBtn;

    public void Start()
    {
        playCtrl.gameObject.SetActive(false);
        handleSelect.Init();
        playerDataUI.Init();
    }
    public void Init()
    {
        playHandBtn.onClick.AddListener(delegate { GamePlayController.Instance.playerContain.handManager.PlaySelectedCards(); });
        discardBtn.onClick.AddListener(delegate { GamePlayController.Instance.playerContain.handManager.DiscardSelectedCards(); });
    }
    public void SetUpData()
    {

    }
    public void Update()
    {
        if(score.text.ToInt32() >= 300)
        {
            foreach(var item in GamePlayController.Instance.playerContain.deckController.drawCards)
            {
                SimplePool2.Despawn(item.gameObject);
            }
            foreach(var item in GamePlayController.Instance.playerContain.deckController.handCards)
            {
                SimplePool2.Despawn(item.gameObject);
            }
            foreach (var item in GamePlayController.Instance.playerContain.handManager.cardViews)
            {
                SimplePool2.Despawn(item.gameObject);
            }
            if (shopUp)
            {
                shopUp = false;
                shopCtrl.Show();
            }
            
        }
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
        if (target == null) return;

        var tf = target.transform;
        Vector3 originalScale = tf.localScale;
        Vector3 bounceScale = originalScale * scaleMultiplier;

        tf.DOKill(true);
        tf.DOScale(originalScale, duration)
          .From(bounceScale)
          .SetEase(Ease.OutBack);
        
    }
}
