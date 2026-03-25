using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "给予Buff", menuName = "SO/卡牌/给予Buff")]
public class GiveBuff : CardEffectSO
{
    public bool self;
    public string _buffName;
    public int _buffStack;
    public bool _forever;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        Buff buff = new Buff();
        buff.buffName = _buffName;
        buff.buffStack = _buffStack;
        buff.forever = _forever;
        if (self)
        {
            caster.AddBuffRpc(buff);
        }
        else
        {
            target.AddBuffRpc(buff);
        }
    }
}
