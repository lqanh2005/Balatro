using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    public int id;
    public Image cardImage;
    public SpriteRenderer cardBackImage;

    public string description;
    public CardAnim cardAnim;

    public void Resign()
    {
        this.RegisterListener(EventID.END_GAME, delegate { SimplePool2.Despawn(this.gameObject); });

    }
    public abstract void Init();

    public abstract void OnActive();
    public void OnDestroy()
    {

        this.RemoveListener(EventID.END_GAME, delegate { SimplePool2.Despawn(this.gameObject); });
    }
}
    