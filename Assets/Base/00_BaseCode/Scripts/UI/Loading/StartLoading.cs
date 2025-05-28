using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartLoading : MonoBehaviour
{
    public Text txtLoading;
    public Image progressBar;
    private string sceneName;
    public int countSecond;
    Coroutine coroutineLoad;
    public bool wasCoolDown;

    public void Init()
    {
        Debug.LogError("432");
        wasCoolDown = true;
        progressBar.fillAmount = 0f;
        countSecond = 0;
        coroutineLoad = StartCoroutine(LoadAdsToChangeScene());
        StartCoroutine(LoadingText());
    }

    // Use this for initialization
    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1);
        progressBar.fillAmount = 0f;
        string name = SceneName.HOME_SCENE;
        //if (!UseProfile.FirstLoading)
        //{
        //    name = SceneName.GAME_PLAY;
        //}
        //else
        //{
        //    name = SceneName.HOME_SCENE;
        //}
        var _asyncOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
       
        while (!_asyncOperation.isDone)
        {
            progressBar.fillAmount = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
            yield return null;

        
        }
    }

    private IEnumerator LoadAdsToChangeScene()
    {
        yield return new WaitForSeconds(1);
        //countSecond += 1;
        //progressBar.fillAmount = 1 - (1 / (float)countSecond);
        //if (GameController.Instance.admobAds.IsOpenAdsReady)
        //{
        //    wasCoolDown = false;
        //}
        //if (countSecond >= 5)
        //{

        //    wasCoolDown = false;

        //}
        //if (wasCoolDown == true)
        //{
        //    coroutineLoad = StartCoroutine(LoadAdsToChangeScene());
        //}
        //else
        //{
            //if (coroutineLoad != null)
            //{
                StartCoroutine(ChangeScene());
        //        StopCoroutine(coroutineLoad);
        //        coroutineLoad = null;
        //    }
        //}

    }


    IEnumerator LoadingText()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            txtLoading.text = "LOADING .";
            yield return wait;

            txtLoading.text = "LOADING ..";
            yield return wait;

            txtLoading.text = "LOADING ...";
            yield return wait;

        }
    }
}
