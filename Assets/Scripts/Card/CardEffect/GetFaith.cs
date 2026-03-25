using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "给予信仰", menuName = "SO/卡牌/卡牌效果/给予信仰")]
public class GetFaith : CardEffectSO
{
    public bool self;
    public int faithValue = 5;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        if (self)
        {
            var p = caster as Player;
            p.TakeFaithRpc(faithValue);
        }
        else
        {
            var p = target as Player;
            p.TakeFaithRpc(faithValue);
        }
    }
}
