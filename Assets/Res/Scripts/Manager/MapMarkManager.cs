using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkManager : MonoSingleton<MapMarkManager>
{
    //分离出去
    public LineRoute origin;
    List<LineRoute> pool = new List<LineRoute>();
    public LineRoute SpawnLineRoute()
    {
        if (pool.Count == 0)
        {
            origin.GetComponentInChildren<Renderer>(true).sortingOrder = 3;
            var g = Instantiate(origin, transform);
            return g.GetComponent<LineRoute>();
        }
        else
        {
            LineRoute result = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            result.gameObject.SetActive(true);
            return result;
        }
    }
    public void UnspawnLineRoute(LineRoute line)
    {
        line.gameObject.SetActive(false);
        pool.Add(line);
    }

}
