using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkMapSceneManager : NetworkBehaviour
{
    public NetworkPlayer player;
    public NetworkObject playerUIPrefab;
    public NetworkObject roomPrefab;
    public GameObject playerZone;
    public List<Room> rooms;
    [Header("UI")]
    public BattleUIRoot battleUIRoot;
    public BattleManager battleManager;
    public MapUIRoot mapUIRoot;
    public TextMeshProUGUI upText;
    public GameObject bagUI;
    public RewardUI rewardUI;
    public ShopUI shopUI;
    [Header("数据层")]
    public readonly SyncVar<int> seed = new SyncVar<int>();
    public System.Random rng;
    public readonly SyncVar<int> rnd = new SyncVar<int>();
    public Vector2 level = new Vector2(0, 0);

    public bool isBattle;
    #region 生命周期
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (seed.Value <= 0)
        {
            seed.Value = UnityEngine.Random.Range(0, int.MaxValue);
        }
        rng = new System.Random(seed.Value);


        int i = 0;
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {

            // 你想给每个玩家生成的物体
            NetworkObject obj = Instantiate(playerUIPrefab, playerZone.transform);
            if (i == 0)
            {
                obj.transform.position = new Vector2(-8.3f, -2.2f);
            }
            if (i == 1)
            {
                obj.transform.position = new Vector2(-4.6f, -2.2f);
            }
            if (i == 2)
            {
                obj.transform.position = new Vector2(-0.9f, -2.2f);
            }
            if (i == 3)
            {
                obj.transform.position = new Vector2(2.8f, -2.2f);
            }
            var objP = obj.GetComponent<Player>();
            objP.characterId.Value = conn.FirstObject.GetComponent<NetworkPlayer>().characterId.Value;
            // 分配 Owner
            InstanceFinder.ServerManager.Spawn(obj, conn);
            i++;
            Debug.Log($"为玩家 {conn.ClientId} 生成了一个玩家对象UI");
        }
        CreateRoom();

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        player = InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>();
        player.InitDeck();
        battleUIRoot = FindObjectOfType<BattleUIRoot>();
        mapUIRoot = FindObjectOfType<MapUIRoot>();

        if (IsServerStarted)
        {
            UnEnableBattleChildren();
        }
    }



    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
    }
    #endregion
    #region 战斗相关


    [Server]
    public void StartBattle()
    {
        SpawnEnemyTest();
        battleManager.ServerInitBattle();

        isBattle = true;
        EnableBattleChildren();
        UnEnableMapChildren();
    }
    [Server]
    public void SpawnEnemyTest()
    {
        int r1 = UnityEngine.Random.Range(0, Dic.Instance.enemies.Count);
        var e = Instantiate(Dic.Instance.enemies[r1]);
        e.gameObject.transform.position = new Vector2(6.5f, 2.7f);
        InstanceFinder.ServerManager.Spawn(e);

        int r2 = UnityEngine.Random.Range(0, Dic.Instance.enemies.Count);
        var e2 = Instantiate(Dic.Instance.enemies[r2]);
        e2.gameObject.transform.position = new Vector2(0f, 2.7f);
        InstanceFinder.ServerManager.Spawn(e2);
    }
    [Server]
    public void EndBattle()
    {
        //isBattle = false;
        EnableMapChildren();
        UnEnableBattleChildren();
        foreach (var p in BattleManager.Instance.players)
        {
            p.AfterBattle();
        }
        //Debug.Log("展示战斗奖励");
        StartRewardRpc();
        //改变房间
        CheckRooms();

    }

    /// <summary>
    /// 检查是否所有房间都被挑战过了，如果是，则进入下一层，生成新房间,且给房间的投票数清零
    /// </summary>
    [Server]
    public void CheckRooms()
    {
        var haveUnLock = false;
        InitRoom();
        foreach (var r in rooms)
        {
            r.chosenNum.Value = 0;
            if (r.isLock.Value == false)
            {
                haveUnLock = true;
            }
        }
        if (!haveUnLock)
        {
            Debug.Log("全房间都挑战完毕");
            foreach (var r in rooms)
            {
                Destroy(r.gameObject);
            }
            rooms.Clear();
            level.y += 1;
            CreateRoom();
        }
        else
        {
            Debug.Log("还有空余房间");
        }
    }
    [ObserversRpc]
    public void InitRoom()
    {
        foreach (var r in rooms)
        {
            r.isChosen = false;
            r.chosenText.text = "0";
            if (r.isLock.Value)
            {
                r.chosenText.text = "";
            }
        }
    }
    [ObserversRpc]
    public void StartRewardRpc()
    {
        rewardUI.gameObject.SetActive(true);
        List<int> list = new List<int>();
        for (int x = 0; x < Dic.Instance.GetCardsCount(player.characterId.Value); x++)
        {
            list.Add(x);
        }

        List<int> result = new List<int>();
        //Debug.Log("list" + list.Count);
        //Debug.Log($"Init: list={string.Join(",", list)}");
        for (int k = 0; k < 3; k++)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            result.Add(list[index]);
            list.RemoveAt(index);
        }
        // Debug.Log($"Init: result={string.Join(",", result)}");

        rewardUI.Init(result);
    }
    [ObserversRpc]
    public void StartShopRpc()
    {
        shopUI.gameObject.SetActive(true);
        CheckRooms();
    }
    #endregion
    #region 开关地图节点和战斗节点
    [ObserversRpc]
    public void EnableMapChildren()
    {
        foreach (Transform child in mapUIRoot.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    [ObserversRpc]
    public void UnEnableMapChildren()
    {
        foreach (Transform child in mapUIRoot.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    [ObserversRpc]
    public void EnableBattleChildren()
    {
        foreach (Transform child in battleUIRoot.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    [ObserversRpc]
    public void UnEnableBattleChildren()
    {
        //Debug.Log("关闭战斗UI");
        foreach (Transform child in battleUIRoot.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    #endregion
    #region 房间相关
    public void CreateRoom()
    {
        Debug.Log("现在在" + level.y + "层");
        if (false)
        {
            NetworkObject roomP = Instantiate(roomPrefab);//创建房间,需要修改
            roomP.transform.position = new Vector3(0, 3f, 0);
            Room r = roomP.GetComponent<Room>();
            r.roomType.Value = RoomType.SmallEnemy;
            InstanceFinder.ServerManager.Spawn(roomP);
        }
        else
        {
            if (true)
            {
                var n = rng.Next(1, 4);
                Debug.Log("生成了" + n + "个房间");
                for (int i = 0; i < n; i++)
                {
                    NetworkObject roomP = Instantiate(roomPrefab);//创建房间,需要修改
                    if (n == 1)
                    {
                        roomP.transform.position = new Vector3(0, 3f, 0);
                    }
                    if (n == 2)
                    {
                        roomP.transform.position = new Vector3(-2f + 4f * i, 3f, 0);
                    }
                    if (n == 3)
                    {
                        roomP.transform.position = new Vector3(-4.5f + 4.5f * i, 3f, 0);
                    }

                    Room r = roomP.GetComponent<Room>();
                    var ii = rng.Next(1, 3);
                    if (ii == 1)
                    {
                        r.roomType.Value = RoomType.SmallEnemy;
                    }
                    else
                    {
                        r.roomType.Value = RoomType.Shop;
                    }

                    InstanceFinder.ServerManager.Spawn(roomP);
                }
            }
        }

    }
    #endregion

    [Client]
    public void ClickBagUI()
    {
        bagUI.SetActive(!bagUI.activeSelf);
    }


}
