using DG.Tweening;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class Character : NetworkBehaviour
{
    //基础数值
    public readonly SyncVar<int> ID = new SyncVar<int>();
    public readonly SyncVar<int> health = new SyncVar<int>();
    public readonly SyncVar<int> maxHealth = new SyncVar<int>();
    public readonly SyncVar<int> block = new SyncVar<int>();
    public readonly SyncVar<bool> isDead = new SyncVar<bool>();
    public readonly SyncVar<bool> isPlayer = new SyncVar<bool>();

    //RPG数值
    public readonly SyncVar<int> attack = new SyncVar<int>();
    public readonly SyncVar<int> attackEx = new SyncVar<int>();//额外攻击力
    public readonly SyncVar<float> defense = new SyncVar<float>();
    public readonly SyncVar<float> defenseEx = new SyncVar<float>();//额外防御力
    //关键词数值
    public readonly SyncVar<int> thorn = new SyncVar<int>();//关键词 荆棘,手上对攻击者造成无来源伤害
    public readonly SyncVar<int> faith = new SyncVar<int>();//关键词:信仰,造成伤害时候随机友军回复25%血
    public readonly SyncVar<int> faithEx = new SyncVar<int>();//信仰额外数值,增加信仰回复量
    public readonly SyncVar<bool> faithDouble = new SyncVar<bool>();//信仰回复量翻倍
    public readonly SyncVar<bool> taunt = new SyncVar<bool>();//信仰回复量翻倍
    public readonly SyncVar<int> frost = new SyncVar<int>();//关键词:寒霜,减伤害,回合结束消失
    public readonly SyncVar<bool> frostForever = new SyncVar<bool>();//关键词:永冻,寒霜不会再回合结束消失
    public readonly SyncVar<bool> freeze = new SyncVar<bool>();//关键词:永冻,寒霜不会再回合结束消失
    [SerializeField]
    public readonly SyncList<Buff> buffList = new SyncList<Buff>();//预留bufflist

    public int _faith;
    public int _ID;
    public int _maxHealth;
    public int _health;
    public int _block;
    public bool _isDead;
    public bool _isPlayer;
    public int _thorn;
    public int _frost;
    [Header("需要导入UI层")]
    public TextMeshPro healthText;
    public TextMeshPro blockText;
    private GameObject dynamicText;
    public SpriteRenderer characterSprite;
    [ContextMenu("增加buff")]
    public void Test4()
    {
        Buff b = new Buff();
        b.buffName = "寒霜";
        b.buffStack = 5;
        b.forever = true;
        AddBuffRpc(b);
        // buffList.Add(b);
    }
    [ContextMenu("读取buff")]
    public void Test5()
    {
        if (buffList.Count == 0)
        {
            Debug.Log("无任何buff"); return;
        }
        Debug.Log("buff长度" + buffList.Count);
        Debug.Log("buff名字" + buffList[0].buffName);
        Debug.Log("buffstack" + buffList[0].buffStack);
        // Debug.Log("力:" + attack.Value + "/" + attackEx.Value);
    }
    [ContextMenu("移除buff")]
    public void Test6()
    {
        RemoveBuffRpc("力量");
    }
    [ContextMenu("获取999护甲")]
    public void Test7()
    {
        block.Value = 999;
    }
    [ContextMenu("读取护甲")]
    public void Test8()
    {
        Debug.Log("护甲值" + defense.Value + "/" + defenseEx.Value);
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
        block.OnChange += Block_OnChange;
        ID.OnChange += ID_OnChange;
        isDead.OnChange += IsDead_OnChange;
        faith.OnChange += Faith_OnChange;
        thorn.OnChange += Thorn_OnChange;
        attackEx.OnChange += AttackEx_OnChange;
        frost.OnChange += Frost_OnChange;
        dynamicText = Resources.Load<GameObject>("DynamicTextPrefab");


    }



    public override void OnStopClient()
    {
        base.OnStopClient();
        health.OnChange -= Health_OnChange;
        ID.OnChange -= ID_OnChange;
        block.OnChange -= Block_OnChange;
        isDead.OnChange -= IsDead_OnChange;
        faith.OnChange -= Faith_OnChange;
        thorn.OnChange -= Thorn_OnChange;
        attackEx.OnChange -= AttackEx_OnChange;
        frost.OnChange -= Frost_OnChange;

    }
    private void Update()
    {
        if (BattleManager.Instance == null)
        {
            return;
        }

        if (isPlayer.Value)
        {
            if (!BattleManager.Instance.players.Contains(this as Player))
            {
                BattleManager.Instance.players.Add(this as Player);
            }
        }
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
    public virtual void TakeDamege(Character caster, int i, bool source)
    {
        i = Mathf.CeilToInt(i * (100 - defense.Value - defenseEx.Value) * 0.01f);//根据防御系数调整伤害,向上取整正数,向下取整负数
        if (i < 0)
        {
            i = 0;
        }
        if (block.Value > i)
        {
            block.Value -= i;
            i = 0;
        }
        else
        {
            i -= block.Value;
            block.Value = 0;
        }
        health.Value -= i;
        if (health.Value <= 0)
        {
            isDead.Value = true;
            if (isPlayer.Value)
            {
                Debug.Log("玩家死亡");
            }
            else
            {
                ClientDead();
                BattleManager.Instance.ServerCheckWin();
            }
        }
        if (source)//如果有伤害来源,就统计伤害,例如吸血
        {
            if (caster != null)
            {
                caster.StatisticalDamage(i);//统计伤害,例如吸血
            }

        }
    }
    /// <summary>
    /// 客户端向服务端发送扣血请求
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamageRpc(Character caster, int i)
    {
        TakeDamege(caster, i, true);
    }
    /// <summary>
    /// 客户端向服务端发送无来源扣血请求
    /// </summary>
    /// <param name="i"></param>
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamageWishoutSourceRpc(Character caster, int i)
    {
        TakeDamege(caster, i, false);
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CauseDamageRpc(Character target, int i)
    {
        if (target == null)
        {
            Debug.Log("目标不存在");
            return;
        }
        if (frost.Value > 0)
        {
            Debug.Log("有寒霜值" + frost.Value);
            if (frost.Value > i)
            {

                var b = new Buff();
                b.buffName = "寒霜";
                b.buffStack = -i;
                AddBuffRpc(b);
                i = 0;
            }
            else
            {
                RemoveBuffRpc("寒霜");
                i -= frost.Value;
            }
        }

        if (target.thorn.Value > 0)
        {
            TakeDamege(this, target.thorn.Value, false);//如果目标身上有荆棘,就对自己造成无来源伤害
            Buff thornB = new Buff();
            thornB.buffStack = -1;
            thornB.buffName = "荆棘";
            target.AddBuffRpc(thornB);
        }
        target.TakeDamageRpc(this, i);
    }
    /// <summary>
    /// 统计伤害
    /// </summary>
    /// <param name="i"></param>
    [Server]
    public virtual void StatisticalDamage(int i)
    {
        if (faith.Value > 0)
        {
            var pList = new List<Character>();
            foreach (var p in BattleManager.Instance.players)
            {
                if (p.health.Value < p.maxHealth.Value)
                {
                    pList.Add(p);
                }
            }
            if (pList.Count == 0)
            {
                Debug.Log("无人负伤返回 ");
                return;
            }
            var item = pList[UnityEngine.Random.Range(0, pList.Count)];
            if (faithDouble.Value)
            {
                item.Heal(i / 2);//50%向上取整
            }
            else
            {
                item.Heal((i + 3) / 4);//25%向上取整
            }
            Buff buff = new Buff();
            buff.buffName = "信仰";
            buff.buffStack = -1;
            buff.forever = false;
            AddBuffRpc(buff);
        }
    }
    [ObserversRpc]
    public void ClientDead()
    {
        gameObject.SetActive(false);
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void HealRpc(int i)
    {
        Heal(i);
    }
    public virtual void Heal(int i)
    {
        if (maxHealth.Value - health.Value > i)
        {
            health.Value += i;
        }
        else
        {
            health.Value = maxHealth.Value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeBlockRpc(int i)
    {
        block.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void DeleteBlockRpc()
    {
        block.Value = 0;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddBuffRpc(Buff newBuff)
    {
        bool isOldBuff = false;

        foreach (var so in Dic.Instance.buffs)
        {
            if (so.buffName == newBuff.buffName)
            {
                foreach (var item in buffList)
                {
                    if (item.buffName == newBuff.buffName)
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
        //为0就清除
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            if (buffList[i].buffStack == 0)
            {
                foreach (var so in Dic.Instance.buffs)
                {
                    if (so.buffName == buffList[i].buffName)
                    {
                        so.RemoveEffect(this, 0);
                        Debug.Log("清除buff" + buffList[i].buffName);
                        buffList.RemoveAt(i);
                        break;
                    }
                }
                //break;
            }

        }
        //Debug.Log("buff增加" + attackEx.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveBuffRpc(string newBuffName)
    {
        //for (int i = buffList.Count - 1; i >= 0; i--)
        //{
        //    if (buffList[i].buffName == newBuff.buffName)
        //    {
        //        buffList[i].buffStack -= newBuff.buffStack;

        //        if (buffList[i].buffStack <= 0)
        //        {
        //            buffList.RemoveAt(i); // ✅ 安全删除
        //        }
        //    }
        //}
        //foreach (var so in Dic.Instance.buffs)
        //{
        //    if (so.buffName == newBuff.buffName)
        //    {
        //        so.RemoveEffect(this, newBuff.buffStack);
        //        break;
        //    }
        //}

        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            if (buffList[i].buffName == newBuffName)
            {
                foreach (var so in Dic.Instance.buffs)
                {
                    if (so.buffName == newBuffName)
                    {
                        so.RemoveEffect(this, buffList[i].buffStack);
                        break;
                    }
                }
                buffList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 回合开始计算buff
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void CalBuff()
    {
        //Debug.Log("检查buff");
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            foreach (var so in Dic.Instance.buffs)
            {
                if (so.buffName == buffList[i].buffName)
                {
                    so.TurnStartEffect(this, buffList[i].buffStack);
                    if (buffList[i].forever)
                    {
                    }
                    else
                    {
                        buffList[i].buffStack -= 1;
                        so.LessOneEffect(this);
                        if (buffList[i].buffStack <= 0)
                        {
                            so.RemoveEffect(this, 0);
                            buffList.RemoveAt(i);
                        }
                    }
                    break;
                }
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
    [ServerRpc(RequireOwnership = false)]
    public void TakeFaithRpc(int i)
    {
        faith.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeAttackExRpc(int i)
    {
        attackEx.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDefenseExRpc(int i)
    {
        defenseEx.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeTauntRpc(bool i)
    {
        taunt.Value = i;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeFrostRpc(int i)
    {
        frost.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeFrostForeverRpc(bool b)
    {
        frostForever.Value = b;
    }
    public void TakeFreezeRpc(bool b)
    {
        freeze.Value = b;
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
    private void Thorn_OnChange(int prev, int next, bool asServer)
    {
        _thorn = thorn.Value;
    }
    private void Block_OnChange(int prev, int next, bool asServer)
    {
        _block = block.Value;
        //if (!asServer && IsServerStarted)
        //{
        //    return;
        //}
        //var dT = Instantiate(dynamicText, transform);
        //var dTS = dT.GetComponent<DynamicText>();
        //dTS.ChangeToBlockText();
        if (blockText != null)
        {
            if (block.Value > 0)
            {
                blockText.gameObject.SetActive(true);
                blockText.text = $"DF:{block.Value}";
            }
            else
            {
                blockText.gameObject.SetActive(false);
            }
        }
    }
    private void IsDead_OnChange(bool prev, bool next, bool asServer)
    {
        _isDead = isDead.Value;
    }
    private void Faith_OnChange(int prev, int next, bool asServer)
    {
        _faith = faith.Value;
    }
    public virtual void AttackEx_OnChange(int prev, int next, bool asServer)
    {

    }
    private void Frost_OnChange(int prev, int next, bool asServer)
    {
        _frost = frost.Value;
        Debug.Log("寒霜值改变" + _frost);
    }
    #endregion
}
[System.Serializable]
public class Buff
{
    public string buffName;
    public int buffStack;
    public bool forever;
}
