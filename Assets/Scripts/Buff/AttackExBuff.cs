using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "力量Buff", menuName = "SO/Buff/力量Buff")]
public class AttackExBuff : BuffEffectSO
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
