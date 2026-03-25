using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 继承角色类,属于游戏内的角色,存储血量等属性,卡牌等字段存储在NetworkPlayer中
/// </summary>
public class Player : Character
{
    public GameObject cardZone;
    public NetworkPlayer netPlayer;
    public TextMeshPro manaText;
    public readonly SyncVar<int> mana = new SyncVar<int>();
    public readonly SyncVar<int> maxMana = new SyncVar<int>();
    public readonly SyncVar<int> characterId = new SyncVar<int>();

    public int _mana;
    public int _maxMana;
    public int _characterId;

    public override void OnStartClient()
    {
        base.OnStartClient();
        netPlayer = Owner.FirstObject.GetComponent<NetworkPlayer>();//他的网络玩家对象
        if (IsServerStarted)//服务端给他基础属性赋值,需整合
        {
            maxMana.Value = 3;
            mana.Value = 3;//初始法力值,需修改
            isPlayer.Value = true;
            InitDataRpc(characterId.Value);//根据角色id改变特殊属性
        }

        if (IsOwner)
        {
            name = "本玩家" + Owner.ClientId + "UI";//编辑器内改名
            InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>().myPlayer = this;//网络玩家调用角色玩家
        }
        else
        {
            name = "其他玩家" + Owner.ClientId + "UI";
        }

        mana.OnChange += Mana_OnChange;

        characterSprite.sprite = Resources.Load<Sprite>($"P_{characterId.Value}");
    }



    [ServerRpc(RequireOwnership = false)]
    public void InitDataRpc(int i)
    {
        if (i == 0)
        {
            maxHealth.Value = 88;
            health.Value = 88;
            attack.Value = 6;
            defense.Value = 10;
        }
        if (i == 1)
        {
            maxHealth.Value = 77;
            health.Value = 77;
        }
    }
    [ContextMenu("把血量改成999")]
    [Server]
    public void Test()
    {
        maxHealth.Value = 999;
        health.Value = 999;
    }

    /// <summary>
    /// 减少法力值Rpc
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public void ConsumeManaRpc(int i)
    {
        ChangeMana(mana.Value - i);
    }
    /// <summary>
    /// 改变法力值到当前数Rpc
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public void ChangeManaRpc(int i)
    {
        ChangeMana(i);
    }
    [Server]
    public void ChangeMana(int i)
    {
        mana.Value = i;
    }
    /// <summary>
    /// 战斗完后清理buff和一些数据
    /// </summary>
    [Server]

    public void AfterBattle()
    {
        buffList.Clear();
        block.Value = 0;
        faith.Value = 0;
        thorn.Value = 0;
    }



    private void Mana_OnChange(int prev, int next, bool asServer)
    {
        _mana = mana.Value;
        if (manaText != null)
        {
            manaText.text = $"MP:{mana.Value}/{maxMana.Value}";
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        mana.OnChange -= Mana_OnChange;
    }
}
