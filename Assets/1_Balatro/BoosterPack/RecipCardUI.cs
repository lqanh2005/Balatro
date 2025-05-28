using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipCardUI : MonoBehaviour
{

    public Image avt;
    public Button select;
    private int index;
    public void Init()
    {
        index = Random.Range(0, RecipeCard.Instance.dataRecipe.Count - 1);
        avt.sprite = RecipeCard.Instance.dataRecipe[index].avt;
        select.onClick.RemoveAllListeners();
        select.onClick.AddListener(() =>
        {
            GameController.Instance.musicManager.PlayClickSound();
            RecipeManager.UpgradeRecipe(RecipeCard.Instance.dataRecipe[index].name);
            GachaBox._instance.Close();
            GamePlayController.Instance.uICtrl.shopCtrl.Show();
        });
    }
}
