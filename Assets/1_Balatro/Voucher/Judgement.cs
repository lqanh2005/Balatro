using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "VoucherEffect/Judgement")]
public class Judgement : VoucherEffectBase
{
    (int id, int level) key1, key2;

    public override void ApplyEffect()
    {
        var deckDict = GamePlayController.Instance.playerContain.deckController.deckDict;
        var keys = deckDict.Keys.ToList();

        if (keys.Count < 2)
        {
            Debug.LogWarning("Không đủ 2 thẻ trong deck để áp dụng hiệu ứng Judgement.");
            return;
        }

        int index1 = Random.Range(0, keys.Count);

        int index2;
        do
        {
            index2 = Random.Range(0, keys.Count);
        } while (index2 == index1);

        key1 = keys[index1];
        key2 = keys[index2];

        var deckController = GamePlayController.Instance.playerContain.deckController;
        deckController.AddCardToDeck(null, key2.id, key2.level);
        deckController.ReCard(key1.id, key1.level);
    }
}
