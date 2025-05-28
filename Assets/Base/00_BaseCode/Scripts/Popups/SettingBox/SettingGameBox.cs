using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Sirenix.OdinInspector;
using EventDispatcher;
public class SettingGameBox : BaseBox
{
    #region instance
    public static SettingGameBox instance;
    public static SettingGameBox Setup(bool isSaveBox = false, Action actionOpenBoxSave = null)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<SettingGameBox>(PathPrefabs.SETTING_GAME_BOX));
            instance.Init();
        }

        instance.InitState();
        return instance;
    }
    #endregion
    #region Var

    [SerializeField] private Button btnClose;
  

    [SerializeField] private Button btnVibration;
    [SerializeField] private Button btnMusic;
    [SerializeField] private Button btnSound;

    public RectTransform objMusic;
    public RectTransform objVibra;
    public RectTransform objSound;
    public Image imgMusic;
    public Image imgVibration;
    public Image imgSound;

    public Sprite spriteOn;
    public Sprite spriteOff;

 
    public Button btnHome;
    public Button btnRestart;

    public bool isGameplay;

    public Vector3 postOn = new Vector3(90, -36, 0);
    public Vector3 postOff = new Vector3(35, -36, 0);

    #endregion
    private void Init()
    {
        btnClose.onClick.AddListener(delegate { OnClickButtonClose(); }); 
        btnVibration.onClick.AddListener(delegate { OnClickBtnVibration(); });
        btnMusic.onClick.AddListener(delegate { OnClickBtnMusic(); });
        btnSound.onClick.AddListener(delegate { OnClickBtnSound(); });
        
  
        btnHome.onClick.AddListener(delegate { HandleBtnHome(); });
        btnRestart.onClick.AddListener(delegate { HandleBtnRestart(); });
       
  
    }
  
    private void InitState()
    {
        SetUpBtn();
    }

    public void OffBtn()
    {
        btnHome.gameObject.SetActive(false);
        btnRestart.gameObject.SetActive(false);
    }
    private void SetUpBtn()
    {
        SetToggle(imgVibration, objVibra, GameController.Instance.useProfile.OnVibration);
        SetToggle(imgMusic, objMusic, GameController.Instance.useProfile.OnMusic);
        SetToggle(imgSound, objSound, GameController.Instance.useProfile.OnSound);
    }

    private void SetToggle(Image img, RectTransform obj, bool isOn)
    {
        img.sprite = isOn ? spriteOn : spriteOff;
        obj.anchoredPosition = isOn ? postOn : postOff;
    }


    private void OnClickBtnVibration()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnVibration)
        {
            GameController.Instance.useProfile.OnVibration = false;
        }
        else
        {
            GameController.Instance.useProfile.OnVibration = true;
        }
        SetUpBtn();
    }

    private void OnClickBtnMusic()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnMusic)
        {
            GameController.Instance.useProfile.OnMusic = false;
        }
        else
        {
            GameController.Instance.useProfile.OnMusic = true;
        }
        SetUpBtn();
    }
    private void OnClickBtnSound()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnSound)
        {
            GameController.Instance.useProfile.OnSound = false;
        }
        else
        {
            GameController.Instance.useProfile.OnSound = true;
        }
        SetUpBtn();
    }


    private void OnClickButtonClose()
    {
        GameController.Instance.musicManager.PlayClickSound();
        Close();
    }


    public void HandleBtnHome()
    {
        GameController.Instance.musicManager.PlayClickSound();
        Initiate.Fade("HomeScene", Color.black, 1.5f);
    }
    public void HandleBtnRestart()
    {
        GameController.Instance.musicManager.PlayClickSound();
        this.gameObject.SetActive(false);

        this.PostEvent(EventID.END_GAME);
        this.PostEvent(EventID.START_GAME);


    }

}
