using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleMapTerrainGenerator : MonoBehaviour
{
    //ref
    public NoiseGenerator noiseGenerator;

    [BoxGroup("PrefabGeneralSetting"), SerializeField] private string prefabPath;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private int density_p;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float posOffsetX;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float posOffsetZ;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float rotOffsetY;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float maxScaleOffset_p;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float minScaleOffset_p;
    [BoxGroup("PrefabGeneralSetting"), SerializeField, Range(0, 1)] private float clip;

    [BoxGroup("ObstacleGeneralSetting"), SerializeField] private List<string> obstaclePaths;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private int density_o;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float maxScaleOffset_o;
    [BoxGroup("PrefabGeneralSetting"), SerializeField] private float minScaleOffset_o;
    [BoxGroup("ObstacleGeneralSetting"), SerializeField, Range(0, 1)] private float minClip;
    [BoxGroup("ObstacleGeneralSetting"), SerializeField, Range(0, 1)] private float maxClip;

    [BoxGroup("TerrainGeneralSetting"), SerializeField] private float minHeight;
    [BoxGroup("TerrainGeneralSetting"), SerializeField] private float maxHeight;
    [BoxGroup("TerrainGeneralSetting"), SerializeField, Range(0, 1)] private float terrain_clip;

    private Terrain terrain;
    private TerrainCollider terrainColider;

    private List<BattleMapTerrain> mapTerrains = new List<BattleMapTerrain>();
    private List<GameObject> prefabs = new List<GameObject>();

    public static int targetSeed;
    public static int seed;
    private void Start()
    {
        terrainColider = GetComponent<TerrainCollider>();
        terrain = GetComponent<Terrain>();

        var terrains = GetComponentsInChildren<BattleMapTerrain>();
        if (terrains != null)
        {
            mapTerrains = terrains.ToList();
            foreach (var terrain in mapTerrains)
            {
                terrain.GenerateTerrain();
            }
        }
        BattleManager.instance.UpdateNavMesh();
        //GenerateTerrain();
        CoroutineManager.DelayedCoroutine(0.01f, () => StartCoroutine(GeneratePrefab()));
    }

    [Button]
    public void ReGenerate()
    {
        StartCoroutine(ReGen());
    }

    private IEnumerator ReGen()
    {
        yield return StartCoroutine(ClearPrefab());

        yield return StartCoroutine(GeneratePrefab());
    }


    private IEnumerator ClearPrefab()
    {
        int flag = 0;

        foreach (var item in prefabs)
        {
            Destroy(item);
            flag++;
            if (flag > 50)
            {
                yield return 0;
                flag = 0;
            }
        }
        prefabs.Clear();

        //foreach (var item in mapTerrains)
        //{
        //    Destroy(item.gameObject);
        //    flag++;
        //    if (flag > 50)
        //    {
        //        yield return 0;
        //        flag = 0;
        //    }
        //}

        mapTerrains.Clear();
    }


    private IEnumerator GeneratePrefab()
    {
        if (targetSeed != 0)
        {
            seed = targetSeed;
            targetSeed = 0;
        }
        else
        {
            seed = Random.Range(1, int.MaxValue);
        }
        Random.InitState(seed);
        TerrainCollider collider = GetComponent<TerrainCollider>();
        noiseGenerator.is3D = false;
        noiseGenerator.Generate();
        Texture2D texture = noiseGenerator.GetNoiseTexture() as Texture2D;

        float minX = terrainColider.bounds.min.x + 20f;
        float minZ = terrainColider.bounds.min.z + 20f;
        float maxX = terrainColider.bounds.max.x - 20f;
        float maxZ = terrainColider.bounds.max.z - 20f;

        float unitX = (maxX - minX) / texture.width;
        float unitZ = (maxZ - minZ) / texture.height;
        int flag = 0;

        //for (float i = minX; i < maxX; i += density_o)
        //{
        //    for (float j = minZ; j < maxZ; j += density_o)
        //    {
        //        int noiseX = (int)(i * unitX);
        //        int noiseY = (int)(j * unitZ);
        //        Color c = texture.GetPixel(noiseX, noiseY);
        //        Vector3 genPos = Vector3.zero;
        //        genPos.x = i + Random.Range(-posOffsetX, posOffsetX);
        //        genPos.z = j + Random.Range(-posOffsetZ, posOffsetZ);
        //        if (c.r > minClip && c.r < maxClip)
        //        {
        //            bool f = false;
        //            foreach (var t in mapTerrains)
        //            {
        //                if (t.IsPointInTerrain(genPos))
        //                {
        //                    f = true;
        //                    break;
        //                }
        //            }
        //            if (f) continue;
        //            GameObject go = GameObjectPoolManager.Instance.Spawn("Prefab/GameObject");
        //            //GameObject go = new GameObject("ObstacleTerrain");
        //            Quaternion rot = Quaternion.Euler(new Vector3(0, Random.Range(-rotOffsetY, rotOffsetY), 0));
        //            float scale = Random.Range(minScaleOffset_o, maxScaleOffset_o);
        //            go.transform.SetParent(transform);
        //            go.transform.SetPositionAndRotation(genPos, rot);
        //            go.transform.localScale = Vector3.one * scale;
        //            var terrain = go.AddComponent<BattleMapTerrain>();
        //            terrain.mapTerrainPrefabPath = obstaclePaths[Random.Range(0, obstaclePaths.Count)];
        //            terrain.GenerateTerrain(genPos, rot);
        //            mapTerrains.Add(terrain);
        //            BattleManager.instance.UpdateMiniMap();
        //            yield return null;
        //        }
        //    }
        //}

        for (float i = minX; i < maxX; i += density_p)
        {
            for (float j = minZ; j < maxZ; j += density_p)
            {
                int noiseX = (int)(i * unitX);
                int noiseY = (int)(j * unitZ);

                Color c = texture.GetPixel(noiseX, noiseY);

                Vector3 genPos = Vector3.zero;
                genPos.x = i + Random.Range(-posOffsetX, posOffsetX);
                genPos.z = j + Random.Range(-posOffsetZ, posOffsetZ);

                bool f = false;
                if (c.r > clip)
                {
                    foreach (var t in mapTerrains)
                    {
                        if (t.IsPointInTerrain(genPos))
                        {
                            f = true;
                            break;
                        }
                    }
                    if (f) continue;

                    float height = TerrainChangeClick.GetHeight(terrain, genPos);
                    genPos.y = height;
                    GameObject go = GameObjectPoolManager.Instance.Spawn("Prefab/" + prefabPath, genPos, transform);
                    Quaternion rot = Quaternion.Euler(new Vector3(0, Random.Range(-rotOffsetY, rotOffsetY), 0));
                    go.transform.SetPositionAndRotation(genPos, rot);
                    float scale = Random.Range(minScaleOffset_p, maxScaleOffset_p);
                    go.transform.localScale = Vector3.one * scale;
                    prefabs.Add(go);
                    flag++;
                }
            }
            if (flag > 50)
            {
                yield return 0;
                flag = 0;
            }
        }
        BattleManager.instance.UpdateMiniMap();
        BattleManager.instance.UpdateNavMesh();
    }



    private void GenerateTerrain()
    {
        noiseGenerator.Generate();
        Texture2D texture = noiseGenerator.GetNoiseTexture() as Texture2D;


        int r = terrain.terrainData.heightmapResolution;
        float unitX = r / texture.width;
        float unitZ = r / texture.height;

        float[,] h = terrain.terrainData.GetHeights(0, 0, r, r);

        for (int i = 1; i < r - 1; ++i)
        {
            for (int j = 1; j < r - 1; ++j)
            {
                int noiseX = (int)(i / unitX);
                int noiseY = (int)(j / unitZ);

                float pix = texture.GetPixel(noiseX, noiseY).r;
                if (pix > terrain_clip)
                {
                    int lerpX = i % (int)unitX;
                    if(lerpX != 0)
                        lerpX /= lerpX;
                    int lerpY = i % (int)unitZ;
                    if(lerpY != 0)
                        lerpY /= lerpY;

                    float f = pix + h[i + lerpX, j + lerpY] / 2;
                    
                    h[i, j] = Mathf.Lerp(minHeight, maxHeight, f);
                }
                else
                    h[i, j] = minHeight;

                //if (texture.GetPixel(noiseX, noiseY).r > terrain_clip)
                //    h[i, j] = Mathf.Lerp(minHeight, maxHeight, texture.GetPixel(noiseX, noiseY).r);
            }
        }

        terrain.terrainData.SetHeights(0, 0, h);

        BattleManager.instance.UpdateNavMesh();
    }
}
