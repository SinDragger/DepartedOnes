using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 图形学计算相关工具类
/// </summary>
public static class GraphicUtil
{

    public static float SCREEN_SIZE_FIX => (float)Screen.width / 1600f;

    /// <summary>
    /// 点旋转
    /// </summary>
    public static Vector2 PointRotate(Vector2 point, float degree)
    {
        degree = Mathf.Deg2Rad * degree;
        float newX = point.x * Mathf.Cos(degree) - point.y * Mathf.Sin(degree);
        float newY = point.x * Mathf.Sin(degree) + point.y * Mathf.Cos(degree);
        point.x = newX;
        point.y = newY;
        return point;
    }

    public static Vector3 PointRotateXZ(Vector3 point, float degree)
    {
        var resultVec2 = PointRotate(new Vector2(point.x, point.z), degree);
        point.x = resultVec2.x;
        point.z = resultVec2.y;
        return point;
    }

    public static Vector2 XZ(this Vector3 point)
    {
        return new Vector2(point.x,point.z);
    }

    /// <summary>
    /// 点是否在多边形内
    /// </summary>
    public static bool IsPointInPolygon(Vector2 point, params Vector2[] borderPoints)
    {
        if (borderPoints.Length < 3) return false;
        int crossCount = 0;
        for (int i = 0; i < borderPoints.Length - 1; i++)
        {
            if (!IsPointCanCrossLine(point, borderPoints[i], borderPoints[i + 1])) continue;//剔除不交的线段
            if (IsPointAtLineRight(point, borderPoints[i], borderPoints[i + 1])) crossCount++;
        }
        if (IsPointCanCrossLine(point, borderPoints[borderPoints.Length - 1], borderPoints[0]))
        {
            if (IsPointAtLineRight(point, borderPoints[borderPoints.Length - 1], borderPoints[0])) crossCount++;
        }
        return crossCount % 2 == 1;//奇数就交错入，没交错出
    }

    /// <summary>
    /// 点是否在多边形内
    /// </summary>
    public static bool IsPointInPolygon(Vector2 point, List<Vector2> borderPoints)
    {
        if (borderPoints.Count < 3) return false;
        int crossCount = 0;
        for (int i = 0; i < borderPoints.Count - 1; i++)
        {
            if (!IsPointCanCrossLine(point, borderPoints[i], borderPoints[i + 1])) continue;//剔除不交的线段
            if (IsPointAtLineRight(point, borderPoints[i], borderPoints[i + 1])) crossCount++;
        }
        if (IsPointCanCrossLine(point, borderPoints[borderPoints.Count - 1], borderPoints[0]))
        {
            if (IsPointAtLineRight(point, borderPoints[borderPoints.Count - 1], borderPoints[0])) crossCount++;
        }
        return crossCount % 2 == 1;//奇数就交错入，没交错出
    }

    /// <summary>
    /// 多边形外部点合并多边形内部点
    /// TODO:待优化-改成链表形式合并
    /// </summary>
    public static Vector2[] MergeMulCurvedShope(List<Vector2> outer, List<Vector2>[] allInner)
    {
        List<Vector2> totalPoints = new List<Vector2>(outer.Count * 2);
        List<Vector2>[] circlePoints = new List<Vector2>[allInner.Length];
        totalPoints.AddRange(outer);
        for (int i = 0; i < allInner.Length; i++)
        {
            List<Vector2> inCirclePoints = new List<Vector2>(100);
            inCirclePoints.AddRange(allInner[i]);
            //内圈顺时针-为了逆时针插入
            PointsSequence(inCirclePoints, true);
            circlePoints[i] = inCirclePoints;
            //points点的并入
        }
        for (int i = 0; i < allInner.Length; i++)
        {
            MergeCurvedShape(totalPoints, circlePoints[i], circlePoints);
        }
        return totalPoints.ToArray();
    }

    /// <summary>
    /// 合并外曲面与内曲面的点
    /// </summary>
    public static void MergeCurvedShape(List<Vector2> outer, List<Vector2> inner, List<Vector2>[] allInner)
    {
        Vector2 outPoint;
        Vector2 inPoint;
        int inFlag = 0;
        int outFlag = 0;
        bool findComplete = false;
        for (int i = 0; i < outer.Count; i++)
        {
            outFlag = i;
            outPoint = outer[i];
            for (int j = 0; j < inner.Count; j++)
            {
                inPoint = inner[j];
                inFlag = j;
                bool isCross = false;
                //判断进行构筑的点是否
                for (int k = 0; k < allInner.Length; k++)
                {
                    if (CheckCross(allInner[k], inPoint, outPoint))
                    {
                        isCross = true;
                        break;
                    }
                }
                if (isCross)
                {
                    continue;
                }
                if (CheckCross(outer, inPoint, outPoint))
                {
                    continue;
                }
                findComplete = true;
                break;
            }
            if (findComplete)
            {
                break;
            }
        }
        outer.Insert(outFlag + 1, outer[outFlag]);
        for (int i = 0; i < inner.Count; i++)
        {
            int nowFlag = (inFlag + i) % inner.Count;
            //顺时针的反向
            outer.Insert(outFlag + 1, inner[nowFlag]);
        }
        outer.Insert(outFlag + 1, inner[inFlag]);
    }

    public static bool CheckCross(List<Vector2> points, Vector2 checkPointA, Vector2 checkPointB)
    {
        for (int i = 1; i < points.Count; i++)
        {
            if (IsTwoSegmentCross(points[i - 1], points[i], checkPointA, checkPointB))
            {
                return true;
            }
        }
        if (IsTwoSegmentCross(points[points.Count - 1], points[0], checkPointA, checkPointB))
        {
            return true;
        }
        return false;
    }

    public static bool IsTwoSegmentCross(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        //如果俩线段对于另外点都一左一右则为相交
        //不算端点在线上
        if (IsPointAtSegmentRight(pointA, pointC, pointD) * IsPointAtSegmentRight(pointB, pointC, pointD) != -1)
        {
            return false;
        }
        if (IsPointAtSegmentRight(pointC, pointA, pointB) * IsPointAtSegmentRight(pointD, pointA, pointB) != -1)
        {
            return false;
        }
        return true;
    }

    public static bool IsTwoAreaNear(Vector2[] areaA, Vector2[] areaB)
    {
        int same = 0;
        for (int i = 0; i < areaA.Length; i++)
        {
            for (int j = 0; j < areaB.Length; j++)
            {
                if (areaA[i] == areaB[j]) same++;
            }
        }
        //if (same >= 2) return true;
        return same >= 2;
    }

    //public static bool IsTwoLineWithin(Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd)
    //{

    //}

    /// <summary>
    /// 面积方位的判断
    /// <0 在内 =0 中间点多余
    /// </summary>
    public static float IsPointConcave(Vector2 pointPre, Vector2 point, Vector2 pointNext)
    {
        Vector2 a = point - pointPre;
        Vector2 b = pointNext - pointPre;
        float y = a.Cross(b);
        return y;
    }

    /// <summary>
    /// 依托最高点
    /// </summary>
    public static void PointsSequence(List<Vector2> sequence, bool isSequence = true)
    {
        //增加对其序列的顺序排列
        int flag = 0;
        //取最高点与邻近两点
        Vector2 highest = default;
        float nowHighest = float.MinValue;
        for (int i = 0; i < sequence.Count; i++)
        {
            if (sequence[i].y > nowHighest)
            {
                nowHighest = sequence[i].y;
                highest = sequence[i];
                flag = i;
            }
        }
        Vector2 left = default;
        Vector2 right = default;
        if (flag > 0)
        {
            left = sequence[flag - 1];
        }
        else
        {
            left = sequence[sequence.Count - 1];
        }
        if (flag < sequence.Count - 1)
        {
            right = sequence[flag + 1];
        }
        else
        {
            right = sequence[0];
        }
        if (isSequence)
        {
            if (!IsTriPointsSequence(left, highest, right))
            {
                sequence.Reverse();
            }
        }
        else
        {
            if (IsTriPointsSequence(left, highest, right))
            {
                sequence.Reverse();
            }
        }
    }

    /// <summary>
    /// 点是否顺时针排列
    /// </summary>
    public static bool IsTriPointsSequence(Vector2 pointA, Vector2 pointB, Vector3 pointC)
    {
        if ((pointB.x - pointA.x) * (pointC.y - pointA.y) - (pointB.y - pointA.y) * (pointC.x - pointA.x) < 0)
            return true;
        else
            return false;
    }
    /// <summary>
    /// 是否点在右侧
    /// </summary>
    public static bool IsPointAtLineRight(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        bool result = ((startPoint.x - point.x) * (endPoint.y - point.y) - (endPoint.x - point.x) * (startPoint.y - point.y)) > 0;
        if (startPoint.y < endPoint.y)
        {
            result = !result;
        }
        return result;
    }
    /// <summary>
    /// 点是否在线段一边
    /// </summary>
    public static int IsPointAtSegmentRight(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        float result = (startPoint.x - point.x) * (endPoint.y - point.y) - (endPoint.x - point.x) * (startPoint.y - point.y);
        if (result > 0)
        {
            return 1;
        }
        else if (result < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 判断是否检测线段
    /// </summary>
    public static bool IsPointCanCrossLine(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        if (startPoint.y < point.y && endPoint.y < point.y) return false;
        if (startPoint.y > point.y && endPoint.y > point.y) return false;
        return true;
    }

    /// <summary>
    /// 点是否在三角面内
    /// </summary>
    public static bool IsPointInTriangle(Vector2 checkPoint, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 ap = checkPoint - a;
        Vector2 ab = b - a;
        Vector2 bp = checkPoint - b;
        Vector2 bc = c - b;
        Vector2 cp = checkPoint - c;
        Vector2 ca = a - c;
        var npab = ap.Cross(ab);
        var npbc = bp.Cross(bc);
        var npca = cp.Cross(ca);
        return (npab * npbc) > 0 && (npab * npca) > 0 && (npbc * npca) > 0;
    }

    /// <summary>
    /// 获得多重点的中心
    /// </summary>
    public static Vector2 GetGravityPoint(params Vector2[] mPoints)
    {
        float area = 0.0f;//多边形面积
        float gx = 0.0f, gy = 0.0f;// 重心的x、y
        for (int i = 1; i <= mPoints.Length; i++)
        {
            float iX = mPoints[i % mPoints.Length].x;
            float iY = mPoints[i % mPoints.Length].y;
            float nextX = mPoints[i - 1].x;
            float nextY = mPoints[i - 1].y;
            float temp = (iX * nextY - iY * nextX) / 2.0f;
            area += temp;
            gx += temp * (iX + nextX) / 3.0f;
            gy += temp * (iY + nextY) / 3.0f;
        }
        gx = gx / area;
        gy = gy / area;
        Vector2 v2 = new Vector2(gx, gy);
        return v2;
    }

    public static float GetPointDegree(Vector2 point, Vector2 faceTo, Vector2 checkPoint)
    {
        if (faceTo == checkPoint - point) return 0;
        var v = Vector2.Dot(faceTo.normalized, (checkPoint - point).normalized);
        //然后反余玄求到夹角的弧度,这里的结果是要给-1 到 1 的弧度。
        float angle = Mathf.Acos(v);
        angle *= Mathf.Rad2Deg;//弧度转度
        if (faceTo.Cross(checkPoint - point) < 0)
        {
            angle = -angle;
        }
        return angle;
    }
}
