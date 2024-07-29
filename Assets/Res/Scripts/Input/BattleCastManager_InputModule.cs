using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCastManager_InputModule : InputModule
{
    const float CAST_WAIT_TIME = 0.45f;
    float countDown = 0f;
    public override void Init()
    {
        mouseDic[MouseCode.RIGHT_DOWN] = new InputResponseNode(InputRightDown, 2);
        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputRight, 2);

        base.Init();
    }

    bool InputRightDown(object param)
    {
        countDown = 0;
        var controlFlag = InputManager.Instance.NowRaycastFind<ControlFlag>();
        if (controlFlag != null && UnitControlManager.instance.CommandIsInCommanders(controlFlag.commander))
        {
            string[] spellArray = controlFlag.commander.GetCastableSpells();
            var command = controlFlag.commander;//
            if (spellArray != null && spellArray.Length > 0 && command.aliveCount > 0)
            {
                BattleCastManager.instance.SetCastCommandUnit(command, spellArray, (Vector2)param);
                return true;
            }
        }

        //检测鼠标当前位置 我方 当前选择军队中 可施法的Command 与 长按成功检测的Command做对比 如果是同一个 就打开ChooseWheel

        return false;
    }
    bool InputRight(object param)
    {
        if (InputManager.Instance.inputMode == InputPointType.NONE && InputManager.Instance.mouseRightDeltaDistance < 20f)
        {
            if (countDown < CAST_WAIT_TIME && countDown + Time.deltaTime > CAST_WAIT_TIME)
            {
                //BattleCastManager.instance.indicate.gameObject.SetActive(false);
                var array = GridMapManager.instance.gridMap.GetCircleGridContain<SoldierModel>(InputManager.Instance.mouseWorldPos, 0.1f);
                if (array.Count == 0)
                {
                    return false;
                }
                SoldierModel[] trs = new SoldierModel[array.Count];
                int i = 0;
                CommandUnit command = null;
                foreach (var unit in array)
                {
                    trs[i] = (unit as SoldierModel);
                    if (i == 0)
                    {
                        command = trs[0].commander;
                    }
                    else if (command != trs[i].commander)
                    {
                        return false;
                    }
                    i++;
                }
                command = trs[0].commander;
                string[] spellArray = trs[0].commander.GetCastableSpells();
                if (spellArray != null && spellArray.Length > 0 && command.aliveCount > 0)
                {
                    BattleCastManager.instance.SetCastCommandUnit(command, spellArray, (Vector2)param);
                    return true;
                }
            }
            countDown += Time.deltaTime;
        }
        return false;
    }
}
