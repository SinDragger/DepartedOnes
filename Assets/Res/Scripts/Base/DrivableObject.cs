using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 所驱动的物体
/// </summary>
public class DrivableObject
{
    public string recyclePoolName;
    public GameObject nowObject;
    public System.Action<int> endAction;
    protected bool isCompleted = false;
    /// <summary>
    /// 物体，终点，时间
    /// </summary>
    public virtual void Init(params object[] param)
    {
        isCompleted = false;
        if (param.Length > 1)
        {
            nowObject = param[0] as GameObject;
            float distance = Vector3.Distance(nowObject.transform.position, (Vector3)param[1]);
            Vector3 startPos = nowObject.transform.position + new Vector3(0, GameConfig.RangeWeaponYFix, 0);
            Vector3 endPos = (Vector3)param[1];
            Vector3 middlePos = (startPos + endPos) / 2 + new Vector3(0, distance / 3, 0f);
            Vector3[] path =
            {
                endPos,
                middlePos,
                endPos,
            };
            nowObject.transform.LookAt(middlePos);
            nowObject.transform.DOPath(path, distance / (float)param[2], PathType.CubicBezier).SetEase(GameManager.instance.arrowCurve).SetLookAt(0).OnComplete(() =>
               {
                   isCompleted = true;
               });
        }
    }
    public virtual void UpdateMovement(float deltaTime)
    {

    }

    public virtual bool IsEnd()
    {
        return isCompleted;
    }

    public virtual void TiggerEnd(int state = 0)
    {
        nowObject.transform.DOKill();
        endAction?.Invoke(state);
    }
}
