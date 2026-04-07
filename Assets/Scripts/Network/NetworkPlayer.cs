using DG.Tweening;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("同步数据区")]
    public readonly SyncVar<int> connID = new SyncVar<int>();
    public readonly SyncVar<int> characterId = new SyncVar<int>();
    public readonly SyncVar<int> coin = new SyncVar<int>();
    public readonly SyncList<string> deck = new SyncList<string>();

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
    public List<string> _deck;
    public int _coin;
    #region 生命周期
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            name = "玩家体";
        }
        name = "玩家" + Owner.ClientId;//方便测试
        Dic.Instance.player = this;
        if (IsServerStarted)
        {
            connID.Value = Owner.ClientId;
        }
        connID.OnChange += OnConnIDChanged;
        characterId.OnChange += OnCharacterIdChanged;
        deck.OnChange += OnDeckChanged;
        coin.OnChange += Coin_OnChange;
    }
    private void OnDestroy()
    {
        deck.OnChange -= OnDeckChanged;
        connID.OnChange -= OnConnIDChanged;
        characterId.OnChange -= OnCharacterIdChanged;
    }
    #endregion

    [ContextMenu("改变角色id为0")]
    [Client]
    public void ChangeCharacterIdTo0()
    {
        SelectCharacterServerRpc(0);
    }
    [ContextMenu("改变角色id为1")]
    [Client]
    public void ChangeCharacterIdTo1()
    {
        SelectCharacterServerRpc(1);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SelectCharacterServerRpc(int id)
    {
        characterId.Value = id;
    }
    #region 卡牌相关

    /// <summary>
    /// 客户端调用,服务端执行初始化该卡组
    /// </summary>
    [ServerRpc]
    public void InitDeck()
    {
        if (characterId.Value == 0)
        {
            //deck.Add("给你一拳");
            //deck.Add("基础防御术式");
            //deck.Add("发现宝箱");
            //deck.Add("发现宝箱");
            //deck.Add("发现宝箱");
            //deck.Add("发现宝箱");
            //deck.Add("还不能放弃");
            //deck.Add("狂热信仰");
            //deck.Add("信仰一击");
            //deck.Add("临阵磨剑");
            //deck.Add("我身为盾");
            //deck.Add("荆棘盾");
            //deck.Add("旋风斩");
            //deck.Add("星光信仰");
            //deck.Add("战争怒吼");
            //deck.Add("奇怪的药剂");
            //deck.Add("战斗姿态");
            //deck.Add("保护");

            deck.Add("旋风斩");
            deck.Add("旋风斩");
            deck.Add("旋风斩");

        }
        if (characterId.Value == 1)
        {
            deck.Add("狂热信仰");
            deck.Add("狂热信仰");
            deck.Add("发现宝箱");
            deck.Add("发现宝箱");
            deck.Add("基础防御术式");
            deck.Add("基础防御术式");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddCard(string cardName)
    {
        deck.Add(cardName);
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
        foreach (var cardN in deck)
        {
            var cardP = Instantiate(cardPrefab, cardLayout.transform);
            cardP.SetActive(false);
            Card card = cardP.GetComponent<Card>();
            var so = Dic.Instance.FindCard(cardN);
            card.InitCard(so);
            drawDeck.Add(card);
        }
        ShuffleDrawDeck();
    }
    /// <summary>
    /// 为该连接玩家增加单张卡片,i为0的时候弃牌堆,i为1的时候抽牌堆,i为2的时候除外堆
    /// </summary>
    /// <param name="conn"></param>
    [TargetRpc]
    public void CreateOneCard(NetworkConnection conn, string cardName, int i)
    {
        Debug.Log("只有这个玩家执行了方法！");
        // 这里写客户端逻辑，比如播放动画、显示 UI
        if (i == 0)
        {
            Debug.Log("在弃牌堆插入");
            var cardP = Instantiate(cardPrefab, cardLayout.transform);
            cardP.SetActive(false);
            Card card = cardP.GetComponent<Card>();
            var so = Dic.Instance.FindCard(cardName);
            card.InitCard(so);
            discardDeck.Add(card);
        }
        if (i == 1)
        {
            Debug.Log("在抽牌堆插入");
        }
        if (i == 2)
        {
            Debug.Log("在除外堆插入");
        }
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
                    //Debug.Log("抽牌堆和弃牌堆都空了，无法继续抽牌");
                    break;
                }
                //Debug.Log("抽牌堆为空,洗牌");
                // 洗回抽牌堆
                ShuffleDiscardIntoDraw();
            }
            // 洗牌后仍然空 → 安全退出
            if (drawDeck.Count == 0)
            {
                //Debug.Log("洗牌后抽牌堆仍为空");
                break;
            }
            // 现在抽牌堆一定有牌，抽一张
            Card card = drawDeck[0];
            drawDeck.RemoveAt(0);
            handDeck.Add(card);
            card.gameObject.SetActive(true);
            card.transform.position = new Vector3(0, 0, 0);
            card.isAni = true;
            //var delay = i * 0.1f;
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

    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void GetCoin(int i)
    {
        coin.Value += i;
    }
    [ContextMenu("增加10金币")]
    public void test()
    {
        GetCoin(10);
    }


    private void OnConnIDChanged(int prev, int next, bool asServer)
    {
        _connID = connID.Value;
    }
    private void OnCharacterIdChanged(int prev, int next, bool asServer)
    {
        _characterId = characterId.Value;
        //NetworkLobbyNode lobby = FindAnyObjectByType<NetworkLobbyNode>(FindObjectsInactive.Include);
        //if (lobby != null)
        //{
        //    Debug.Log("找到了Lobby");
        //}
    }
    private void OnDeckChanged(SyncListOperation op, int index, string oldItem, string newItem, bool asServer)
    {
        _deck.Clear();
        _deck.AddRange(deck);
    }

    private void Coin_OnChange(int prev, int next, bool asServer)
    {
        _coin = coin.Value;
        if (IsOwner)
        {
            TextMeshProUGUI t = GameObject.Find("CoinText")?.GetComponent<TextMeshProUGUI>();
            if (t != null)
            {
                t.text = $"金币:{coin.Value}";
            }
            else
            {
                Debug.Log("没有找到Text");
            }
        }

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


}
