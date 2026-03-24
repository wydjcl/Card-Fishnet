using DG.Tweening;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("同步数据区")]
    public readonly SyncVar<int> connID = new SyncVar<int>();
    public readonly SyncVar<int> characterId = new SyncVar<int>();
    public readonly SyncList<int> deck = new SyncList<int>();
    [Header("需要导入的实例")]
    public GameObject cardPrefab;
    public CardLayout cardLayout;
    [Header("非同步数据区")]
    public Player myPlayer;
    public List<Card> drawDeck = new List<Card>();//抽牌堆
    public List<Card> handDeck = new List<Card>();//手牌
    public List<Card> discardDeck = new List<Card>();//弃牌堆
    public List<Card> removeDeck = new List<Card>();//除外牌堆

    public List<Character> characterList = new List<Character>();

    [Header("Debug区域")]
    public int _characterId;
    public int _connID;
    public List<int> _deck;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            name = "玩家体";
        }
        name = "玩家" + Owner.ClientId;//方便测试
        if (IsServerStarted)
        {
            connID.Value = Owner.ClientId;
        }
        connID.OnChange += OnConnIDChanged;
        characterId.OnChange += OnCharacterIdChanged;
        deck.OnChange += OnDeckChanged;

    }



    [ContextMenu("改变角色id为1")]
    public void ChangeCharacterId()
    {
        SelectCharacterServerRpc(1);
    }
    [ServerRpc]
    public void SelectCharacterServerRpc(int id)
    {
        characterId.Value = id;
    }
    /// <summary>
    /// 客户端调用,服务端执行初始化该卡组
    /// </summary>
    [ServerRpc]
    public void InitDeck()
    {
        if (characterId.Value == 0)
        {
            deck.Add(0);
            deck.Add(1);
            deck.Add(2);
            deck.Add(0);
            deck.Add(1);
            deck.Add(2);
        }
        if (characterId.Value == 1)
        {
            deck.Add(1);
            deck.Add(1);
        }
    }
    [ContextMenu("创建卡牌")]
    [Client]
    public void CreateCard()
    {
        for (int i = cardLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardLayout.transform.GetChild(i).gameObject);
        }
        handDeck.Clear();
        discardDeck.Clear();
        drawDeck.Clear();
        removeDeck.Clear();
        foreach (var cardID in deck)
        {
            var cardP = Instantiate(cardPrefab, cardLayout.transform);
            cardP.SetActive(false);
            Card card = cardP.GetComponent<Card>();
            var so = Dic.Instance.FindCard(cardID);
            card.InitCard(so);
            drawDeck.Add(card);
        }
        ShuffleDrawDeck();
    }

    public void DestroyCard()
    {
        for (int i = cardLayout.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardLayout.transform.GetChild(i).gameObject);
        }
        handDeck.Clear();
        discardDeck.Clear();
        drawDeck.Clear();
        removeDeck.Clear();
    }
    [ContextMenu("抽5牌")]
    public void DrawFive()
    {
        DrawCard(5);
    }
    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawDeck.Count == 0)
            {
                // 弃牌堆也空 → 没牌可抽，直接停止
                if (discardDeck.Count == 0)
                {
                    Debug.Log("抽牌堆和弃牌堆都空了，无法继续抽牌");
                    return;
                }
                Debug.Log("抽牌堆为空,洗牌");
                // 洗回抽牌堆
                ShuffleDiscardIntoDraw();
            }
            // 洗牌后仍然空 → 安全退出
            if (drawDeck.Count == 0)
            {
                Debug.Log("洗牌后抽牌堆仍为空");
                return;
            }
            // 现在抽牌堆一定有牌，抽一张
            Card card = drawDeck[0];
            drawDeck.RemoveAt(0);
            handDeck.Add(card);
            card.gameObject.SetActive(true);
            card.transform.position = new Vector3(0, 0, 0);
            card.isAni = true;
            var delay = i * 0.1f;
            // SetCardLayout(delay);
        }
        SetCardLayout(0);
    }
    private void SetCardLayout(float delay)
    {
        for (int i = 0; i < handDeck.Count; i++)
        {
            var currentCard = handDeck[i];
            currentCard.transform.DOKill();
        }//删去所有卡牌的动画
        for (int i = 0; i < handDeck.Count; i++)
        {

            var currentCard = handDeck[i];
            CardTransForm cardTransForm = cardLayout.GetCardTransForm(i, handDeck.Count);
            currentCard.transform.DOScale(Vector3.one, 0.05f).SetDelay(delay).onComplete = () =>
            {
                currentCard.transform.DOMove(cardTransForm.pos, 0.1f).onComplete = () =>
                {
                    currentCard.isAni = false;
                };
            };
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            currentCard.orSortingOrder = i;
            currentCard.UpdatePosRot(cardTransForm.pos, cardTransForm.rotation);
        }
    }
    public void ShuffleDiscardIntoDraw()
    {
        drawDeck.AddRange(discardDeck);
        discardDeck.Clear();
        ShuffleDrawDeck();
    }
    /// <summary>
    /// 打乱抽牌堆的顺序（Fisher–Yates 洗牌）
    /// </summary>
    public void ShuffleDrawDeck()
    {
        if (drawDeck.Count <= 1)
            return;
        for (int i = 0; i < drawDeck.Count; i++)
        {
            int rand = Random.Range(i, drawDeck.Count);
            (drawDeck[i], drawDeck[rand]) = (drawDeck[rand], drawDeck[i]);
        }
    }
    public void DiscardCard(Card card)
    {
        discardDeck.Add(card);
        handDeck.Remove(card);
        card.gameObject.SetActive(false);
        SetCardLayout(0f);
    }

    public void RemoveCard(Card card)
    {
        removeDeck.Add(card);
        handDeck.Remove(card);
        card.gameObject.SetActive(false);
        SetCardLayout(0f);
    }

    public void DiscardAllCards()
    {
        // 倒序遍历手牌堆
        for (int i = handDeck.Count - 1; i >= 0; i--)
        {
            Card card = handDeck[i];

            // 移动到弃牌堆
            discardDeck.Add(card);

            // 从手牌堆移除
            handDeck.RemoveAt(i);

            // 隐藏卡牌
            card.gameObject.SetActive(false);
        }

        // 更新手牌布局
        SetCardLayout(0f);
    }





    private void OnConnIDChanged(int prev, int next, bool asServer)
    {
        _connID = connID.Value;
    }
    private void OnCharacterIdChanged(int prev, int next, bool asServer)
    {
        _characterId = characterId.Value;
    }
    private void OnDeckChanged(SyncListOperation op, int index, int oldItem, int newItem, bool asServer)
    {
        _deck.Clear();
        _deck.AddRange(deck);
    }
    [ContextMenu("Debug")]
    public void DebugAllData()
    {
        if (IsOwner)
        {
            Debug.Log("我是该客户端玩家体");
        }
        _characterId = characterId.Value;
    }
    private void OnDestroy()
    {
        deck.OnChange -= OnDeckChanged;
        connID.OnChange -= OnConnIDChanged;
        characterId.OnChange -= OnCharacterIdChanged;
    }
}
