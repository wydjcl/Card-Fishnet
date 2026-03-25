using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BagUI : MonoBehaviour
{
    public GameObject cardUI;
    public GameObject content;
    public NetworkPlayer player;
    private void OnEnable()
    {
        Debug.Log("打开背包UI");
        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();

        CreateCard();
    }
    [ContextMenu("创建卡牌")]
    [Client]
    public void CreateCard()
    {
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        foreach (var cardID in player.deck)
        {
            var cardP = Instantiate(cardUI, content.transform);
            //cardP.SetActive(false);
            CardUI card = cardP.GetComponent<CardUI>();

            var so = Dic.Instance.FindCard(cardID);
            card.cardCostText.text = so.cardCost.ToString();
            card.cardDesText.text = so.cardDes.ToString();
            card.cardNameText.text = so.cardName.ToString();
            card.cardImage.sprite = so.cardImage;
            //card.InitCard(so);
            //drawDeck.Add(card);
        }
    }
}
