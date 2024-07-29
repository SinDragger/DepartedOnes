using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionPatrolState : LegionState
{
    LegionControl nowLegion;
    Vector2 targetPosition;
    Vector2 startPosition;
    bool isReverse;
    List<Vector2> route = new List<Vector2>();
    int targetFlag;
    float routeLength;
    LineRoute ui;
    public LegionPatrolState(LegionControl legion, Vector2 pos)
    {
        targetFlag = 1;
        nowLegion = legion;
        startPosition = nowLegion.position;
        routeLength = 0f;
        route.Add(startPosition);
        route.Add(pos);
        targetPosition = route[targetFlag];
        routeLength += Vector2.Distance(startPosition, targetPosition);

        if (ui == null)
            ui = MapMarkManager.instance.SpawnLineRoute();
        ui.InitRoutePath(route.ToArray());
        ui.SetRouteColor(Color.yellow);
        ui.routeCompletePercent = 0f;
    }

    bool isCircle = false;
    /// <summary>
    /// 给指定循环半径则初始点算临时点，不纳入循环逻辑之中
    /// </summary>
    /// <param name="legion"></param>
    /// <param name="posArray"></param>
    public LegionPatrolState(LegionControl legion, Vector2[] posArray, int flag = 0)
    {
        targetFlag = flag;
        nowLegion = legion;
        startPosition = nowLegion.position;
        routeLength = 0f;
        route.Add(posArray[0]);
        for (int i = 1; i < posArray.Length; i++)
        {
            route.Add(posArray[i]);
            routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        }

        route.Add(posArray[0]);//首尾
        routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);

        targetPosition = route[targetFlag];
        isCircle = true;
        //if (ui == null)
        //    ui = SectorBlockManager.Instance.SpawnLineRoute();
        //ui.InitRoutePath(route.ToArray());
        //ui.SetRouteColor(Color.yellow);
        //ui.routeCompletePercent = 0f;
    }

    public void AddRoute(Vector2 newPos)
    {
        route.Add(newPos);
        routeLength += Vector2.Distance(route[route.Count - 2], route[route.Count - 1]);
        ui.InitRoutePath(route.ToArray());
    }


    public override void OnUpdate(float deltaTime)
    {
        nowLegion.position = Vector2.MoveTowards(nowLegion.position, targetPosition, nowLegion.moveSpeed * TimeManager.Instance.nowDeltaTime / 3600f);
        if (nowLegion.position == targetPosition)
        {
            //抵达目标点。执行下一个逻辑
            if (targetFlag >= route.Count - 1 || targetFlag <= 0)
            {
                if (!isCircle)
                    isReverse = !isReverse;
            }
            if (isReverse)
            {
                targetFlag--;
            }
            else
            {
                targetFlag++;
                if (isCircle)
                    targetFlag %= route.Count;

            }
            targetPosition = route[targetFlag];
        }
    }

    public override void LeaveState()
    {
        base.LeaveState();
        if (ui)
        {
            MapMarkManager.instance.UnspawnLineRoute(ui);
            ui = null;
        }
    }

    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_PATROL";
    }
}
