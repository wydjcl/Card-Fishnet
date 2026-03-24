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
    public TextMeshPro manaText;
    public readonly SyncVar<int> mana = new SyncVar<int>();
    public readonly SyncVar<int> maxMana = new SyncVar<int>();
    public int _mana;
    public int _maxMana;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            name = "本玩家" + Owner.ClientId + "UI";
            InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<NetworkPlayer>().myPlayer = this;
        }
        else
        {
            name = "其他玩家" + Owner.ClientId + "UI";
        }

        mana.OnChange += Mana_OnChange;

        if (IsServerStarted)
        {
            maxMana.Value = 3;
            mana.Value = 3;//初始法力值,需修改
            isPlayer.Value = true;
        }
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
