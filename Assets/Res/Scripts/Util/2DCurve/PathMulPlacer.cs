using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMulPlacer : MonoBehaviour
{
    public GameObject origin;
    public List<PathCreator> pathCreatorList = new List<PathCreator>();
    public void Init(List<Vector2[]> posList)
    {
        Debug.LogError(posList.Count);
        ArrayUtil.ListShowFit(pathCreatorList, posList, origin, transform,(t,v)=> {
            t.path = new Path2D(v);
            t.path.AutoSetControlPoints = true;
            CoroutineManager.StartFrameDelayedCoroutine(() =>
            {
                t.GetComponent<PathPlacer>().enabled = true;
            });
        });
    }
}
