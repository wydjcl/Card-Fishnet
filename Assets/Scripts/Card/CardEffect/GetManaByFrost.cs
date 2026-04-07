using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "根据冻结人数给法力", menuName = "SO/卡牌/根据冻结人数给法力")]
public class GetManaByFrost : CardEffectSO
{
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        int i = 0;
        foreach (var e in BattleManager.Instance.enemies)
        {
            if (e.frost.Value > 0)
            {
                i++;
            }
        }
        var p = target as Player;
        p.AddManaRpc(i);
    }
}
