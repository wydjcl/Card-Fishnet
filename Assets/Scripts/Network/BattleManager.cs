using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance;
    [Header("UI")]
    public NetworkPlayer player;
    public NetworkMapSceneManager networkMapSceneManager;
    public TextMeshProUGUI turnButtomText;

    public readonly SyncVar<int> agreeCount = new SyncVar<int>();
    public readonly SyncVar<int> totalPlayers = new SyncVar<int>();
    public readonly SyncVar<int> turnCount = new SyncVar<int>();
    public readonly SyncVar<TurnState> turnState = new SyncVar<TurnState>();

    public int _agreeCount;//同意回合结束的人数
    public int _totalPlayers;
    public int _turnCount;
    public TurnState _turnState;

    public bool isAgree;//本地是否同意
    public int readyToBattlePlayerCount;//初始化成功玩家计数
    public List<Player> players = new();
    public List<Enemy> enemies = new();

    // public TurnState turnState;

    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        agreeCount.OnChange += AgreeCount_OnChange;
        turnState.OnChange += TurnState_OnChange;
        Instance = this;
    }



    [Server]
    public void ServerInitBattle()
    {
        totalPlayers.Value = InstanceFinder.ServerManager.Clients.Count;
        turnCount.Value = 0;
        CilentInitBattle();

    }
    [ObserversRpc]
    public void CilentInitBattle()
    {
        player.CreateCard();
        networkMapSceneManager.battleUIRoot.gameObject.SetActive(true);
        //networkMapSceneManager.BattleUICanvas.SetActive(true);
        isAgree = false;
        turnButtomText.text = $"回合结束{agreeCount.Value}/{totalPlayers.Value}";
        ReadyToBattle();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ReadyToBattle()
    {
        readyToBattlePlayerCount++;
        Debug.Log("接受到一个玩家的准备请求" + readyToBattlePlayerCount);
        if (readyToBattlePlayerCount >= totalPlayers.Value)
        {
            Debug.Log("全部玩家加载完成,开始回合开始阶段");
            readyToBattlePlayerCount = 0;

            enemies.Clear();
            players.Clear();
            foreach (var netObj in InstanceFinder.ServerManager.Objects.Spawned.Values)
            {
                if (netObj.TryGetComponent<Player>(out Player player))
                {
                    players.Add(player);
                }
                if (netObj.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemies.Add(enemy);
                }
            }
            EnterState(TurnState.PlayerTurnStart);
        }
    }

    public void ClickTurnEndButtom()
    {
        if (turnState.Value != TurnState.PlayerTurn)
        {
            Debug.Log("不是玩家回合无法点击");
            return;
        }
        isAgree = !isAgree;
        ClickTurnEndButtomRpc(isAgree);
    }

    [ServerRpc(RequireOwnership = false)]

    public void ClickTurnEndButtomRpc(bool b)
    {
        if (b)
        {
            agreeCount.Value++;
            if (agreeCount.Value >= totalPlayers.Value)
            {
                agreeCount.Value = 0;
                NextState();
            }
        }
        else
        {
            agreeCount.Value--;
            if (agreeCount.Value < 0)
            {
                agreeCount.Value = 0;
            }
        }
    }
    #region 玩家回合方法
    [Server]
    private void EnterState(TurnState state)
    {
        Debug.Log("请求进入" + state.ToString());
        turnState.Value = state;
        switch (state)
        {
            case TurnState.PlayerTurnStart:
                ServerPlayerTurnStart();
                break;
            case TurnState.PlayerTurn:
                //PlayerTurn();
                break;
            case TurnState.PlayerTurnEnd:
                ServerPlayerTurnEnd();
                break;
            case TurnState.EnemyTurnStart:
                ServerEnemyTurnStart();
                break;
            case TurnState.EnemyTurnEnd:
                ServerEnemyTurnEnd();
                break;
        }
    }
    [ContextMenu("切换到下一阶段")]
    [Server]
    /// <summary>
    /// 切换到下一个状态
    /// </summary>
    public void NextState()
    {
        // 枚举循环
        int next = ((int)turnState.Value + 1) % Enum.GetValues(typeof(TurnState)).Length;
        //turnState.Value = (TurnState)next;
        EnterState((TurnState)next);
    }
    [Server]
    public void ServerPlayerTurnStart()
    {
        ClientPlayerTurnStart();
        NextState();
    }
    [ObserversRpc]
    public void ClientPlayerTurnStart()
    {
        player.DrawCard(3);
        player.myPlayer.ChangeManaRpc(player.myPlayer.maxMana.Value);
        player.myPlayer.DeleteDefenseRpc();
        isAgree = false;
        turnButtomText.text = $"回合结束{agreeCount.Value}/{totalPlayers.Value}";
    }
    [Server]
    public void ServerPlayerTurnEnd()
    {
        ClientPlayerTurnEnd();
        NextState();
    }
    [ObserversRpc]
    public void ClientPlayerTurnEnd()
    {
        player.DiscardAllCards();

    }

    [Server]
    public void ServerEnemyTurnStart()
    {
        // ClientPlayerTurnEnd();
        StartEnemyActs();
        // NextState();
    }
    [Server]
    public void StartEnemyActs()
    {
        if (turnState.Value != TurnState.EnemyTurnStart)
        {
            Debug.LogWarning("不在敌人回合,警告!!!");
        }
        // 启动父协程，顺序执行敌人动作
        StartCoroutine(EnemyActs());
    }

    private IEnumerator EnemyActs()
    {
        foreach (var e in enemies)
        {
            // 等待当前敌人的 Act 协程执行完再执行下一个
            yield return StartCoroutine(e.Act());
        }
        if (turnState.Value != TurnState.EnemyTurnStart)
        {
            Debug.LogWarning("不在敌人回合,警告!!!");
        }
        NextState();
        Debug.Log("所有敌人行动完毕");
    }
    [Server]
    public void ServerEnemyTurnEnd()
    {
        //ClientPlayerTurnEnd();
        NextState();
    }

    #endregion 玩家回合方法
    [Server]
    public void ServerCheckWin()
    {
        foreach (var e in enemies)
        {
            if (!e.isDead.Value)
            {
                return;//有没死亡的
            }
        }
        foreach (var e in enemies)
        {
            Destroy(e.gameObject);
        }
        enemies.Clear();
        ClientWin();
        networkMapSceneManager.EndBattle();
    }
    /// <summary>
    /// 玩家客户端获胜后反应
    /// </summary>
    [ObserversRpc]
    public void ClientWin()
    {
        player.DestroyCard();
    }


    #region 同步数据变换回调
    private void AgreeCount_OnChange(int prev, int next, bool asServer)
    {
        if (turnState.Value == TurnState.PlayerTurn)
        {
            if (isAgree)
            {
                turnButtomText.text = $"已准备{agreeCount.Value}/{totalPlayers.Value}";
            }
            else
            {
                turnButtomText.text = $"回合结束{agreeCount.Value}/{totalPlayers.Value}";
            }
        }
        else
        {
            turnButtomText.text = $"请稍等";
        }

    }
    private void TurnState_OnChange(TurnState prev, TurnState next, bool asServer)
    {
        _turnState = turnState.Value;
        // Debug.Log("Debug:回合阶段改变为" + turnState.Value.ToString());
    }
    #endregion 同步数据变换回调


    private void OnDestroy()
    {
        turnState.OnChange -= TurnState_OnChange;
        agreeCount.OnChange -= AgreeCount_OnChange;
    }
    [ContextMenu("打印当前回合")]
    public void Test()
    {
        Debug.Log(turnState.Value);
    }
}
