using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "普通攻击", menuName = "SO/卡牌/普通攻击")]
public class NormalAttack : CardEffectSO
{
    public int damageValue = 6;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        target.TakeDamageRpc(damageValue);
    }
}
