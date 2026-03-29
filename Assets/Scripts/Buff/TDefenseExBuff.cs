using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "临时坚韧Buff", menuName = "SO/Buff/临时坚韧Buff")]
public class TDefenseExBuff : BuffEffectSO
{
    // Start is called before the first frame update
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeDefenseExRpc(newBuffstack);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeDefenseExRpc(-stack);
    }
}
