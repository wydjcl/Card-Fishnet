using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "嘲讽Buff", menuName = "SO/Buff/嘲讽Buff")]
public class TauntBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeTauntRpc(true);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeTauntRpc(false);
    }
}
