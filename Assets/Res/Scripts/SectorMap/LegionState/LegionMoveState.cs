using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionMoveState : LegionState
{
    LegionControl nowLegion;
    Vector2 targetPosition;
    Vector2 startPosition;
    List<Vector2> route = new List<Vector2>();
    int targetFlag;
    float routeLength;
    LineRoute ui;
    System.Action<float> nowDistanceAction;
    public LegionMoveState(LegionControl legion, Vector2 pos, System.Action<float> nowDistanceAction, Color routeColor = default)
    {
        targetFlag = 1;
        nowLegion = legion;
        startPosition = nowLegion.position;
        routeLength = 0f;
        this.nowDistanceAction = nowDistanceAction;
        route.Add(startPosition);
        //var start = SectorBlockManager.instance.GetBlock(legion.position);
        //var end = SectorBlockManager.instance.GetBlock(pos);
        //if (start != null & end != null)
        //{
        //    var startTri = start.GetTri(legion.position);
        //    var endTri = end.GetTri(pos);
        //    if (startTri != null && endTri != null)
        //    {
        //        var triRoute = MathUtil.AStarPathFind(startTri, endTri);

        //        if (triRoute.Length > 1)
        //        {
        //            //首尾改为中间边界值
        //            route.Add((triRoute[1].GravityPoint + startPosition) / 2);
        //            routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);

        //            for (int i = 1; i < triRoute.Length - 1; i++)
        //            {
        //                route.Add(triRoute[i].GravityPoint);
        //                routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        //            }
        //            route.Add((triRoute[triRoute.Length - 1].GravityPoint + pos) / 2);
        //            routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
        //else
        //{
        //    //Debug.LogError(legion.position);
        //    //Debug.LogError(pos);
        //}
        //Debug.LogError($"{legion.position} {pos}");
        //判断起始点与终点间是否可以抵达

        route.Add(pos);
        routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        //判断起始点到目标点之间的地块如何
        //判断有向图的链接

        targetPosition = route[targetFlag];

        if (ui == null)
        {
            ui = MapMarkManager.instance.SpawnLineRoute();
        }
        ui.InitRoutePath(route.ToArray());
        ui.SetRouteColor(routeColor);
    }

    public void AddRoute(Vector2 newPos)
    {
        route.Add(newPos);
        routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        ui.InitRoutePath(route.ToArray());
    }


    public override void OnUpdate(float deltaTime)
    {
        //除以小时
        nowLegion.position = Vector2.MoveTowards(nowLegion.position, targetPosition, nowLegion.moveSpeed * TimeManager.Instance.nowDeltaTime / 3600f);
        if (nowLegion.position == targetPosition)
        {
            //抵达目标点。执行下一个逻辑
            if (targetFlag >= route.Count - 1)
            {
                //nowLegion.State = new LegionDefendState(nowLegion);
            }
            else
            {
                targetFlag++;
                targetPosition = route[targetFlag];
            }
        }
        float nowDistance = NowDistance();
        nowDistanceAction?.Invoke(routeLength - nowDistance);
        if (ui)
        {
            ui.routeCompletePercent = NowDistance() / routeLength;
        }
    }

    float NowDistance()
    {
        float leftLength = Vector2.Distance(nowLegion.position, route[targetFlag - 1]);
        for (int i = 0; i < targetFlag - 1; i++)
        {
            leftLength += Vector2.Distance(route[i], route[i + 1]);
        }
        return leftLength;
    }

    public override void LeaveState()
    {
        if (ui)
        {
            MapMarkManager.instance.UnspawnLineRoute(ui);
            ui = null;
        }
        base.LeaveState();
    }

    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_MOVE";
    }
}
