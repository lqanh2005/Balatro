using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBase : MonoBehaviour
{
    public int id;
    public SpriteRenderer cardImage;
    public SpriteRenderer cardBackImage;

    public string description;
    public CardAnim cardAnim;

    public abstract void Init();

    public abstract void OnActive();
}
    