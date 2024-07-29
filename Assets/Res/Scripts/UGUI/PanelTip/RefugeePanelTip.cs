using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RefugeePanelTip : TempPanelTip
{
    protected override void ShowTip(Vector2 pos)
    {
        StringBuilder s = new StringBuilder();
        s.Append($@"子民数量(可调动/总数):{ColorUtil.GetColorTextStart("147632")}{RefugeeManager.Instance.ableRefugeeNum}{ColorUtil.GetColorTextTipEnd()}/{ColorUtil.GetColorTextStart("147632")}{RefugeeManager.Instance.refugeeNumber}{ColorUtil.GetColorTextTipEnd()}");
        s.AppendLine("");
        s.Append($@"<size=20>{ColorUtil.GetColorTextStart("6F6F6F")}可部署进行运作建筑、交互地区、研发生产等行动{ColorUtil.GetColorTextTipEnd()}</size>");
        s.AppendLine("");
        s.Append($@"<size=20>{ColorUtil.GetColorTextStart("6F6F6F")}但需满足地区控制度最低需求{ColorUtil.GetColorTextTipEnd()}</size>");
        UIManager.Instance.ShowTipPanel("RefugeePanelTip", s.ToString(), pos, deltaPos.y >= 0);
    }
}
