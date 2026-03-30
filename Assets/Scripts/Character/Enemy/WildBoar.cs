using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoar : Enemy
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServerStarted)
        {
            maxHealth.Value = 50;
            health.Value = 50;//以后改
            attack.Value = 15;
            skillNum.Value = 0;
            maxSkillNum.Value = 2;
            // ChangeIntentionText($"攻击{attack.Value + attackEx.Value}");
        }
        //if (IsServerStarted)
        //{
        //    if (skillNum.Value == 0)
        //    {
        //        ChangeIntentionText($"攻击{attack.Value + attackEx.Value}");
        //    }
        //}

        // IntentionText.text = $"攻击{attack.Value + attackEx.Value}";
    }

    public override IEnumerator Act()
    {
        yield return null;
        // 模拟行动耗时，比如攻击动画 1 秒
        //yield return new WaitForSeconds(1.5f);
        ClientAni();
        yield return StartCoroutine(Ani());
        //foreach (var p in BattleManager.Instance.players)
        //{
        //    this.CauseDamageRpc(p, attack.Value);
        //}
        if (skillNum.Value == 0)
        {
            this.CauseDamageRpc(BattleManager.Instance.FindPlayer(), attack.Value + attackEx.Value);
            NextSkill();
        }
        else if (skillNum.Value == 1)
        {
            NextSkill();
        }
        else if (skillNum.Value == 2)
        {
            NextSkill();
        }
        yield return null;
    }

    [Client]
    public override void ClientChangeIntentionBySkill()//冲锋15→休整→休整→冲锋15
    {
        base.ClientChangeIntentionBySkill();
        if (skillNum.Value == 0)
        {
            IntentionText.text = ($"攻击{attack.Value + attackEx.Value}");
        }
        if (skillNum.Value == 1)
        {
            IntentionText.text = ($"休整");
        }
        if (skillNum.Value == 2)
        {
            IntentionText.text = ($"休整");
        }
    }
    public override void AttackEx_OnChange(int prev, int next, bool asServer)
    {
        base.AttackEx_OnChange(prev, next, asServer);
        ClientChangeIntentionBySkill();
    }
}
