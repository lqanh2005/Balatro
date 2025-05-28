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
    public Text tvCoin;
    public Text tvCoin_2;
    public CanvasGroup canvasGroup;
    public void Init()
    {
        nextButton.onClick.AddListener(delegate { HandleNext();    });
        rewardButton.onClick.AddListener(delegate { HandleReward(); });
 
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



     
    }    
    private void HandleNext()
    {
        GameController.Instance.musicManager.PlayClickSound();
 
     
       
        
        void Next()
        {
   
            Close();
            Initiate.Fade("GamePlay", Color.black, 2f);

        }
    }
    private void HandleReward()
    {
        GameController.Instance.musicManager.PlayClickSound();
       
    }
    private void OnDestroy()
    {
        
    }
}
