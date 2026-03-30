using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public TextMeshPro IntentionText;

    public readonly SyncVar<int> skillNum = new SyncVar<int>();
    public readonly SyncVar<int> maxSkillNum = new SyncVar<int>();
    float timer = 0;
    public override void OnStartClient()
    {
        base.OnStartClient();
        BattleUIRoot battleUIRoot = FindObjectOfType<BattleUIRoot>();

        transform.SetParent(battleUIRoot.transform);
        Transform t = transform.Find("IntentionText");
        healthText = transform.Find("HealthText").GetComponent<TextMeshPro>();
        blockText = transform.Find("BlockText").GetComponent<TextMeshPro>();
        characterSprite = transform.Find("EnemySprite").GetComponent<SpriteRenderer>();
        if (t != null)
        {
            IntentionText = t.GetComponent<TextMeshPro>();
            IntentionText.text = "修改成功";
        }
        BattleManager.Instance.enemies.Add(this);
    }
    public virtual IEnumerator Act()
    {
        yield return null;
    }
    public virtual IEnumerator Ani()
    {
        // 假设是缩放目标
        Transform target = this.transform;

        // 初始缩放
        Vector3 originalScale = target.localScale;

        // 使用 DOTween 创建缩小 -> 放大动画
        // 注意 DOTween 动画不是 IEnumerator，需要 yield return WaitForCompletion
        Tween tween = target
            .DOScale(originalScale * 0.8f, 0.18f)  // 缩小到 0.5 倍，持续 0.2 秒
            .SetLoops(2, LoopType.Yoyo)
            .SetLink(target.gameObject);
        ;          // Yoyo 表示缩小完再放大回原始大小

        // 等待动画完成
        yield return tween.WaitForCompletion();

        // 动画完成后继续协程
        //Debug.Log(name + " Ani 动画完成");

    }
    [Server]
    public virtual void NextSkill()
    {
        skillNum.Value++;
        if (skillNum.Value > maxSkillNum.Value)
        {
            skillNum.Value = 0;
        }
    }

    [ObserversRpc]
    public virtual void ChangeIntentionText(string text)
    {
        if (IntentionText != null)
        {
            IntentionText.text = text;
        }
    }

    [Client]
    public virtual void ClientChangeIntentionBySkill()
    {
        // Debug.Log("改变意图" + skillNum.Value);
    }
    [ObserversRpc]
    public virtual void ClientAni()
    {
        if (IsServerStarted)
        {
            return;
        }
        StartCoroutine(Ani());
    }
    void Update()
    {
        //if (!IsServerStarted)
        //{
        //    return;
        //}
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer = 0f;
            ClientChangeIntentionBySkill();
        }
    }
}
