using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveThorn : CardEffectSO
{
    public bool self;
    public int thornValue;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        Buff buff = new Buff();
        buff.buffName = "荆棘";
        if (self)
        {
            caster.TakeThornRpc(thornValue);
        }
        else
        {
            target.TakeThornRpc(thornValue);
        }
    }

}
