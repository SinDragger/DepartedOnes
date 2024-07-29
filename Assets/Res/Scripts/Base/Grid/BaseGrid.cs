using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseGrid
{
    public int belong;
    public int x, y;
    Dictionary<System.Type, HashSet<object>> storage = new Dictionary<System.Type, HashSet<object>>();

    public HashSet<object> GetGridContain<T>()
    {
        System.Type t = typeof(T);
        if (!storage.ContainsKey(t))
        {
            storage[t] = new HashSet<object>();
        }
        return storage[t];
    }

    public void AddGridContain<T>(object value)
    {
        System.Type t = typeof(T);
        if (!storage.ContainsKey(t))
        {
            storage.Add(t, new HashSet<object>());
        }
        storage[t].Add(value);
    }

    public void RemoveGridContain<T>(object value)
    {
        System.Type t = typeof(T);
        if (storage.ContainsKey(t))
        {
            storage[t].Remove(value);
        }
    }
    public void ResetData()
    {
        belong = 0;
        foreach(var pair in storage)
        {
            pair.Value.Clear();
        }
    }
    public Vector2 GetPositionPoint(GridPosition postionType)
    {
        switch (postionType)
        {
            case GridPosition.EAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * (y + 0.5f));
            case GridPosition.WEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * (y + 0.5f));
            case GridPosition.NORTH:
                return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 1f));
            case GridPosition.SOUTH:
                return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 0f));
            case GridPosition.SOUTHWEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * y);
            case GridPosition.NORTHWEST:
                return new Vector2(GridMapManager.gridLength * x, GridMapManager.gridLength * (y + 1f));
            case GridPosition.NORTHEAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * (y + 1f));
            case GridPosition.SOUTHEAST:
                return new Vector2(GridMapManager.gridLength * (x + 1f), GridMapManager.gridLength * y);
        }
        return new Vector2(GridMapManager.gridLength * (x + 0.5f), GridMapManager.gridLength * (y + 0.5f));
    }
    Vector2 tempLoad;
    public float GetTargetPositionDistance(GridPosition positionType,Vector2 origin)
    {
        switch (positionType)
        {
            case GridPosition.EAST:
                tempLoad.x = GridMapManager.gridLength * (x + 1f);
                tempLoad.y = GridMapManager.gridLength * (y + 0.5f);
                break;
            case GridPosition.WEST:
                tempLoad.x = GridMapManager.gridLength * x;
                tempLoad.y = GridMapManager.gridLength * (y + 0.5f);
                break;
            case GridPosition.NORTH:
                tempLoad.x = GridMapManager.gridLength * (x + 0.5f);
                tempLoad.y = GridMapManager.gridLength * (y + 1f);
                break;
            case GridPosition.SOUTH:
                tempLoad.x = GridMapManager.gridLength * (x + 0.5f);
                tempLoad.y = GridMapManager.gridLength * y;
                break;
            case GridPosition.SOUTHWEST:
                tempLoad.x = GridMapManager.gridLength * x;
                tempLoad.y = GridMapManager.gridLength * y;
                break;
            case GridPosition.NORTHWEST:
                tempLoad.x = GridMapManager.gridLength * x;
                tempLoad.y = GridMapManager.gridLength * (y + 1f);
                break;
            case GridPosition.NORTHEAST:
                tempLoad.x = GridMapManager.gridLength * (x + 1f);
                tempLoad.y = GridMapManager.gridLength * (y + 1f);
                break;
            case GridPosition.SOUTHEAST:
                tempLoad.x = GridMapManager.gridLength * (x + 1f);
                tempLoad.y = GridMapManager.gridLength * y;
                break;
        }
        return Vector2.Distance(tempLoad, origin);
    }
}
