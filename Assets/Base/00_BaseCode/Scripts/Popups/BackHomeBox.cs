using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public enum TypeBackHOme
{
    ResetLevel,
    BackHome
}
public class BackHomeBox : BaseBox
{
    public static BackHomeBox instance;
    [SerializeField]private Button btnClose;
    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnStay;
    public Text tvCoin;
    public Text tvTitler;
    public Text tvBtnReset;
    TypeBackHOme typeBackHOme;
    public CoinHeartBar coinHeartBar;
    public List<WinStreakBar> lsProgesst;

    public static BackHomeBox Setup(TypeBackHOme typeParam ,bool isSaveBox = false, Action actionOpenBoxSave = null)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<BackHomeBox>(PathPrefabs.BACK_HOME_BOX));
            instance.Init();
        }
        instance.InitState( typeParam);
        return instance;
    }
    public void Init()
    {
        btnClose.onClick.AddListener(delegate { GameController.Instance.musicManager.PlayClickSound(); Close();  });

        btnStay.onClick.AddListener(delegate {
            GameController.Instance.musicManager.PlayClickSound();
            HandleClose();
        });
        coinHeartBar.Init();
       
        //gameObject.GetComponent<Canvas>().sortingOrder = 21;
    }
    public void HandleClose()
    {
        switch (typeBackHOme)
        {
            case TypeBackHOme.BackHome:
                Close();
                if(GamePlayController.Instance.stateGame == StateGame.Lose)
                {
                    LoseBox.Setup().Show();
                }    
   
                break;
            case TypeBackHOme.ResetLevel:

                Debug.LogError("ResetLevel");
                //    GamePlayController.Instance.playerContain.boomInputController.enabled = true;

                Close();
                break;
        }
    }
    public void InitState(TypeBackHOme typeParam)
    {
        btnHome.onClick.RemoveAllListeners();
        switch (typeParam)
        {
            case TypeBackHOme.BackHome:
                tvTitler.text = "BACK HOME";
                tvBtnReset.text = "HOME";
                btnHome.onClick.AddListener(BackHome);
                break;
            case TypeBackHOme.ResetLevel:
                tvTitler.text = "RESET LEVEL";
                tvBtnReset.text = "RESET";
                btnHome.onClick.AddListener(ResetScene);
                break;
        }
        typeBackHOme = typeParam;
     
        for (int i = 0; i < lsProgesst.Count;i ++)
        {
            if(i <= UseProfile.WinStreak)
            {
                lsProgesst[i].gameObject.SetActive(true);
                lsProgesst[i].Init();
            }
        }
    }
    private void ResetScene()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (UseProfile.Heart > 0)
        {
            GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "ResetSceneAtLoseBox");
            void Next()
            {
           
                UseProfile.WinStreak = 0;
                GameController.Instance.heartGame.HandleCoolDown();
                Close();
                Initiate.Fade("GamePlay", Color.black, 1.5f);

            }  
        }    
        else
        {

            HeartBox.Setup().Show();

        }    
 
    }
    private void BackHome()
    {
        GameController.Instance.musicManager.PlayClickSound();
        GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "BackHomeLoseBox");
        void Next()
        {
         
            GameController.Instance.heartGame.HandleCoolDown();

            Close();
            UseProfile.WinStreak = 0;
            Initiate.Fade("HomeScene", Color.black, 1.5f);
        }
 

    }
 

}
