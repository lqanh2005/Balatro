using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleLayout : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.5f;

    public HorizontalLayoutGroup layoutGroup;
    public CardUI item1;
    public CardUI item2;

    public void Init()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        item1 = transform.GetChild(0).GetComponent<CardUI>();
        item2 = transform.GetChild(1).GetComponent<CardUI>();
        item1.Init();
        item2.Init();
    }

}
