using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区块、可A*
/// </summary>
public class SectorBlock : AggregationEntity, ITimeUpdatable, IAstarAvaliable
{
    public SectorBlockData originData;
    public Color recognizeColor;
    /// <summary>
    /// 区块重心 用于距离位置计算
    /// </summary>
    public Vector2 gravityPoint;
    public int hideLevel;
    public List<SectorConstruction> constructions = new List<SectorConstruction>();
    public HashSet<Border> borders = new HashSet<Border>();//边界们
    public List<SectorControlProcess> progressList = new List<SectorControlProcess>();
    LinkedList<Vector2> uiPoints = new LinkedList<Vector2>();
    HashSet<Vector2[]> waitLoadPoints = new HashSet<Vector2[]>();
    public List<Triangle> tris = new List<Triangle>();
    public List<Work> searchAbleWorks = new List<Work>();
    public List<(float, Work)> ableWorks = new List<(float, Work)>();

    public int belong { private set; get; }
    //测试专用
    public bool hasGetRefugee;

    public ResourcePool pool = new ResourcePool();


    public Triangle GetTri(Vector2 clickPoint)
    {
        foreach (Triangle tri in tris)
        {
            if (GraphicUtil.IsPointInPolygon(clickPoint, tri.vector2Array))
            {
                return tri;
            }
        }
        return null;
    }

    public SectorBlock(Color useColor)
    {
        recognizeColor = useColor;
        if (GameManager.instance.fullViewTest)
        {
            playerViewPercent = 100f;
        }
    }

    public float playerViewPercent;

    public float GetProgress(int belong)
    {
        var nowProgress = progressList.Find((i) => i.belong == belong);
        if (nowProgress == null) return 0;
        return nowProgress.controlProcess;
    }

    public void SetProgress(string belongString, int percent)
    {
        int belong = int.Parse(belongString);
        var nowProgress = progressList.Find((i) => i.belong == belong);
        if (nowProgress == null)
        {
            nowProgress = new SectorControlProcess(belong);
            nowProgress.controlProcess = (float)percent;
            progressList.Add(nowProgress);
        }
    }

    /// <summary>
    /// 被施加控制
    /// </summary>
    public void LegionEnter(LegionControl legion)
    {
        var nowProgress = progressList.Find((i) => i.belong == legion.belong);
        if (nowProgress == null)
        {
            nowProgress = new SectorControlProcess(legion.belong);
            progressList.Add(nowProgress);
        }
        nowProgress.LegionStartControl(legion);
    }

    public void LegionLeave(LegionControl legion)
    {
        var nowProgress = progressList.Find((i) => i.belong == legion.belong);
        if (nowProgress == null)
        {
            return;
        }
        nowProgress.LegionLeaveControl(legion);
    }

    List<SectorControlProcess> nonLegionList = new List<SectorControlProcess>();
    public void OnUpdate(float deltaTime)
    {
        SectorControlProcess controled = null;
        nonLegionList.Clear();
        int isControlCount = 0;
        for (int i = 0; i < progressList.Count; i++)
        {
            if (progressList[i].IsControlEmpty())
            {
                nonLegionList.Add(progressList[i]);
            }
            else
            {
                isControlCount++;
                controled = progressList[i];
                if (progressList[i].belong == GameManager.instance.belong)
                {
                    progressList[i].OnUpdate(deltaTime);
                    ProcessPlayerControl(progressList[i].controlProcess);
                }
            }
        }
        //存在争夺者
        if (controled != null)
        {
            for (int i = 0; i < nonLegionList.Count; i++)
            {
                //控制度下降
                nonLegionList[i].TriggerDrop(deltaTime);
            }
        }
        if (isControlCount == 1)//唯一控制者
        {
            belong = controled.belong;
        }
        //for (int i = 0; i < progressList.Count; i++)
        //{
        //    UpdateBelong();
        //    //确定当前区域的所属
        //}
        //belong = controled.belong;

        //SectorBlockManager.Instance.SectorControlChange(this, belong, controled.belong);
        //存在其他势力所占区域
    }

    void ProcessPlayerControl(float percent)
    {
        float last = playerViewPercent;
        if (percent < playerViewPercent)
        {
            //if (playerViewPercent > 0.2f)
            //{
            //    playerViewPercent = progressList[i].controlProcess;
            //    playerViewPercent = Mathf.Clamp(playerViewPercent, 0.2f, 1f);
            //}
        }
        else
        {
            playerViewPercent = percent;
        }
        if (last > 0f && playerViewPercent <= 0f)
        {
            //失去地区地形有效信息控制，非己方军团进入隐藏状态
        }
        if (percent > 99f && !hasGetRefugee)
        {
            RefugeeManager.Instance.GetRefugee(1);
            hasGetRefugee = true;
        }
    }

    void UpdateBelong()
    {
        int result = -1;
        float minProgress = 0f;
        for (int i = 0; i < progressList.Count; i++)
        {
            if (progressList[i].controlProcess > minProgress)
            {
                minProgress = progressList[i].controlProcess;
                result = progressList[i].belong;
            }
        }
        if (belong != result)
        {
            SectorBlockManager.Instance.SectorControlChange(this, belong, result);
            belong = result;
        }
    }

    public Border FindBorderWith(SectorBlock anotherSectorBlock)
    {
        foreach (var border in borders)
        {
            if (border.MatchTargetColor(anotherSectorBlock.recognizeColor))
            {
                return border;
            }
        }
        return null;
    }

    /// <summary>
    /// 点正序列化
    /// </summary>
    public void PointsSequence()
    {
        //增加对其序列的顺序排列
        //取最高点与邻近两点
        var node = uiPoints.First;
        float nowHighest = float.MinValue;
        LinkedListNode<Vector2> highestNode = null;
        for (int i = 0; i < uiPoints.Count; i++)
        {
            if (node != null && node.Value.y > nowHighest)
            {
                nowHighest = node.Value.y;
                highestNode = node;
            }
            node = node.Next;
        }
        Vector2 middle = highestNode.Value;
        Vector2 left = default;
        Vector2 right = default;
        if (highestNode.Previous != null)
        {
            left = highestNode.Previous.Value;
        }
        else
        {
            left = uiPoints.Last.Value;
        }
        if (highestNode.Next != null)
        {
            right = highestNode.Next.Value;
        }
        else
        {
            right = uiPoints.First.Value;
        }
        if (!GraphicUtil.IsTriPointsSequence(left, middle, right))
        {
            ArrayUtil.LinkedListReverse(uiPoints);
        }
    }

    public Vector2[] ExtracBorderPoints()
    {
        Vector2[] result = new Vector2[uiPoints.Count];
        var node = uiPoints.First;
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = node.Value;
            node = node.Next;
        }
        return result;
    }

    public void CheckOverlapPoint()
    {
        if (uiPoints.First.Value.Equals(uiPoints.Last.Value))
        {
            uiPoints.RemoveLast();
        }
    }

    public bool InsertUIPoints(Vector2[] points)
    {
        if (AddUIPoints(points))
        {
            int beforeTry = waitLoadPoints.Count;
            foreach (var data in waitLoadPoints)
            {
                if (AddUIPoints(data))
                {
                    waitLoadPoints.Remove(data);
                    break;
                }
            }
            while (waitLoadPoints.Count < beforeTry)//确实消化了记录的数据
            {
                beforeTry = waitLoadPoints.Count;
                foreach (var data in waitLoadPoints)
                {
                    if (AddUIPoints(data))
                    {
                        waitLoadPoints.Remove(data);
                        break;
                    }
                }
            }
            return true;
        }
        waitLoadPoints.Add(points);//没能第一次加成功的点的归处
        return false;
    }
    bool AddUIPoints(Vector2[] points)
    {
        if (uiPoints.Count == 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                uiPoints.AddLast(points[i]);
            }
            return true;
        }
        //略过重合的点
        if (uiPoints.First.Value.Equals(points[0]))
        {
            for (int i = 1; i < points.Length; i++)
            {
                uiPoints.AddFirst(points[i]);
            }
            return true;
        }
        else if (uiPoints.First.Value.Equals(points[points.Length - 1]))
        {
            for (int i = points.Length - 2; i >= 0; i--)
            {
                uiPoints.AddFirst(points[i]);
            }
            return true;
        }
        else if (uiPoints.Last.Value.Equals(points[0]))
        {
            for (int i = 1; i < points.Length; i++)
            {
                uiPoints.AddLast(points[i]);
            }
            return true;
        }
        else if (uiPoints.Last.Value.Equals(points[points.Length - 1]))
        {
            for (int i = points.Length - 2; i >= 0; i--)
            {
                uiPoints.AddLast(points[i]);
            }
            return true;
        }
        return false;
    }

    public void SynOriginData(SectorBlockData data)
    {
        originData = data;
        hideLevel = data.hideLevel;
    }
    #region AStar
    public int Cost(IAstarAvaliable target)
    {
        return (int)Vector2.Distance(gravityPoint, (target as SectorBlock).gravityPoint);
    }

    public int Heuristic(IAstarAvaliable target)
    {
        return (int)Vector2.Distance(gravityPoint, (target as SectorBlock).gravityPoint);
    }

    public IAstarAvaliable[] Neighbours()
    {
        List<IAstarAvaliable> result = new List<IAstarAvaliable>(6);
        SectorBlock target = null;
        foreach (var border in borders)
        {
            target = SectorBlockManager.Instance.GetBlock(border.mainColor);
            if (target != null && target != this)
            {
                result.Add(target);
            }
            target = SectorBlockManager.Instance.GetBlock(border.otherColor);
            if (target != null && target != this)
            {
                result.Add(target);
            }
        }
        return result.ToArray();
    }
    #endregion

    /// <summary>
    /// 获取该区块可攻击的区块
    /// </summary>
    /// <returns></returns>
    public List<SectorBlock> GetAttableNeighbour()
    {
        List<SectorBlock> result = new List<SectorBlock>(6);
        SectorBlock target = null;
        foreach (var border in borders)
        {
            if (border.mainColor != recognizeColor)
            {
                target = SectorBlockManager.Instance.GetBlock(border.mainColor);
                if (target != null && target.belong != belong && !result.Contains(target))
                {
                    result.Add(target);
                }
            }
            if (border.otherColor != recognizeColor)
            {
                target = SectorBlockManager.Instance.GetBlock(border.otherColor);
                if (target != null && target.belong != belong && !result.Contains(target))
                {
                    result.Add(target);
                }
            }
        }
        return result;
    }

    public void AddWork(float percent, Work work)
    {
        ableWorks.Add((percent, work));
        ableWorks.Sort((a, b) => a.Item1.CompareTo(b.Item1));
    }
}


/// <summary>
/// 可复数存在
/// </summary>
public class Border
{
    public int id;
    public bool isBlackBorder => mainColor == Color.black && otherColor == Color.black;
    public Color mainColor;
    public Color otherColor;
    public GameObject borderObject;
    LinkedList<(int, int)> points = new LinkedList<(int, int)>();
    public int pointsNum => points.Count;

    public Border(Color a, Color b)
    {
        bool needSwap = false;
        if (a.CompareTo(b) == 1)
        {
            needSwap = true;
        }
        if (needSwap)
        {
            Color temp = a;
            a = b;
            b = temp;
        }
        mainColor = a;
        otherColor = b;
    }

    public bool MatchTargetColor(Color a)
    {
        if (mainColor != a && otherColor != a)
        {
            return false;
        }
        return true;
    }

    public bool MatchTargetColor(Border border)
    {
        return MatchTargetColor(border.mainColor, border.otherColor);
    }

    public bool MatchTargetColor(Color a, Color b)
    {
        if (mainColor != a && otherColor != a)
        {
            return false;
        }
        if (mainColor != b && otherColor != b)
        {
            return false;
        }
        return true;
    }

    public bool InsertPoint(int width, int height)
    {
        (int, int) point = (width, height);
        points.AddFirst(point);
        return true;
    }

    public bool InsertLastPoint(int width, int height)
    {
        (int, int) point = (width, height);
        points.AddLast(point);
        return true;
    }

    public (int, int) GetFirstPoint()
    {
        return points.First.Value;
    }

    public void PointsAction(System.Action<(int, int)> controlPointAction)
    {
        foreach (var point in points)
        {
            controlPointAction.Invoke(point);
        }
    }

    public void SetPoints(Vector3[] outPoints)
    {
        points.Clear();
        for (int i = 0; i < outPoints.Length; i++)
        {
            InsertLastPoint((int)outPoints[i].x, (int)outPoints[i].y);
        }
    }

    public Vector2[] ExtracBorderPoints()
    {
        Vector2[] result = new Vector2[points.Count];
        var node = points.First;
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new Vector2(node.Value.Item1, node.Value.Item2);
            node = node.Next;
        }
        return result;
    }

    public bool HasDoubleColor()
    {
        return mainColor != Color.black && otherColor != Color.black && mainColor != otherColor;
    }
    public Color GetAnotherColor(Color from)
    {
        if (mainColor == from) return otherColor;
        else return mainColor;
    }

    public Vector2 GetCenterPoint()
    {
        int targetFlag = points.Count / 2;
        var node = points.First;
        while (targetFlag > 0)
        {
            targetFlag--;
            node = node.Next;
        }
        return new Vector2(node.Value.Item1, node.Value.Item2);
    }

    /// <summary>
    /// 简化边界点情况
    /// </summary>
    public bool SimplfyPoints()
    {
        return false;
    }

}