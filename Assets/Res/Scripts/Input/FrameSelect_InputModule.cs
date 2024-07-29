using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class FrameSelect_InputModule : InputModule
{
    public float refreshTiem = 0;
    public List<ControlFlag> controlFlagList = new List<ControlFlag>();
    public List<ControlFlag> prepareControlFlagList = new List<ControlFlag>();
    public override void Init()
    {
        mouseDic[MouseCode.LEFT_UP] = new InputResponseNode(InputLeftUp, 3);
        mouseDic[MouseCode.LEFT] = new InputResponseNode(InputLeftCheck, 3);

        mouseDic[MouseCode.RIGHT_DOWN] = new InputResponseNode(InputShield, 3);
        mouseDic[MouseCode.RIGHT] = new InputResponseNode(InputShield, 3);
        mouseDic[MouseCode.RIGHT_UP] = new InputResponseNode(InputShield, 3);

        base.Init();
    }

    bool InputLeftUp(object param)
    {

        GridMapManager.instance.SelectAllTargetArea(InputManager.Instance.leftFirstV, (Vector2)param, out controlFlagList);
        foreach (var flag in controlFlagList)
        {
            flag.GetComponent<RectTransform>().localScale = Vector3.one * ControlFlag.MAX_SIZE;
            flag.isCommand = true;
        }
        UnitControlManager.instance.ShowFlagSetOnChoose();
        foreach (var flag in controlFlagList)
        {
            flag.isCommand = false;
        }
        controlFlagList.Clear();
        UIManager.Instance.CloseSelectBox();
        Deactive();
        return true;
    }

    bool InputLeftCheck(object param)
    {

        UIManager.Instance.ShowSelectBox(InputManager.Instance.leftFirstV, (Vector2)param);
        //刷新选取框
        refreshTiem += Time.deltaTime;
        if (refreshTiem > 0.1f)
        {
            //获取框选中的
            GridMapManager.instance.SelectAllTargetArea(InputManager.Instance.leftFirstV, (Vector2)param, out controlFlagList);
            refreshTiem -= 0.1f;
            //对可选取的TroopControl进行放大

            foreach (var flag in controlFlagList)
            {
                flag.GetComponent<RectTransform>().localScale = Vector3.one * ControlFlag.MAX_SIZE;
                prepareControlFlagList.Remove(flag);
                flag.commander.SetOnChoose(true);
                flag.SetOnChoose(true);
            }
            foreach (var flag in prepareControlFlagList)
            {
                flag.GetComponent<RectTransform>().localScale = Vector3.one * ControlFlag.MIN_SIZE;
                flag.commander.SetOnChoose(false);
                flag.SetOnChoose(false);
            }
            prepareControlFlagList = controlFlagList;
        }


        return true;
    }

    bool InputShield(object param)
    {

        return true;
    }

}
