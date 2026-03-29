using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "信仰效果翻倍", menuName = "SO/卡牌/信仰效果翻倍")]
public class FaithDouble : CardEffectSO
{
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        caster.faithDouble.Value = true;
    }
}
