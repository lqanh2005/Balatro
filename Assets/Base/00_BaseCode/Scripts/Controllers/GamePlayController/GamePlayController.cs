using Crystal;
using DG.Tweening;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MaxSdkBase;
using static UnityEngine.GraphicsBuffer;


public enum StateGame
{
    Loading = 0,
    Playing = 1,
    Win = 2,
    Lose = 3,
    Pause = 4,
    SelectRound = 5,
    Shopping = 6
}

public class GamePlayController : Singleton<GamePlayController>
{
    public StateGame stateGame;
    public PlayerContain playerContain;
    public GameScene gameScene;
    public UIController uICtrl;
 
    public bool isLevelDone = false;

    protected override void OnAwake()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.currentScene = SceneType.GamePlay;
        }

        uICtrl.Init();
        playerContain.Init();
        
    }

    public void Start()
    {
        Continue();
        isLevelDone = true;
    }

    private void Continue()
    {
        switch (UseProfile.SavedState)
        {
            case StateGame.SelectRound:
                this.PostEvent(EventID.ON_SELECT_ROUND);
                break;

            case StateGame.Playing:
                ContinuePlaying();
                this.PostEvent(EventID.START_GAME);
                uICtrl.initLevelDone = true;
                break;

            case StateGame.Shopping:
                uICtrl.shopCtrl.ContinueShopping();
                this.PostEvent(EventID.ON_SHOPPING);
                break;
        }
    }
    private void ContinuePlaying()
    {
        uICtrl.playCtrl.gameObject.SetActive(true);
        uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].Init();
        uICtrl.topState.avt = uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].avt;
        uICtrl.topState.text1.text = uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].text1;
        uICtrl.topState.text2.text = uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].target.text;
        uICtrl.topState.text3.text = uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].reward.text;
        uICtrl.targetScore = int.Parse(uICtrl.handleSelect.rounds[UseProfile.CurrentRound - 1].target.text);
    }
}
