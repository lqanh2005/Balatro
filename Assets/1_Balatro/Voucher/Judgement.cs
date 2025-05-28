using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/Judgement")]
public class Judgement : VoucherEffectBase
{
    int id1, id2;
    public override void ApplyEffect()
    {
        id1 = Random.Range(0, GamePlayController.Instance.playerContain.deckController.deckDict.Count);
        do
        {
            id2 = Random.Range(0, GamePlayController.Instance.playerContain.deckController.deckDict.Count);
        } while (id1 == id2);
        GamePlayController.Instance.playerContain.deckController.AddCardToDeck(null, id2, GamePlayController.Instance.playerContain.deckController.deckDict[id2].level);
        GamePlayController.Instance.playerContain.deckController.ReCard(id1);
    }
}
