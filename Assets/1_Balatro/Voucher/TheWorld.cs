using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/TheWorld")]
public class TheWorld : VoucherEffectBase
{
    public override void ApplyEffect()
    {
        var deckController = GamePlayController.Instance.playerContain.deckController;
        var allKeys = deckController.deckDict.Keys.ToList();

        if (allKeys.Count == 0) return;

        var randomKey = allKeys[Random.Range(0, allKeys.Count)];
        int id = randomKey.id;
        int level = randomKey.level;

        deckController.AddCardToDeck(null, id, level);
    }
}
