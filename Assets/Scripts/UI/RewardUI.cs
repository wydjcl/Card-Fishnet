using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RewardUI : MonoBehaviour
{
    public CardUI card0;
    public CardUI card1;
    public CardUI card2;

    private void OnEnable()
    {
        BattleManager.Instance.player.GetCoin(UnityEngine.Random.Range(40, 78));
    }
    public void Init(List<int> list)
    {
        card0.cardImage.sprite = Dic.Instance.P0CardDataSOs[list[0]].cardImage;
        card0.cardNameText.text = Dic.Instance.P0CardDataSOs[list[0]].cardName;
        card0.cardDesText.text = Dic.Instance.P0CardDataSOs[list[0]].cardDes;
        card0.cardCostText.text = Dic.Instance.P0CardDataSOs[list[0]].cardCost.ToString();

        card1.cardImage.sprite = Dic.Instance.P0CardDataSOs[list[1]].cardImage;
        card1.cardNameText.text = Dic.Instance.P0CardDataSOs[list[1]].cardName;
        card1.cardDesText.text = Dic.Instance.P0CardDataSOs[list[1]].cardDes;
        card1.cardCostText.text = Dic.Instance.P0CardDataSOs[list[1]].cardCost.ToString();

        card2.cardImage.sprite = Dic.Instance.P0CardDataSOs[list[2]].cardImage;
        card2.cardNameText.text = Dic.Instance.P0CardDataSOs[list[2]].cardName;
        card2.cardDesText.text = Dic.Instance.P0CardDataSOs[list[2]].cardDes;
        card2.cardCostText.text = Dic.Instance.P0CardDataSOs[list[2]].cardCost.ToString();
    }
    public void Click0()
    {
        InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>().AddCard(card0.cardNameText.text);
        Quit();
    }
    public void Click1()
    {
        InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>().AddCard(card1.cardNameText.text);
        Quit();
    }
    public void Click2()
    {
        InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>().AddCard(card2.cardNameText.text);
        Quit();
    }
    public void Quit()
    {
        this.gameObject.SetActive(false);
    }
}
