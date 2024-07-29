using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StatusPanelTip : TempPanelTip
{
    public StandardStatus status;
    protected override void ShowTip(Vector2 pos)
    {
        StringBuilder s = new StringBuilder();
        //s.Append($@"『{status.name}』");
        s.Append($@"{status.name}");
        s.AppendLine("");
        s.Append($@"  <size=26>{status.effectDescribe}</size>");
        s.AppendLine("");
        s.Append($@"  <size=20>{ColorUtil.GetColorTextStart("6F6F6F")}{status.describe}{ColorUtil.GetColorTextTipEnd()}</size>");
        UIManager.Instance.ShowTipPanel("StatusTip", s.ToString(), pos, deltaPos.y >= 0);
    }
}
