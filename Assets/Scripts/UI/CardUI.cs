using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    public GameObject bagUI;
    public TextMeshProUGUI cardCostText;
    public TextMeshProUGUI cardMagicText;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDesText;

    public bool canDelete;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canDelete)
        {
            return;
        }
        Debug.Log("删除");
        Dic.Instance.player.DeleteCard(cardNameText.text);//通过名字文本删除,如果要多语言需修改
        ShopUI.deleteCount--;
        bagUI.SetActive(false);
    }

    public void Init(CardDataSO so)
    {
        cardImage.sprite = so.cardImage;
        cardNameText.text = so.cardName;
        cardDesText.text = so.cardDes;
        cardMagicText.text = so.cardMagicCost.ToString();
        cardCostText.text = so.cardCost.ToString();
        if (so.isMagic)
        {
            cardCostText.gameObject.SetActive(false);
        }
        else
        {
            cardMagicText.gameObject.SetActive(false);
        }
    }
}
