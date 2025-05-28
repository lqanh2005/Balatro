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
    public Button btnClose;
    public Button btnAdsRevive;
    public Button btnReviveByCoin;


    public void Init()
    {
        btnClose.onClick.AddListener(delegate { HandleClose(); });
        btnAdsRevive.onClick.AddListener(delegate { HandleAdsRevive(); });
        btnReviveByCoin.onClick.AddListener(delegate { HandleReviveByCoin(); });
   
       
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
