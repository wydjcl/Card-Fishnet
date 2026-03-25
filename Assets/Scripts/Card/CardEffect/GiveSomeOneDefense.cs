using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "给予友军护盾", menuName = "SO/卡牌/给予友军护盾")]
public class GiveSomeOneDefense : CardEffectSO
{
    public int blockValue = 5;
    public override void ApplyEffect(Character caster, Character target, Card card, NetworkPlayer player)
    {
        target.TakeBlockRpc(blockValue);
    }
}
