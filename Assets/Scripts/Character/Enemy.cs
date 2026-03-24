using DG.Tweening;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Enemy : Character
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        BattleUIRoot battleUIRoot = FindObjectOfType<BattleUIRoot>();

        transform.SetParent(battleUIRoot.transform);
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
    [ObserversRpc]
    public virtual void ClientAni()
    {
        if (IsServerStarted)
        {
            return;
        }
        StartCoroutine(Ani());
    }
}
