using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "普通攻击", menuName = "SO/卡牌/普通攻击")]
public class NormalAttack : CardEffectSO
{
    public int damageValue = 6;
    public bool byAttack = false;
    public float damagePercent = 1f;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        if (byAttack)
        {
            caster.CauseDamageRpc(target, Mathf.CeilToInt((caster.attack.Value + caster.attackEx.Value) * damagePercent));
        }
        else
        {
            caster.CauseDamageRpc(target, damageValue);
        }

    }
}
