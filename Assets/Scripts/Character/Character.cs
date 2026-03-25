using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using Sequence = DG.Tweening.Sequence;

public class Character : NetworkBehaviour
{
    public readonly SyncVar<int> ID = new SyncVar<int>();
    public readonly SyncVar<int> health = new SyncVar<int>();
    public readonly SyncVar<int> maxHealth = new SyncVar<int>();
    public readonly SyncVar<int> defense = new SyncVar<int>();
    public readonly SyncVar<bool> isDead = new SyncVar<bool>();
    public readonly SyncVar<bool> isPlayer = new SyncVar<bool>();
    public readonly SyncVar<int> thorn = new SyncVar<int>();//关键词 荆棘
    public readonly SyncList<Buff> buffList = new SyncList<Buff>();//预留bufflist
    public int _ID;
    public int _maxHealth;
    public int _health;
    public int _defense;
    public bool _isDead;
    public bool _isPlayer;
    public int _thorn;
    [Header("需要导入UI层")]
    public TextMeshPro healthText;
    public TextMeshPro defenseText;
    private GameObject dynamicText;
    public SpriteRenderer characterSprite;
    [ContextMenu("增加buff")]
    public void Test4()
    {
        Buff b = new Buff();
        b.buffId = 0;
        b.buffStack = 3;
        b.forever = true;
        AddBuffRpc(b);
        // buffList.Add(b);
    }
    [ContextMenu("读取buff")]
    public void Test5()
    {
        Debug.Log("buff长度" + buffList.Count);
        Debug.Log("buffstack" + buffList[0].buffStack);
        Debug.Log("t:" + thorn.Value);
    }
    #region 生命周期
    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        health.OnChange += Health_OnChange;
        defense.OnChange += Defense_OnChange;
        ID.OnChange += ID_OnChange;
        isDead.OnChange += IsDead_OnChange;
        dynamicText = Resources.Load<GameObject>("DynamicTextPrefab");
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        health.OnChange -= Health_OnChange;
        ID.OnChange -= ID_OnChange;
        defense.OnChange -= Defense_OnChange;
        isDead.OnChange -= IsDead_OnChange;
    }
    protected virtual void OnDestroy()
    {
        transform.DOKill();
        characterSprite.DOKill();
    }

    #endregion
    #region 服务端增改数值或Buff
    /// <summary>
    /// 服务端扣血
    /// </summary>
    /// <param name="i"></param>
    [Server]
    public virtual void TakeDamege(int i)
    {
        if (defense.Value > i)
        {
            defense.Value -= i;
            i = 0;
        }
        else
        {
            i -= defense.Value;
            defense.Value = 0;
        }
        health.Value -= i;
        if (health.Value <= 0)
        {
            isDead.Value = true;
            if (isPlayer.Value)
            {

            }
            else
            {
                gameObject.SetActive(false);
                BattleManager.Instance.ServerCheckWin();
            }
        }
    }
    /// <summary>
    /// 客户端向服务端发送扣血请求
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamageRpc(int i)
    {
        TakeDamege(i);
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDefenseRpc(int i)
    {
        defense.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void DeleteDefenseRpc()
    {
        defense.Value = 0;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddBuffRpc(Buff newBuff)
    {
        bool isOldBuff = false;

        foreach (var so in Dic.Instance.buffs)
        {
            if (so.buffId == newBuff.buffId)
            {
                foreach (var item in buffList)
                {
                    if (item.buffId == newBuff.buffId)
                    {
                        item.buffStack += newBuff.buffStack;
                        isOldBuff = true;
                        so.ApplyEffect(this, newBuff.buffStack, item.buffStack);
                    }
                }
                if (!isOldBuff)
                {
                    buffList.Add(newBuff);
                    so.ApplyEffect(this, newBuff.buffStack, newBuff.buffStack);
                }

                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveBuffRpc(Buff newBuff)
    {
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            if (buffList[i].buffId == newBuff.buffId)
            {
                buffList[i].buffStack -= newBuff.buffStack;

                if (buffList[i].buffStack <= 0)
                {
                    buffList.RemoveAt(i); // ✅ 安全删除
                }
            }
        }
        foreach (var so in Dic.Instance.buffs)
        {
            if (so.buffId == newBuff.buffId)
            {
                so.RemoveEffect(this, newBuff.buffStack);
                break;
            }
        }
    }
    /// <summary>
    /// 获得荆棘
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public void TakeThornRpc(int i)
    {
        thorn.Value += i;
    }
    #endregion 服务端增改数值或Buff
    #region 同步数据回调

    protected virtual void Health_OnChange(int prev, int next, bool asServer)
    {
        if (!asServer && IsServerStarted)
        {
            return;
        } //禁用host端的客户端
          // Debug.Log("血量变换");
        _health = health.Value;
        if (next < prev)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(characterSprite.DOFade(0.3f, 0.12f))   // 淡出
               .Append(characterSprite.DOFade(1f, 0.05f))
               .SetLink(characterSprite.gameObject);
            ;   // 淡入 
        }


        var dT = Instantiate(dynamicText, transform);
        var dTS = dT.GetComponent<DynamicText>();
        dTS.ChangeToHurtText(next - prev);
        if (healthText != null)
        {
            healthText.text = $"HP:{health.Value}/{maxHealth.Value}";
        }
    }
    private void ID_OnChange(int prev, int next, bool asServer)
    {
        _ID = ID.Value;
    }
    private void Defense_OnChange(int prev, int next, bool asServer)
    {
        _defense = defense.Value;
        if (defenseText != null)
        {
            if (defense.Value > 0)
            {
                defenseText.gameObject.SetActive(true);
                defenseText.text = $"DF:{defense.Value}";
            }
            else
            {
                defenseText.gameObject.SetActive(false);
            }
        }
    }
    private void IsDead_OnChange(bool prev, bool next, bool asServer)
    {
        _isDead = isDead.Value;
    }
    #endregion
}
[System.Serializable]
public class Buff
{
    public int buffId;
    public int buffStack;
    public bool forever;
}
