using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExpend
{
    //检查x
    public static float ProjectLength(this Vector3 normal,Vector3 target)
    {
        target = Vector3.Project(target, normal);//投影向量
        float result = Vector3.Distance(Vector3.zero, target);
        if ((target.x > 0 && normal.x < 0) || (target.x < 0 && normal.x > 0)) return -result;//反方向向量
        return result;
    }
}
