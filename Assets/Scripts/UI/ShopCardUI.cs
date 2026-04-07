using FishNet;
using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour, IPointerClickHandler
{
    public CardDataSO data;
    public Image cardImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI desText;
    public void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (data != null)
        {
            nameText.text = data.cardName;
            coinText.text = "$:" + data.cardCoin.ToString();
            costText.text = data.cardCost.ToString();
            desText.text = data.cardDes;
            cardImage.sprite = data.cardImage;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        var p = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        if (p.coin.Value < data.cardCoin)
        {
            Debug.Log($"Not enough coin+{Dic.Instance.player.coin.Value}/{data.cardCoin}");
            return;
        }
        else
        {
            p.GetCoin(-data.cardCoin);
            p.AddCard(data.cardName);
            gameObject.SetActive(false);
        }
    }
}
