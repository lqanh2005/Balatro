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

    public Button btnPlay;


    public void Init()
    {
        

        btnPlay.onClick.AddListener(delegate { GameController.Instance.musicManager.PlayClickSound(); Initiate.Fade("GamePlay", Color.black, 1.5f); });
       
   
    }
    //private void Update()
    //{

    //       // OnScreenChange();


    //}





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
