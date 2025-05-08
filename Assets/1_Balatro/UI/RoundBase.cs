using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;

public class RoundBase : MonoBehaviour
{
    public Button select;
    public TMP_Text target;
    public TMP_Text reward;
    public Image tagg;
    public Button skip;

    public void Init()
    {
        this.RegisterListener(EventID.ON_SELECT_ROUND, delegate { this.SelectedRound(); });
        this.RegisterListener(EventID.ON_SKIP_ROUND, delegate { this.SkipRound(); });
        select.onClick.AddListener(delegate { SelectedRound(); });
        skip.onClick.AddListener(delegate { OnSkip(); });

    }
    public void SelectedRound()
    {
        GamePlayController.Instance.uICtrl.handleSelect.gameObject.SetActive(false);
        GamePlayController.Instance.uICtrl.playCtrl.gameObject.SetActive(true);
        GamePlayController.Instance.StartGame();
        
        //this.PostEvent(EventID.START_GAME);
    }
    public void SkipRound()
    {
        Debug.LogError("Skip Round");
    }
    public void OnSelect()
    {
        this.PostEvent(EventID.ON_SELECT_ROUND);
    }
    public void OnSkip()
    {
        this.PostEvent(EventID.ON_SKIP_ROUND);
    }
    public void OnDisable()
    {
        this.RemoveListener(EventID.ON_SELECT_ROUND, delegate { this.SelectedRound(); });
        this.RemoveListener(EventID.ON_SKIP_ROUND, delegate { this.SkipRound(); });
    }
}
