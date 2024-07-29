using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LifeRecoveryPanelTip : TempPanelTip
{
    protected override void ShowTip(Vector2 pos)
    {
        LegionControl nowLegion = LegionManager.Instance.nowLegion;
        int delta = 0;
        if (nowLegion != null && nowLegion.mainGeneral != null)
        {
            delta = (int)Mathf.Ceil(nowLegion.mainGeneral.LifeLostPercent);
        }
        StringBuilder s = new StringBuilder();
        s.Append($@"魂灵愈合:完全恢复生命值");
        s.AppendLine("");
        s.Append($@"每1%消耗1点灵魂能量({delta}%)");
        //s.AppendLine("");
        //s.Append($@"+1 来自运作的建筑");
        UIManager.Instance.ShowTipPanel("LifeRecoveryPanelTip", s.ToString(), pos, deltaPos.y >= 0);

    }
}
