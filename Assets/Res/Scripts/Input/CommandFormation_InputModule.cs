using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFormation_InputModule : InputModule
{
    public override void Init()
    {
        mouseDic[MouseCode.LEFT] = new InputResponseNode(IsTrue, 3);
        mouseDic[MouseCode.LEFT_DOWN] = new InputResponseNode(IsTrue, 3);
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(IsTrue, 3);

        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputRightPreviewGUID, 3);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputRightUp, 10);
        keyUpDic[InputType.ESC.ToString()] = new InputResponseNode(InputRightEsc, 3);
        base.Init();
    }

    public override void Active()
    {
        base.Active();

        isRightCheckActive = true;
        rightIsESC = false;
    }

    bool IsTrue(object param)
    {

        return true;
    }

    bool isRightCheckActive = true;

    bool rightIsESC = false;
    bool InputRightEsc(object param)
    {
        if (rightIsESC) return false;
        rightIsESC = true;
        UnitControlManager.instance.rightMouseCenter = default;
        foreach (var item in UnitControlManager.instance.guidesList)
        {
            item.GetComponent<GUIDEControl>().Action();
        }
        UnitControlManager.instance.guidesList.Clear();
        UnitControlManager.instance.desTargetDic.Clear();
        return true;
    }
    bool InputRightPreviewGUID(object param)
    {
        if (rightIsESC)
        {
            return true;
        }
        if (InputManager.Instance.mouseRightDeltaDistance < 33f && isRightCheckActive)
        {
            //只显示光圈
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.rightFirstV);
            RaycastHit m_HitInfo;
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                UnitControlManager.instance.PreviewMoveGUIDE(m_HitInfo.point);
            }
            return true;
        }
        if (isRightCheckActive)
        {
            isRightCheckActive = false;
        }
        UnitControlManager.instance.PreviewChangeMoveGUIDE(InputManager.Instance.mouseWorldPos);
        return true;
    }

    bool InputRightUp(object param)
    {
        isRightCheckActive = true;
        if (rightIsESC)
        {
            rightIsESC = false;
            Deactive();
            return true;
        }
        Vector2 mousePos = (Vector2)param;
        if (InputManager.Instance.inputMode == InputPointType.NONE)
        {
            UnitControlManager.instance.ChooseToMove(InputManager.Instance.mouseWorldPos, false);
            //BattleManager.instance.ActiveClick(InputManager.Instance.rightFirstV + (mousePos - InputManager.Instance.rightFirstV) / 2, 1);//可能是移动、进入、占领、摧毁、收集、触发、晋升等交互。向外委托
            Deactive();
            return true;
        }
        return false;
    }


}
