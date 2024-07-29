using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//图片区域显示(可用于遮罩)
public class EditableGraphic : Graphic, IColorChangableUI
{
    public RectTransform[] points;//点位

    public bool hadCalculate;
    (int, int, int)[] calculateAreaData;
    Vector2[] pointsVec;
    /// <summary>
    /// 只适用与凸多边形
    /// </summary>
    /// <param name="vh"></param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //3个以上点时构建图形
        vh.Clear();
        //只进行一遍运算 之后使用演算完毕的数据
        //确定第一个三角形的中心点是否在图形之中
        if (!hadCalculate)
        {
            //hadCalculate = true;
            CalculateData();
        }
        if (pointsVec == null || calculateAreaData == null) return;
        for (int i = 0; i < pointsVec.Length; i++)
        {
            vh.AddVert(pointsVec[i], color, Vector2.zero);
            //vh.AddVert(pointsVec[i],Color.Lerp(color,Color.white,(float)i/(float)pointsVec.Length), Vector2.zero);
        }
        for (int i = 0; i < calculateAreaData.Length; i++)
        {
            //面积注入
            vh.AddTriangle(calculateAreaData[i].Item1, calculateAreaData[i].Item2, calculateAreaData[i].Item3);
        }
    }
    public (int, int, int)[] InsertPointsData(Vector2[] points)
    {
        pointsVec = points;
        List<Vector2> calculatePoints = new List<Vector2>(points);
        List<int> flagList = new List<int>(points.Length);//剩余点的下标列表
        for (int i = 0; i < points.Length; i++)
        {
            flagList.Add(i);
        }
        List<(int, int, int)> resultList = new List<(int, int, int)>(pointsVec.Length - 2);
        calculateAreaData = new (int, int, int)[pointsVec.Length - 2];//三角形面数是点-2
        int maxCount = 9999;
        while (calculatePoints.Count > 3 && maxCount > 0)
        {
            maxCount--;
            if (maxCount == 0)
            {
                foreach (var p in calculatePoints)
                {
                    Debug.LogError(p);
                }
                Debug.LogError("MaxOut");
            }
            for (int i = 0; i < calculatePoints.Count; i++)
            {
                int flag = i;
                int middle = flag;
                int left = middle - 1;
                if (left < 0) left = calculatePoints.Count - 1;
                int preLeft = left - 1;
                if (preLeft < 0) preLeft = calculatePoints.Count - 1;
                int right = middle + 1;
                if (right >= flagList.Count) right = 0;
                int nextRight = right + 1;
                if (nextRight >= flagList.Count) nextRight = 0;
                //判断其他点是否在当前三点内
                //判断当前点是否在图形内
                //if (!GraphicUtil.IsPointConcave(pointsVec[flagList[i]], calculatePoints))//非凸点
                //{
                //    continue;
                //}
                Vector2 pointLeft = pointsVec[flagList[left]];
                Vector2 pointMiddle = pointsVec[flagList[middle]];
                Vector2 pointRight = pointsVec[flagList[right]];
                Vector2 pointPreLeft = pointsVec[flagList[preLeft]];
                Vector2 pointNextRight = pointsVec[flagList[nextRight]];
                float paramY = GraphicUtil.IsPointConcave(pointLeft, pointMiddle, pointRight);
                if (paramY > 0)//非凸点
                {
                    //Debug.LogError("Next");
                    continue;
                }
                if (paramY == 0)
                {
                    calculatePoints.RemoveAt(flag);
                    flagList.RemoveAt(flag);
                    break;
                }
                bool hasConflict = false;
                foreach (var point in points)
                {
                    if (point == pointLeft) continue;
                    if (point == pointMiddle) continue;
                    if (point == pointRight) continue;
                    if (GraphicUtil.IsPointInTriangle(point, pointLeft, pointMiddle, pointRight))
                    {
                        hasConflict = true;
                        //Debug.LogError("Next");
                        break;
                    }
                }
                if (hasConflict) continue;//存在点在刨分三角形之中

                resultList.Add((flagList[left], flagList[middle], flagList[right]));
                //移除邻近共线点 剔除无效三角面
                if (GraphicUtil.IsPointConcave(pointLeft, pointRight, pointNextRight) == 0f)
                {
                    calculatePoints.RemoveAt(right);
                    flagList.RemoveAt(right);
                    if (flag > right)
                    {
                        flag--;
                    }
                    if (left > right)
                    {
                        left--;
                    }
                }
                calculatePoints.RemoveAt(flag);
                flagList.RemoveAt(flag);
                if (GraphicUtil.IsPointConcave(pointPreLeft, pointLeft, pointRight) == 0f)
                {
                    if (left > flag) left--;
                    calculatePoints.RemoveAt(left);
                    flagList.RemoveAt(left);
                }
                break;
            }

        }
        if (maxCount != 0)
            resultList.Add((flagList[0], flagList[1], flagList[2]));
        //最后加上
        calculateAreaData = resultList.ToArray();
        hadCalculate = true;
        return calculateAreaData;
    }
    void CalculateData()
    {
        if (points != null && points.Length > 2)
        {
            pointsVec = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                pointsVec[i] = points[i].anchoredPosition;
            }
        }
        if (pointsVec == null || pointsVec.Length == 0) return;
        int flag = 0;
        Vector2 firstTriPoint = default;
        while (firstTriPoint == default)
        {
            firstTriPoint = (pointsVec[flag] + pointsVec[flag + 1] + pointsVec[flag + 2]) / 3;
            flag++;
            if (GraphicUtil.IsPointInPolygon(firstTriPoint, pointsVec))
            {
                break;
            }
            else
            {
                firstTriPoint = default;
            }
        }
        int leftPoint = flag - 1;
        int rightPoint = flag + 1;
        calculateAreaData = new (int, int, int)[pointsVec.Length - 2];//三角形面数是点-2
        int arrayFlag = 0;
        int newPoint = flag;
        calculateAreaData[arrayFlag] = (leftPoint, rightPoint, newPoint);
        for (int i = 1; i < calculateAreaData.Length; i++)
        {
            arrayFlag++;
            //计算newPoint
            //先左再右
            int leftNext = leftPoint - 1;
            if (leftNext < 0) leftNext = pointsVec.Length - 1;
            int rightNext = rightPoint + 1;
            if (rightNext >= pointsVec.Length) rightNext = 0;
            //检测左前进点是否会包含右前进点
            if (!GraphicUtil.IsPointInTriangle(pointsVec[rightNext], pointsVec[rightPoint], pointsVec[leftPoint], pointsVec[leftNext]))
            {
                //使用左边点延申三角形
                newPoint = leftPoint;
                leftPoint = leftNext;
            }
            else//使用右边点
            {
                newPoint = rightPoint;
                rightPoint = rightNext;
            }
            calculateAreaData[arrayFlag] = (leftPoint, rightPoint, newPoint);
        }
    }

    public bool IsPointInTriangle(Vector2 checkPoint, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 ap = checkPoint - a;
        Vector2 ab = b - a;
        Vector2 bp = checkPoint - b;
        Vector2 bc = c - b;
        Vector2 cp = checkPoint - c;
        Vector2 ca = a - c;

        var npab = Vector3.Cross(ap, ab).y;
        var npbc = Vector3.Cross(bp, bc).y;
        var npca = Vector3.Cross(cp, ca).y;
        return (npab * npbc) > 0 && (npab * npca) > 0;
    }


    private void Update()
    {
        SetAllDirty();
    }

    public void ChangeColorTo(Color value)
    {
        color = value;
    }
}
