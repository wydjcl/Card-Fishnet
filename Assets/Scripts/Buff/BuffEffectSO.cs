using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffEffectSO : ScriptableObject
{
    public string buffName;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="newBuffstack">新增添buff层数,例如给予荆棘3层,就用这个量,每次都是三层</param>
    /// <param name="totalstack">总层数,例如给予buff的时候收到buff层数的伤害,就用这个</param>
    public virtual void ApplyEffect(Character caster, int newBuffstack, int totalstack)
    {

    }
    public virtual void RemoveEffect(Character caster, int stack)
    {

    }
    public virtual void TurnStartEffect(Character caster, int stack)
    {

    }
    public virtual void TurnEndEffect(Character caster, int stack)
    {

    }
}
