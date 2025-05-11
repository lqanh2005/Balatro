using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;

public class PlayerDataUI : MonoBehaviour
{
    [Header("----------Player----------")]
    public TMP_Text gold;
    public TMP_Text cntPlay;
    public TMP_Text cntDis;
    public TMP_Text ante;
    public TMP_Text round;
    public Button settingBtn;
    public Button runIn4;

    public void Init()
    {
        this.RegisterListener(EventID.CHANGE_HAND, delegate { HandleChangeHand(null); });
        this.RegisterListener(EventID.CHANGE_DIS, delegate { HandleChangeDis(null); });
        this.RegisterListener(EventID.CHANGE_GOLD, delegate { HandleChangeGold(null); });
        this.RegisterListener(EventID.CHANGE_ROUND, delegate { HandleChangeRound(null); });
        this.RegisterListener(EventID.CHANGE_ANTE, delegate { HandleChangeAnte(null); });
        UpdatePlayerUI();
        settingBtn.onClick.AddListener(delegate { SettingBox.Setup(true).Show(); });
    }

    public void UpdatePlayerUI()
    {
        gold.text = UseProfile.CurrentGold.ToString();
        UseProfile.CurrentHand = 4;
        UseProfile.CurrentDis = 4;
    }
    public void OnDisable()
    {
        this.RemoveListener(EventID.CHANGE_HAND, delegate { HandleChangeHand(null); });
        this.RemoveListener(EventID.CHANGE_DIS, delegate { HandleChangeDis(null); });
        this.RemoveListener(EventID.CHANGE_GOLD, delegate { HandleChangeGold(null); });
        this.RemoveListener(EventID.CHANGE_ROUND, delegate { HandleChangeRound(null); });
        this.RemoveListener(EventID.CHANGE_ANTE, delegate { HandleChangeAnte(null); });
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.CHANGE_HAND, delegate { HandleChangeHand(null); });
        this.RemoveListener(EventID.CHANGE_DIS, delegate { HandleChangeDis(null); });
        this.RemoveListener(EventID.CHANGE_GOLD, delegate { HandleChangeGold(null); });
        this.RemoveListener(EventID.CHANGE_ROUND, delegate { HandleChangeRound(null); });
        this.RemoveListener(EventID.CHANGE_ANTE, delegate { HandleChangeAnte(null); });
    }
    public void HandleChangeHand(object param)
    {
        cntPlay.text = UseProfile.CurrentHand.ToString();
    }
    public void HandleChangeDis(object param)
    {
        cntDis.text = UseProfile.CurrentDis.ToString();
    }
    public void HandleChangeGold(object param)
    {
        gold.text = UseProfile.CurrentGold.ToString();
    }
    public void HandleChangeRound(object param)
    {
        round.text = UseProfile.CurrentRound.ToString();
    }
    public void HandleChangeAnte(object param)
    {
        ante.text = UseProfile.CurrentAnte.ToString() + "/8";
    }
}
