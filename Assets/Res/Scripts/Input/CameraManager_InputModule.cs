using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制器注册
/// </summary>
public class CameraManager_InputModule : InputModule
{
    public override void Init()
    {
        keyDic[InputType.CAMERA_LEFT.ToString()] = new InputResponseNode(InputLeftMove, 0);
        keyDic[InputType.CAMERA_UP.ToString()] = new InputResponseNode(InputUpMove, 0);
        keyDic[InputType.CAMERA_RIGHT.ToString()] = new InputResponseNode(InputRightMove, 0);
        keyDic[InputType.CAMERA_DOWN.ToString()] = new InputResponseNode(InputDownMove, 0);
        keyDownDic[InputType.CAMERA_ROTATE_LEFT.ToString()] = new InputResponseNode(InputRotateLeft, 0);
        keyDownDic[InputType.CAMERA_ROTATE_RIGHT.ToString()] = new InputResponseNode(InputRotateRight, 0);
        mouseDic[MouseCode.SCROLL] = new InputResponseNode(InputScroll, 0);
        mouseDic[MouseCode.MIDDLE_DOWN] = new InputResponseNode(InputMoveStart, 0);
        mouseDic[MouseCode.MIDDLE] = new InputResponseNode(InputMoving, 0);
        base.Init();
    }
    


    Vector2 prevPosition;
    bool InputMoveStart(object param)
    {
        prevPosition = (Vector2)param;
        
        return true;
    }


    bool InputMoving(object param)
    {
        Vector2 delta = prevPosition - (Vector2)param;
        prevPosition = (Vector2)param;
        CameraControl.Instance.CameraMove(delta);
        return true;
    }


    bool InputScroll(object param)
    {
        float value = (float)param;
        if (value < 0f)
        {
            CameraControl.Instance.CameraSizeChange(false);
        }
        else
        {
            CameraControl.Instance.CameraSizeChange(true);
        }
        return true;
    }
    bool InputLeftMove(object param)
    {
        CameraControl.Instance.CameraMove(new Vector2(-10, 0));
        return true;
    }
    bool InputUpMove(object param)
    {
        CameraControl.Instance.CameraMove(new Vector2(0, 10));
        return true;
    }
    bool InputRightMove(object param)
    {
        CameraControl.Instance.CameraMove(new Vector2(10, 0));
        return true;
    }
    bool InputDownMove(object param)
    {
        CameraControl.Instance.CameraMove(new Vector2(0, -10));
        return true;
    }
    bool InputRotateLeft(object param)
    {
        CameraControl.Instance.CameraRotate(true);
        return true;
    }
    bool InputRotateRight(object param)
    {
        CameraControl.Instance.CameraRotate(false);
        return true;
    }
}
