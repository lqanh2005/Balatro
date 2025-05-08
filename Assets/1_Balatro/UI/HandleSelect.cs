using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSelect : MonoBehaviour
{
    public List<RoundBase> rounds = new List<RoundBase>();

    public void Init()
    {
        RectTransform rect = GetComponent<RectTransform>();
        DOTween.To(
           () => rect.offsetMin,
           x => rect.offsetMin = new Vector2(x.x, x.y),
           new Vector2(rect.offsetMin.x, 0),
           0.5f
       );
        DOTween.To(
            () => rect.offsetMax,
            x => rect.offsetMax = new Vector2(x.x, x.y),
            new Vector2(rect.offsetMax.x, 0),
            0.5f
        );
        foreach (var round in rounds)
        {
            round.Init();
        }
    }
}
