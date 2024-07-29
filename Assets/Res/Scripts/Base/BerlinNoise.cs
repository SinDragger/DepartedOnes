using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerlinNoise : Singleton<BerlinNoise>
{
    public static float MIN_RANDOM = 0f;
    public static float MAX_RANDOM = 0.2f;

    public GameObject posPre;
    public GameObject itemPre;
    public Dictionary<int, Vector3> points = new Dictionary<int, Vector3>();

    /// <summary>
    /// 点位数量
    /// </summary>
    public int posNumber;
    public float[,] gridPoints;
    /// <summary>
    /// 连续性划分
    /// </summary>
    public int interIndexMax = 100;

    public void SetBerlinRandom(Terrain terrian)
    {
        Vector2 size = new Vector2(terrian.terrainData.heightmapTexture.width, terrian.terrainData.heightmapTexture.height);
        //var a = CreateGridPoints((int)size.x, (int)size.y, 40);
        MAX_RANDOM = 0.2f;
        var b = CreateGridPoints((int)size.x, (int)size.y, 60);
        //var c = CreateGridPoints((int)size.x, (int)size.y, 80);
        MAX_RANDOM = 0.4f;
        var d = CreateGridPoints((int)size.x, (int)size.y, 100);
        float[,] result = new float[(int)size.x, (int)size.y];
        //var e = CreateGridPoints((int)size.x, (int)size.y, 120);
        //terrian.terrainData.SetHeights(0, 0, result);//ArrayUtil.Combine(b, d)
        //terrian.terrainData.SetHeights(0, 0, ArrayUtil.Combine(b, d));//
    }

    public float[,] CreateGridPoints(int width, int length, int resolution)
    {
        int x = width / resolution + 2;//留冗余量
        int y = length / resolution + 2;//留冗余量
        gridPoints = new float[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float nub = RandomNumExp(MIN_RANDOM, MAX_RANDOM, 3f);
                //GameObject go = Instantiate(itemPre, transform);
                //go.transform.position = new Vector3(i + 1, nub, j + 1);
                gridPoints[i, j] = nub;
            }
        }
        float[,] result = new float[width, length];
        for (int i = 0; i < x - 1; i++)
        {
            for (int j = 0; j < y - 1; j++)
            {
                GridCoverData(ref result, i, j, resolution);
            }
        }
        return result;
        //
    }
    void GridCoverData(ref float[,] result, int x, int y, int resolution)
    {
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float interX = Mathf.Lerp(gridPoints[x, y], gridPoints[x + 1, y], InterpolationCalculation(i / (float)resolution));
                float interY = Mathf.Lerp(gridPoints[x, y + 1], gridPoints[x + 1, y + 1], InterpolationCalculation(i / (float)resolution));
                float inter = Mathf.Lerp(interX, interY, InterpolationCalculation(j / (float)resolution));
                if (x * resolution + i >= result.GetLength(0) || y * resolution + j >= result.GetLength(1))
                {
                    continue;
                }
                result[x * resolution + i, y * resolution + j] = inter;
            }
        }
    }

    float InterpolationCalculation(float num)
    {
        return 6 * Mathf.Pow(num, 5) - 15 * Mathf.Pow(num, 4) + 10 * Mathf.Pow(num, 3);
    }

    float RandomNumExp(float min, float max, float exp)
    {
        float result = Random.Range(0f, 1f);
        result = Mathf.Pow(result, exp);
        return result * (max - min) + min;

    }

}
