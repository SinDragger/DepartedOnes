using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LabourPanelTip : TempPanelTip
{
    protected override void ShowTip(Vector2 pos)
    {
        StringBuilder s = new StringBuilder();
        s.Append($@"可动用的劳力值:{ColorUtil.GetColorTextStart("147632")}{LabourWorkManager.Instance.labourPoint}{ColorUtil.GetColorTextTipEnd()}");
        s.AppendLine("");
        s.Append($@"+{ColorUtil.GetColorTextStart("147632")}{RefugeeManager.Instance.ableRefugeeNum}{ColorUtil.GetColorTextTipEnd()} 来自闲置的子民");
        //s.AppendLine("");
        //s.Append($@"+1 来自运作的建筑");
        UIManager.Instance.ShowTipPanel("LabourPanelTip", s.ToString(), pos, deltaPos.y >= 0);
    }
}
