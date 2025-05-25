using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher;

public class HandleSelect : MonoBehaviour
{
    public List<RoundBase> rounds = new List<RoundBase>();
    [HideInInspector] public int currentRound;

    public void Init()
    {
        this.RegisterListener(EventID.ON_SELECT_ROUND, delegate { OnSelected(); });
        
    }
    public void OnSelected()
    {
        this.gameObject.SetActive(true);
        currentRound = UseProfile.CurrentRound;
        rounds[currentRound].isActive=true;
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
    public void OnDestroy()
    {
        this.RemoveListener(EventID.ON_SELECT_ROUND, delegate { OnSelected(); });
    }
}
