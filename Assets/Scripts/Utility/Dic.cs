using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dic : SingletonMono<Dic>
{
    [SerializeField]
    public Dictionary<int, string> dict = new Dictionary<int, string>();
    public List<CardDataSO> cardDataSOs = new List<CardDataSO>();
    public List<NetworkObject> enemies = new List<NetworkObject>();
    public List<BuffEffectSO> buffs = new List<BuffEffectSO>();
    public CardDataSO FindCard(string i)
    {
        foreach (CardDataSO s in cardDataSOs)
        {
            if (s.cardName == i)
            {
                return s;
            }
        }
        return null;
    }
}
