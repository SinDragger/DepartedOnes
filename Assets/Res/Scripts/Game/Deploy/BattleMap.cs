using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗地图
/// </summary>
public class BattleMap : MonoBehaviour
{
    [HideInInspector]
    public string targetBattleMapId;
    public Transform baseBornPoint;
    public Transform enemyBornPoint;
    public Transform enemyFleePoint;
    public Transform centerPoint;
    public Terrain mapterrain;
    public int playerBelong;
    Dictionary<int, int> belongCount = new Dictionary<int, int>();

    int nowTargetBelong = 0;
    Dictionary<TroopControl, Vector3> posDic = new Dictionary<TroopControl, Vector3>();

    public Dictionary<TroopControl, (Vector2, Vector3)> linePosDic = new Dictionary<TroopControl, (Vector2, Vector3)>();

    public Dictionary<TroopControl, Vector3> finalPosDic = new Dictionary<TroopControl, Vector3>();
    public Dictionary<General, (int, Vector3, Vector3)> generalPosDic = new Dictionary<General, (int, Vector3, Vector3)>();
    int nowLine;
    public List<TroopControl> lineStore = new List<TroopControl>();
    /// <summary>
    /// 横列间的空隙
    /// </summary>
    const float lineSpace = 14f;
    /// <summary>
    /// 部队间的空隙
    /// </summary>
    const float troopSpace = 16f;
    float[,,] alphaMap;
    private void OnEnable()
    {
        alphaMap = mapterrain.terrainData.GetAlphamaps(0, 0, mapterrain.terrainData.alphamapWidth, mapterrain.terrainData.alphamapHeight);
    }

    private void OnDisable()
    {
        mapterrain.terrainData.SetAlphamaps(0, 0, alphaMap);
    }

    public void RandomTarrian(int randomSeed = 0)
    {
        if (randomSeed != 0)
            Random.InitState(randomSeed);
        BerlinNoise.Instance.SetBerlinRandom(mapterrain);
    }

    /// <summary>
    /// 预先计算部队坐标
    /// </summary>
    /// <param name="troop"></param>
    /// <param name="belong"></param>
    /// <param name="line"></param>
    public void PreLoadDeployData(TroopControl troop, int belong, int line = 0)
    {
        nowTargetBelong = belong;
        Vector3 targetPosition = default;
        if (belong == GameManager.instance.belong)
        {
            targetPosition = baseBornPoint.transform.position;
        }
        else
        {
            targetPosition = enemyBornPoint.transform.position;
        }
        nowLine = line;
        int key = belong * 10 + line;
        if (!belongCount.ContainsKey(key))
        {
            belongCount[key] = 1;
        }
        else
        {
            belongCount[key]++;
        }
        var faceTo = (centerPoint.position - targetPosition).normalized;
        Vector3 deployPos = GetNowDeplayPosition(belongCount[key], targetPosition) - faceTo * line * lineSpace;
        posDic.Add(troop, deployPos);
        lineStore.Add(troop);
    }

    public void PreLoadDeployData(TroopControl troop, int belong, int line, float centerDistance)
    {
        nowTargetBelong = belong;
        Vector3 targetPosition = default;
        if (belong == GameManager.instance.belong)
        {
            targetPosition = baseBornPoint.transform.position;
        }
        else
        {
            targetPosition = enemyBornPoint.transform.position;
        }
        nowLine = line;
        int key = belong * 10 + line;
        if (!belongCount.ContainsKey(key))
        {
            belongCount[key] = 1;
        }
        else
        {
            belongCount[key]++;
        }
        var faceTo = (centerPoint.position - targetPosition).normalized;
        targetPosition = centerPoint.position - centerDistance * faceTo;
        Vector3 deployPos = GetNowDeplayPosition(belongCount[key], targetPosition) - faceTo * line * lineSpace;
        posDic.Add(troop, deployPos);
        lineStore.Add(troop);
    }

    public void DeployGeneral(General general, int belong, Vector3 targetPos, Vector3 forward)
    {
        generalPosDic[general] = (belong, targetPos, forward);
    }

    public void DeployGeneral(General general, int belong, int line = -1)
    {
        nowTargetBelong = belong;
        Vector3 targetPosition = default;
        if (belong == GameManager.instance.belong)
        {
            targetPosition = baseBornPoint.transform.position;
        }
        else
        {
            targetPosition = enemyBornPoint.transform.position;
        }
        var faceTo = (centerPoint.position - targetPosition).normalized;
        Vector3 targetPos = targetPosition - faceTo * line * lineSpace;
        generalPosDic[general] = (belong, targetPos, faceTo.normalized);

    }


    public void ApplyLineDeploy()
    {
        Vector3 faceTo = default;
        if (nowTargetBelong == GameManager.instance.belong)
        {
            faceTo = centerPoint.position - baseBornPoint.transform.position;
        }
        else
        {
            faceTo = centerPoint.position - enemyBornPoint.transform.position;
        }
        foreach (var pair in posDic)
        {
            Vector3 targetPos = pair.Value;
            targetPos.x -= (posDic.Count - 1) * (troopSpace / 2);
            finalPosDic[pair.Key] = targetPos;
        }
        posDic.Clear();
        if (nowTargetBelong == GameManager.instance.belong)
        {
            for (int i = 0; i < lineStore.Count; i++)
            {
                linePosDic[lineStore[i]] = (new Vector2(i - lineStore.Count / 2, -nowLine * 2 - 1), faceTo);//, faceTo
            }
        }
        else
        {
            for (int i = 0; i < lineStore.Count; i++)
            {
                linePosDic[lineStore[i]] = (new Vector2(i - lineStore.Count / 2, nowLine * 2 + 1), faceTo);//, faceTo
            }
        }
        lineStore.Clear();
    }

    public void FinalDeploy()
    {
        Vector3 faceTo = default;
        foreach (var pair in finalPosDic)
        {
            if (pair.Key.belong == GameManager.instance.belong)
            {
                faceTo = centerPoint.position - baseBornPoint.transform.position;
            }
            else
            {
                faceTo = centerPoint.position - enemyBornPoint.transform.position;
            }
            UnitControlManager.instance.DeployTroop(pair.Value, faceTo.normalized, pair.Key, pair.Key.belong, pair.Key.belong);
        }
        foreach (var pair in generalPosDic)
        {
            UnitControlManager.instance.DeployTroop(pair.Value.Item2, pair.Value.Item3, pair.Key, pair.Value.Item1, pair.Value.Item1);
        }
        //mapterrain.terrainData= mapterrain.terrainData
        belongCount.Clear();
        posDic.Clear();
        finalPosDic.Clear();
        generalPosDic.Clear();
    }


    Vector3 GetNowDeplayPosition(int flag, Vector3 source)
    {
        source.x += (flag - 1) * troopSpace;
        return source;
    }
}
