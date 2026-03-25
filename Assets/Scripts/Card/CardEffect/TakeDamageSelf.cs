using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "自残", menuName = "SO/卡牌/自残")]
public class TakeDamageSelf : CardEffectSO
{
    public int damageValue;

    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        caster.TakeDamageWishoutSourceRpc(caster, damageValue);
    }
}
