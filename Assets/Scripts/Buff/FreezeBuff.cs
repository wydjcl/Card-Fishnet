using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "冻结Buff", menuName = "SO/Buff/冻结Buff")]
public class FreezeBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeFreezeRpc(true);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeFreezeRpc(false);
    }
}
