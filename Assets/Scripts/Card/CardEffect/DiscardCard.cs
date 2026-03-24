using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "弃置此牌", menuName = "SO/卡牌/卡牌效果/弃置此牌")]
public class DiscardCard : CardEffectSO
{
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        player.DiscardCard(card);
    }
}
