using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "寒霜攻击", menuName = "SO/卡牌/寒霜攻击")]
public class FrostAttack : CardEffectSO
{
    public int damageValue = 6;
    public bool byAttack = true;
    public float damagePercent = 1f;
    public float frostPercent = 1.5f;
    public bool isAOE = false;

    public bool giveFrost = true;
    public int frostStack = 0;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        var _frostPercent = frostPercent;

        Buff b = new Buff();
        b.buffStack = frostStack;
        b.buffName = "寒霜";
        b.forever = true;
        if (byAttack)
        {
            if (isAOE)
            {
                BattleManager.Instance.AoeAttack(caster, BattleManager.Instance.enemies, Mathf.CeilToInt((caster.attack.Value + caster.attackEx.Value) * damagePercent), true, _frostPercent);
            }
            else
            {
                if (target.frost.Value <= 0)
                {
                    _frostPercent = 1f;
                }
                caster.CauseDamageRpc(target, Mathf.CeilToInt((caster.attack.Value + caster.attackEx.Value) * damagePercent * _frostPercent));
            }
        }
        else
        {
            caster.CauseDamageRpc(target, damageValue);
        }

    }
}
