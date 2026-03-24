using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSkull : Enemy
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServerStarted)
        {
            maxHealth.Value = 66;
            health.Value = 66;//以后改
        }
    }

    public override IEnumerator Act()
    {
        // 模拟行动耗时，比如攻击动画 1 秒
        //yield return new WaitForSeconds(1.5f);
        ClientAni();
        yield return StartCoroutine(Ani());
        foreach (var p in BattleManager.Instance.players)
        {
            p.TakeDamege(7);
        }
    }

    //public override IEnumerator Ani()
    //{
    //    // 假设是缩放目标
    //    Transform target = this.transform;

    //    // 初始缩放
    //    Vector3 originalScale = target.localScale;

    //    // 使用 DOTween 创建缩小 -> 放大动画
    //    // 注意 DOTween 动画不是 IEnumerator，需要 yield return WaitForCompletion
    //    Tween tween = target
    //        .DOScale(originalScale * 0.5f, 0.2f)  // 缩小到 0.5 倍，持续 0.2 秒
    //        .SetLoops(2, LoopType.Yoyo);          // Yoyo 表示缩小完再放大回原始大小

    //    // 等待动画完成
    //    yield return tween.WaitForCompletion();

    //    // 动画完成后继续协程
    //    Debug.Log(name + " Ani 动画完成");

    //}
}
