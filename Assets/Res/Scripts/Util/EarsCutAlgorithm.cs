using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 耳切法
/// </summary>
public class EarsCutAlgorithm
{
    public static List<Vector3> GetTriangles(List<Vector3> points)
    {
        int lenf = points.Count - 2;
        List<Vector3> Triangles = new List<Vector3>();
        int index = 0;
        for (int i = 0; i < lenf; i++)
        {
            if (index >= points.Count - 2)
            {
                if (points.Count > 3)
                {
                    i = 0;
                    index = 0;
                }
                else
                {
                    break;
                }
            }
            Vector3 a = points[index + 1] - points[index];
            Vector3 b = points[index + 2] - points[index];
            switch (GetCross(a, b))
            {
                case 0://一条线 删掉中间的点
                    if (points.Count > 3)
                        points.RemoveAt(index + 1);
                    break;
                case 1://凹三角 继续下一个
                    index++;
                    continue;
                case -1://逆时针方向  加入
                    Vector3 center = points[index];
                    Vector3 left = points[index + 1];
                    Vector3 right = points[index + 2];
                    bool isOK = true;
                    foreach (var item in points)
                    {
                        if (InTrigon(item, center, left, right))//判断不在三角形内  true 就是在三角形内
                        {
                            //Debug.Log("在三角形内");
                            isOK = false;
                            break;
                        }
                        else
                        {
                            if (GetLine(center, left, item) ||
                              GetLine(left, right, item) ||
                             GetLine(right, center, item))
                            {
                                //Debug.Log("在边上");
                                isOK = false;
                                break;
                            }
                        }
                    }
                    if (isOK)
                    {
                        Triangles.Add(center);
                        Triangles.Add(left);
                        Triangles.Add(right);
                        if (points.Count > 3)
                            points.RemoveAt(index + 1);
                    }
                    index++;
                    break;
            }
        }
        return Triangles;
    }
    /// <summary>
    /// 叉乘计算方向 判断凹凸三角形
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static int GetCross(Vector3 a, Vector3 b)
    {
        float y = Vector3.Cross(a, b).y;
        if (y == 0) return 0;
        if (y > 0) return 1;
        else return -1;
    }
    //判断是否在线上  c点是否在ab线段上
    static bool GetLine(Vector3 a, Vector3 b, Vector3 c)
    {
        if (a == c || b == c) return false;//相同的点直接返回false
        if (Vector3.Distance(a, c) > Vector3.Distance(a, b)) return false;//同一条线但是不在ab线段上 在之外的延长线 也返回false
        //两个向量的夹角  判断共线问题的话 就是如果夹角为180就是点在线段上  点c在ab线段上  ac向量和bc向量的角度 acb的角度
        Vector3 ac = a - c;
        Vector3 bc = b - c;
        double sin = ac.x * bc.z - bc.x * ac.z;
        double cos = ac.x * bc.x + ac.z * bc.z;
        return Math.Atan2(sin, cos) * (180 / Math.PI) == 180;
    }
    /// <summary>
    /// 判断target是否在center left right三个点形成的三角形范围内  在边上也是返回false
    /// </summary>
    public static bool InTrigon(Vector3 _target, Vector3 _center, Vector3 _left, Vector3 _right)
    {
        Vector3 Ctl = _left - _center;
        Vector3 Ctr = _right - _center;
        Vector3 Ctt = _target - _center;
        Vector3 Ltr = _right - _left;
        Vector3 Ltc = _right - _center;
        Vector3 Ltt = _left - _target;
        Vector3 Rtl = _left - _right;
        Vector3 Rtc = _center - _right;
        Vector3 Rtt = _target - _right;
        if (
        Vector3.Dot(Vector3.Cross(Ctl, Ctr).normalized, Vector3.Cross(Ctl, Ctt).normalized) == 1 &&
        Vector3.Dot(Vector3.Cross(Ltr, Ltc).normalized, Vector3.Cross(Ltr, Ltt).normalized) == 1 &&
        Vector3.Dot(Vector3.Cross(Rtc, Rtl).normalized, Vector3.Cross(Rtc, Rtt).normalized) == 1
        )
            return true;
        else
            return false;
    }
}