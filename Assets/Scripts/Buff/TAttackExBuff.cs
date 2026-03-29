using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "临时力量Buff", menuName = "SO/Buff/临时力量Buff")]
public class TAttackExBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeAttackExRpc(newBuffstack);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        caster.TakeAttackExRpc(-stack);
    }
}
