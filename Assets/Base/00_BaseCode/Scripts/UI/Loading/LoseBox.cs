using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoseBox : BaseBox
{
    public static LoseBox _instance;
    public static LoseBox Setup()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<LoseBox>(PathPrefabs.LOSE_BOX));
            _instance.Init();
        }
        _instance.InitState();
        return _instance;
    }
    public Text tvScore;
    public Button newGame;


    public void Init()
    {
        newGame.onClick.AddListener(delegate { NewGame(); });


    }
    public void NewGame()
    {
        GameController.Instance.musicManager.PlayClickSound();
        UseProfile.CurrentAnte = 1;
        UseProfile.CurrentRound = 0;
        UseProfile.CurrentGold = 5;
        UseProfile.NeedCheckShop = false;
        UseProfile.IsNewGame = true;
        UseProfile.SavedState = StateGame.SelectRound;
        Initiate.Fade("GamePlay", Color.black, 1.5f);
    }
    public void InitState()
    {
        
    }
    public void HandleReviveByCoin()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (UseProfile.Coin >= 100)
        {
            UseProfile.Coin -= 100;
            GamePlayController.Instance.stateGame = StateGame.Playing;
    
         
 
            Close();
        
        }
        else
        {
     
        }


    }
    public void HandleAdsRevive()
    {
        GameController.Instance.musicManager.PlayClickSound();
       



    }
    public void HandleClose()
    {
        GameController.Instance.musicManager.PlayClickSound();

    }

}
