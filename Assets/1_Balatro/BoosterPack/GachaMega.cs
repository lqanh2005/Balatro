using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaMega : MonoBehaviour
{
    public List<RecipCardUI> recipCardUIs;

    public void Init()
    {
        this.gameObject.SetActive(true);
        foreach (var card in recipCardUIs)
        {
            card.Init();
        }
    }
}
