using Crystal;
using DG.Tweening;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateGame
{
    Loading = 0,
    Playing = 1,
    Win = 2,
    Lose = 3,
    Pause = 4
}

public class GamePlayController : Singleton<GamePlayController>
{
    public StateGame stateGame;
    public PlayerContain playerContain;
    public GameScene gameScene;
    public UIController uICtrl;
 
 
    
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

        this.PostEvent(EventID.ON_SELECT_ROUND);
    }
   
}
