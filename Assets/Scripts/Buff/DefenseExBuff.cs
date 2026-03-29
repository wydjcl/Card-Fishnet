using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "坚韧Buff", menuName = "SO/Buff/坚韧Buff")]
public class DefenseExBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeDefenseExRpc(newBuffstack);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeDefenseExRpc(-stack);
    }
}
