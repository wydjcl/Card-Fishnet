using DG.Tweening;
using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [Header("UI层")]
    public GameObject Entry;
    public SpriteRenderer cardSprite;
    public TextMeshPro cardNameText;
    public TextMeshPro cardCostText;
    public TextMeshPro cardDesText;
    [Header("数据层")]
    public bool isAni = true;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public int orSortingOrder;
    public int originaLayerOrder;//原始叠层排序

    public NetworkPlayer player;
    public int cardID;
    public string cardName;
    public int cardCost;
    public CardType cardType;
    public List<CardEffectSO> cardEffectSOs;
    public void InitCard(CardDataSO so)
    {
        cardName = so.cardName;
        cardID = so.cardID;
        cardCost = so.cardCost;
        cardType = so.cardType;
        cardEffectSOs = so.effects;

        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        cardSprite.sprite = so.cardImage;
        cardNameText.text = so.cardName;
        cardCostText.text = so.cardCost.ToString();
        cardDesText.text = so.cardDes;

    }

    public void UpdatePosRot(Vector3 pos, Quaternion rot)
    {
        originalPosition = pos;
        originalRotation = rot;
        originaLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }
    public void UseCard(Character caster, Character target)
    {
        if (player.myPlayer.mana.Value < cardCost)
        {
            Debug.Log("法力值不够");
            return;
        }
        else
        {
            player.myPlayer.ConsumeManaRpc(cardCost);
        }
        foreach (CardEffectSO so in cardEffectSOs)
        {
            so.ApplyEffect(caster, target, this, player);
        }
    }


    private void OnDestroy()
    {
        transform.DOKill();
        Entry.transform.DOKill();
    }

}
