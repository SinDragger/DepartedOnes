using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : UIPanel
{
    public override string uiPanelName => "TutorialPanel";
    public TextUnit textUnit;

    public void SetShow(int state = 0)
    {
        string textString = "";
        textUnit.SpriteClear();
        switch (state)
        {
            case 0:
                textUnit.SetSprite(0, DataBaseManager.Instance.GetSpriteByIdName($"Icon_Space"));
                textString += $"按 {"<quad>"} 重生";
                textUnit.SetText(textString);
                break;
            case 1:
                textUnit.SpriteClear();
                textUnit.SetSprite(0, DataBaseManager.Instance.GetSpriteByIdName($"Icon_Key_A"));
                textUnit.SetSprite(1, DataBaseManager.Instance.GetSpriteByIdName($"Icon_Key_W"));
                textUnit.SetSprite(2, DataBaseManager.Instance.GetSpriteByIdName($"Icon_Key_S"));
                textUnit.SetSprite(3, DataBaseManager.Instance.GetSpriteByIdName($"Icon_Key_D"));
                textString += $@"按 {"<quad>"}{"<quad>"}{"<quad>"}{"<quad>"} 移动
——离开陵寝——";
                textUnit.SetText(textString);
                break;
            case 2:
                textUnit.SpriteClear();
                textString += "W A S D/鼠标中键拖拽移动镜头 左键-可框选部队/点击兵牌选中 右键-移动/攻击";
                textUnit.SetText(textString);
                break;
            case 3:
                textUnit.SpriteClear();
                textString += "继续前进";
                textUnit.SetText(textString);
                break;
            case 4:
                textUnit.SpriteClear();
                textString += "鼠标右键单击兵牌 打开技能轮 使用驱役死尸 复活遗落的人类骸骨 战胜狼群  ";
                textUnit.SetText(textString);
                break;
            case 5:
                textUnit.SpriteClear();
                textString += "看来他们只能到这了 这些破烂的骨架没有利用的价值了 ";
                textUnit.SetText(textString);
                StartCoroutine(Show6());
                break;
            case 6:
                textUnit.SpriteClear();
                textString += "继续前进吧";
                textUnit.SetText(textString);
                break;
        }
    }
    IEnumerator Show6() {
        yield return new WaitForSeconds(1f);
        SetShow(6);
    }
    public void EndShow()
    {
        textUnit.SpriteClear();
        textUnit.SetText("");
    }
}
