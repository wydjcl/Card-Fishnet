using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "永冻Buff", menuName = "SO/Buff/永冻Buff")]
public class FrostFoeverBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeFrostForeverRpc(true);
    }

    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeFrostForeverRpc(false);
    }
}
