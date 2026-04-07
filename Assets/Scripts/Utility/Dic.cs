using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Dic : SingletonMono<Dic>
{
    [HideInInspector]
    public NetworkPlayer player;
    [Header("卡牌数据字典")]
    public List<CardDataSO> cardDataSOs = new List<CardDataSO>();
    [Header("敌人字典")]
    public List<NetworkObject> enemies = new List<NetworkObject>();
    [Header("Buff字典")]
    public List<BuffEffectSO> buffs = new List<BuffEffectSO>();
    [Header("女骑士卡牌数据字典")]
    public List<CardDataSO> P0CardDataSOs = new List<CardDataSO>();
    public CardDataSO FindCard(string i)
    {
        foreach (CardDataSO s in cardDataSOs)
        {
            if (s.cardName == i)
            {
                return s;
            }
        }
        Debug.LogWarning("没找到卡牌:" + i);
        return null;
    }
    /// <summary>
    /// 根据角色id和数字获取卡片 ,TODO:角色id暂时没用，后续根据角色id区分不同角色的卡牌,区分稀有度
    /// </summary>
    /// <param name="characterID"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    public CardDataSO GetCard(int characterID, int i)
    {
        if (characterID == 0)
        {
            return cardDataSOs[i];
        }
        return null;
    }
    public int GetCardsCount(int i)
    {
        if (i == 0)
        {
            return P0CardDataSOs.Count;
        }

        return 0;
    }
    public List<CardDataSO> GetCardsRandom(int characterID, int num)
    {
        List<CardDataSO> temp;
        if (characterID == 0)
        {
            temp = new List<CardDataSO>(P0CardDataSOs);
        }
        else
        {
            temp = new List<CardDataSO>(P0CardDataSOs);
        }

        if (temp.Count <= num)
            return new List<CardDataSO>(P0CardDataSOs);


        System.Random rng = new System.Random();

        for (int i = 0; i < num; i++)
        {
            int r = rng.Next(i, temp.Count);
            (temp[i], temp[r]) = (temp[r], temp[i]);
        }

        return temp.GetRange(0, num);
    }
}
