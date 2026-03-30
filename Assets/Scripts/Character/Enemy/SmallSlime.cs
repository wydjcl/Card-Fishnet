using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallSlime : Enemy
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServerStarted)
        {
            maxHealth.Value = 30;
            health.Value = 30;//以后改
            attack.Value = 7;
            skillNum.Value = 0;
            maxSkillNum.Value = 1;
            // ChangeIntentionText($"攻击{attack.Value + attackEx.Value}");
        }
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
            foreach (var conn in InstanceFinder.ServerManager.Clients)
            {
                NetworkPlayer playerObj = conn.Value.FirstObject.GetComponent<NetworkPlayer>();
                playerObj.CreateOneCard(conn.Value, "清理粘液", 0);
                playerObj.CreateOneCard(conn.Value, "清理粘液", 0);
            }

            NextSkill();
        }
        else if (skillNum.Value == 1)
        {
            this.CauseDamageRpc(BattleManager.Instance.FindPlayer(), attack.Value + attackEx.Value);
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
            IntentionText.text = ($"施加负面");
        }
        if (skillNum.Value == 1)
        {
            IntentionText.text = ($"攻击{attack.Value + attackEx.Value}");
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
