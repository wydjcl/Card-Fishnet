using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "寒霜Buff", menuName = "SO/Buff/寒霜Buff")]
public class FrostBuff : BuffEffectSO
{
    public override void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {
        caster.TakeFrostRpc(newBuffstack);
    }
    public override void RemoveEffect(Character caster, int stack)
    {
        Debug.Log("移除" + stack + ")");
        caster.TakeFrostRpc(-stack);
        Debug.Log("角色剩余寒霜" + caster.frost.Value);
    }
}
