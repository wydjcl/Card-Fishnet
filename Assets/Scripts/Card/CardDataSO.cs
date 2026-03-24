using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardData", menuName = "SO/卡牌/卡牌数据")]
public class CardDataSO : ScriptableObject
{
    public int cardID;
    public string cardName;
    public int cardCost;
    public CardType cardType;
    public Sprite cardImage;
    [TextArea]
    public string cardDes;
    public List<CardEffectSO> effects;
}
