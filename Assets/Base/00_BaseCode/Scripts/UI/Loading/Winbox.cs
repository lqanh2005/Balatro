using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Winbox : BaseBox
{
    public static Winbox _instance;
    public static Winbox Setup()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<Winbox>(PathPrefabs.WIN_BOX));
            _instance.Init();
        }
        _instance.InitState();
        return _instance;
    }

    public Button nextButton;
    public Button rewardButton;
    public CoinHeartBar coinHeartBar;
    public Text tvCoin;
    public Text tvCoin_2;
    public CanvasGroup canvasGroup;
    public void Init()
    {
        nextButton.onClick.AddListener(delegate { HandleNext();    });
        rewardButton.onClick.AddListener(delegate { HandleReward(); });
 
        coinHeartBar.Init();
        UseProfile.CurrentLevel += 1;
        if(UseProfile.CurrentLevel >= 84)
        {
            UseProfile.CurrentLevel = 84;
        }    
        UseProfile.WinStreak += 1;
  
        GameController.Instance.musicManager.PlayWinSound();
    }   
    public void InitState()
    {

        GameController.Instance.AnalyticsController.WinLevel(UseProfile.CurrentLevel);

     
    }    
    private void HandleNext()
    {
        GameController.Instance.musicManager.PlayClickSound();
 
     
       
        GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "InterWinBox");
        void Next()
        {
   
            Close();
            Initiate.Fade("GamePlay", Color.black, 2f);

        }
    }
    private void HandleReward()
    {
        GameController.Instance.musicManager.PlayClickSound();
        GameController.Instance.admobAds.ShowVideoReward(
                   actionReward: () =>
                   {
                       Close();
                       //GameController.Instance.admobAds.HandleHideMerec();
                    
                       List<GiftRewardShow> giftRewardShows = new List<GiftRewardShow>();
                       giftRewardShows.Add(new GiftRewardShow() { amount = 1, type = GiftType.Coin });
                       PopupRewardBase.Setup(false).Show(giftRewardShows, delegate {
                           PopupRewardBase.Setup(false).Close();
                           Initiate.Fade("GamePlay", Color.black, 2f);
                       });

                   },
                   actionNotLoadedVideo: () =>
                   {
                       GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_UI
                        (rewardButton.transform,
                        rewardButton.transform.position,
                        "No video at the moment!",
                        Color.white,
                        isSpawnItemPlayer: true
                        );
                   },
                   actionClose: null,
                   ActionWatchVideo.WinBox_Claim_Coin,
                   UseProfile.CurrentLevel.ToString());
    }
    private void OnDestroy()
    {
        
    }
}
