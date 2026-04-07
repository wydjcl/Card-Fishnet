using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardData", menuName = "SO/卡牌Data")]
public class CardDataSO : ScriptableObject
{
    public int cardID;
    public string cardName;
    public int cardCost;
    public int cardCoin;
    public CardType cardType;
    public CardQuality cardQuality;
    public Sprite cardImage;
    [TextArea]
    public string cardDes;

    [TextArea]
    public string cardUIDes;
    public List<CardEffectSO> effects;
}
