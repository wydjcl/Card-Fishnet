using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌效果的抽象基类
/// </summary>
public abstract class CardEffectSO : ScriptableObject
{
    public abstract void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player);
}