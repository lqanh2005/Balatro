using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class HomeScene : BaseScene
{

    public Button newGameBtn;
    public Button continueGameBtn;


    public void Init()
    {
        if (UseProfile.IsNewGame) continueGameBtn.gameObject.SetActive(false);
        else
        {
            continueGameBtn.gameObject.SetActive(true);
            continueGameBtn.onClick.AddListener(delegate { ContinueGame(); });
        }
        newGameBtn.onClick.AddListener(delegate { NewGame(); });
        //RecipeManager.LoadAll();
    }

    private void ContinueGame()
    {
        GameController.Instance.musicManager.PlayClickSound(); 
        Initiate.Fade("GamePlay", Color.black, 1.5f);
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





    public override void OnEscapeWhenStackBoxEmpty()
    {
        //Hiển thị popup bạn có muốn thoát game ko?
    }
    private void OnSettingClick()
    {
        SettingGameBox.Setup(false).Show();
        //MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }

    


}
