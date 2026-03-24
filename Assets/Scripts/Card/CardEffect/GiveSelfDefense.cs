using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "给予自己护盾", menuName = "SO/卡牌/卡牌效果/给予自己护盾")]
public class GiveSelfDefense : CardEffectSO
{
    public int defenseValue = 5;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        caster.TakeDefenseRpc(defenseValue);
    }
}
