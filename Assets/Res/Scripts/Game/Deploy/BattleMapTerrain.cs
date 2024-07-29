using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleMapTerrain : MonoBehaviour
{
    public string mapTerrainPrefabPath;


    //private float minX;
    //private float maxX;
    //private float minZ;
    //private float maxZ;

    private List<Collider> coliders = new List<Collider>();

    private void Init()
    {
        var cs = GetComponentsInChildren<Collider>();
        if(cs != null)
            coliders = cs.ToList();
        //List<float> minX_l = new List<float>();
        //List<float> minZ_l = new List<float>();
        //List<float> maxX_l = new List<float>();
        //List<float> maxZ_l = new List<float>();

        //foreach (var collider in colliders)
        //{
        //    minX_l.Add(collider.bounds.min.x);
        //    minZ_l.Add(collider.bounds.min.z);
        //    maxX_l.Add(collider.bounds.max.x);
        //    maxZ_l.Add(collider.bounds.max.z);
        //}

        //minX = minX_l.Min();
        //maxX = maxX_l.Max();
        //minZ = minZ_l.Min();
        //maxZ = maxZ_l.Max();
    }

    public void GenerateTerrain(Vector3 pos = default, Quaternion rotate = default)
    {
        GameObject go = GameObjectPoolManager.Instance.Spawn("Prefab/" + mapTerrainPrefabPath, transform);
        Vector3 genPos = pos == default ? pos : transform.position;
        Quaternion genRot = rotate == default ? rotate : transform.rotation;
        go.transform.SetPositionAndRotation(pos, rotate);

        Init();
    }

    public bool IsPointInTerrain(Vector3 point)
    {
        //if(point.x > minX && point.x < maxX && point.z > minZ && point.z < maxZ) return true;
        //return false;

        foreach (var c in coliders)
        {
            if(c.bounds.Contains(point))
                return true;
        }
        return false;
    }
}
