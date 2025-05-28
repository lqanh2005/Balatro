using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/TheWorld")]
public class TheWorld : VoucherEffectBase
{
    private int id;
    public override void ApplyEffect()
    {
        id = Random.Range(0, GamePlayController.Instance.playerContain.deckController.deckDict.Count);
        GamePlayController.Instance.playerContain.deckController.AddCardToDeck(null, id, GamePlayController.Instance.playerContain.deckController.deckDict[id].level);
    }
}
