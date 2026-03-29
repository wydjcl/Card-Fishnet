using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "群体攻击", menuName = "SO/卡牌/群体攻击")]
public class AOEAttack : CardEffectSO
{
    public int damageValue = 6;
    public bool byAttack = false;
    public float damagePercent = 1f;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        foreach (var c in BattleManager.Instance.enemies)
        {
            if (byAttack)
            {
                caster.CauseDamageRpc(c, Mathf.CeilToInt((caster.attack.Value + caster.attackEx.Value) * damagePercent));
            }
            else
            {
                caster.CauseDamageRpc(c, damageValue);
            }
        }


    }
}
