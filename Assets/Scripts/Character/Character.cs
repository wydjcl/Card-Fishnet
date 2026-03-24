using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public readonly SyncVar<int> ID = new SyncVar<int>();
    public readonly SyncVar<int> health = new SyncVar<int>();
    public readonly SyncVar<int> maxHealth = new SyncVar<int>();
    public readonly SyncVar<int> defense = new SyncVar<int>();
    public readonly SyncVar<bool> isDead = new SyncVar<bool>();
    public readonly SyncVar<bool> isPlayer = new SyncVar<bool>();
    public int _ID;
    public int _maxHealth;
    public int _health;
    public int _defense;
    public bool _isDead;
    public bool _isPlayer;
    [Header("需要导入UI层")]
    public TextMeshPro healthText;
    public TextMeshPro defenseText;
    private GameObject dynamicText;
    public SpriteRenderer characterSprite;


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

    [Server]
    public virtual void TakeDefense(int i)
    {
        defense.Value += i;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDefenseRpc(int i)
    {
        TakeDefense(i);
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void DeleteDefenseRpc()
    {
        defense.Value = 0;
    }

    [ContextMenu("改变ID")]
    public void ChangeID()
    {
        ID.Value = (int)(Random.value * 100);
    }



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
}
