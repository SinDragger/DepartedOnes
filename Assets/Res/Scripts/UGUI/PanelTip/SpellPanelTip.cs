using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpellPanelTip : TempPanelTip
{
    public CastableSpellData spell;

    protected override void ShowTip(Vector2 pos)
    {
        StringBuilder s = new StringBuilder();
        s.Append($@"{spell.name}");
        s.AppendLine("");
        s.Append($@"<size=20>{spell.des}</size>");
        s.AppendLine("");
        s.Append($@"<size=20>{spell.costDes}</size>");
        UIManager.Instance.ShowTipPanel(spell.idName, s.ToString(), pos, deltaPos.y >= 0);
        tipTarget = spell.idName;
    }
}
