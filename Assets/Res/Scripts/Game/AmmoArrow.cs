using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoArrow : MonoBehaviour
{
    public BlockAbleTrigger modelTrigger;
    DrivableObject drivableObject;
    public DrivableObject Init(Vector3 startPos,Vector3 targetPos,string routeName,params object[] param)
    {
        if (drivableObject == null) drivableObject = new DrivableObject();
        drivableObject.recyclePoolName = routeName;
        transform.position = startPos + new Vector3(0, GameConfig.RangeWeaponYFix, 0);
        transform.up = startPos - targetPos;
        modelTrigger.onBlockAction = () =>
        {
            var c = BattleManager.instance.GetRelatedEntity(modelTrigger.checkTarget, 3);
            if (c != null)
            {
                BattleManager.instance.EndEntityEffect(c);
            }
        };
        modelTrigger.param = param;
        drivableObject.endAction = (state) =>
        {
            System.Action delayRemove = () =>
            {
                if (drivableObject.nowObject != null)
                    GameObjectPoolManager.Instance.Recycle(drivableObject.nowObject, drivableObject.recyclePoolName);
            };
            if (state == 0)
            {
                //CoroutineManager.RemoveCoroutine(delayRemove);
                GameObjectPoolManager.Instance.Recycle(drivableObject.nowObject, drivableObject.recyclePoolName);
            }
            else if (state == 1)
            {
                CoroutineManager.DelayedCoroutine(10f, delayRemove);
            }
        };
        return drivableObject;
    }

    /// <summary>
    /// 投射驱动
    /// </summary>
    public void DrivenProject(float speed)
    {

    }
}
