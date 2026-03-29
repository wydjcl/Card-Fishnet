using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkMapSceneManager : NetworkBehaviour
{
    public NetworkPlayer player;
    public NetworkObject playerUIPrefab;
    public NetworkObject roomPrefab;
    public GameObject playerZone;
    [Header("UI")]
    public BattleUIRoot battleUIRoot;
    public BattleManager battleManager;
    public MapUIRoot mapUIRoot;
    public TextMeshProUGUI upText;
    public GameObject bagUI;

    public bool isBattle;
    private void Awake()
    {

    }
    public override void OnStartServer()
    {
        base.OnStartServer();
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
        NetworkObject roomP = Instantiate(roomPrefab);//创建房间,需要修改
        roomP.transform.position = new Vector3(0, 3.6f, 0);
        InstanceFinder.ServerManager.Spawn(roomP);

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
    [Server]
    public void StartBattle()
    {
        SpawnEnemyTest();
        battleManager.ServerInitBattle();

        isBattle = true;
        EnableBattleChildren();
        UnEnableMapChildren();
    }

    public void SpawnEnemyTest()
    {
        var e = Instantiate(Dic.Instance.enemies[0]);
        e.gameObject.transform.position = new Vector2(6.5f, 2.7f);
        InstanceFinder.ServerManager.Spawn(e);
        var e2 = Instantiate(Dic.Instance.enemies[0]);
        e2.gameObject.transform.position = new Vector2(0f, 2.7f);
        InstanceFinder.ServerManager.Spawn(e2);
    }
    [Server]
    public void EndBattle()
    {
        isBattle = false;
        EnableMapChildren();
        UnEnableBattleChildren();
        foreach (var p in BattleManager.Instance.players)
        {
            p.AfterBattle();
        }
    }

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
        Debug.Log("关闭战斗UI");
        foreach (Transform child in battleUIRoot.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    [Client]
    public void ClickBagUI()
    {
        bagUI.SetActive(!bagUI.activeSelf);
    }


}
