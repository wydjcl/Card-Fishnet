using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "荆棘Buff", menuName = "SO/Buff/荆棘Buff")]
public class ThornBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeThornRpc(newBuffstack);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeThornRpc(-stack);
    }
}
