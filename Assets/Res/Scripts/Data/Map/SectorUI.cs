using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 注册事件来进行
/// </summary>
public class SectorUI : EventListener
{
    protected override string EventId => MapEventType.SECTOR_SELECT.ToString();
    //
    public Color recognizeColor;
    Color selfColor;
    public IColorChangableUI ui;
    bool isSelect;

    public void InitTargetColor(Color color)
    {
        selfColor = color;
        float percent = SectorBlockManager.Instance.GetBlock(recognizeColor).playerViewPercent;
        ui.ChangeColorTo(Color.Lerp(Color.black, selfColor, percent));

    }

    public void UpdateColor()
    {
        float percent = SectorBlockManager.Instance.GetBlock(recognizeColor).playerViewPercent;
        Color targetColor = selfColor;
        ui.ChangeColorTo(Color.Lerp(Color.black, selfColor, percent / 5f));
        if (ui != null)
        {
            targetColor = Color.Lerp(Color.black, targetColor, percent / 5f);
            ui.ChangeColorTo(targetColor);
        }
    }

    protected override void OnTrigger(EventData data)
    {
        Color nowSelectColor = data.GetValue<Color>("SectorColor");
        var block = SectorBlockManager.Instance.GetBlock(recognizeColor);
        if (ui != null)
        {
            if (recognizeColor == nowSelectColor && !isSelect)
            {
                isSelect = true;
                foreach (var border in block.borders)
                {
                    var line = border.borderObject.GetComponent<LineRenderer>();
                    border.borderObject.transform.SetAsFirstSibling();
                    line.widthMultiplier = 0.3f;
                    line.startColor = Color.yellow;
                    line.endColor = Color.yellow;
                }
            }
            else if (recognizeColor != nowSelectColor && isSelect)
            {
                isSelect = false;
                foreach (var border in block.borders)
                {
                    if (border.MatchTargetColor(nowSelectColor)) continue;
                    var line = border.borderObject.GetComponent<LineRenderer>();
                    line.widthMultiplier = 0.2f;
                    line.startColor = Color.white;
                    line.endColor = Color.white;
                }
            }
        }
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }

    public void OnShow()
    {
        gameObject.SetActive(true);
    }
}
