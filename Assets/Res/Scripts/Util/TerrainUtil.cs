using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainUtil
{
    public static int[] GetHeightmapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.alphamapResolution);
        int y = (int)((point.z - terrain.GetPosition().z) / length * tData.alphamapResolution);
        //Debug.LogError(tData.heightmapResolution);
        //Debug.LogError(x);
        //Debug.LogError(y);
        //float x = (float)((point.x - terrain.GetPosition().x) / width * tData.heightmapResolution);
        //float y = (float)((point.z - terrain.GetPosition().z) / length * tData.heightmapResolution);
        //mapX = (int)(((point.x - terrainPosition.x) / terrainData.size.x) * heightmapWidth);
        //mapY = (int)(((point.z - terrainPosition.z) / terrainData.size.z) * heigtmapHeight);

        return new int[2] { x, y };
    }

    public enum Paint_Type
    {
        Rectangle,
        Circle,
        triangle
    }
    //public static void ChangeTexture(Terrain Ta, Vector3 Pos, int Range, int Num, Paint_Type paint_Type = Paint_Type.Rectangle)
    //{
    //    //Ta.drawInstanced = true;
    //    switch (paint_Type)
    //    {
    //        case Paint_Type.Rectangle:
    //            break;
    //        case Paint_Type.Circle:
    //            break;
    //        case Paint_Type.triangle:
    //            break;
    //        default:
    //            break;
    //    }

    //    int PosX = GetHeightmapIndex(Ta, Pos)[0] - 1;
    //    int PosY = GetHeightmapIndex(Ta, Pos)[1] - 1;
    //    int width = Range;
    //    int height = Range;

    //    var splatmapData = Ta.terrainData.GetAlphamaps(PosX, PosY, width, height);
    //    float[,,] element = new float[Range, Range, splatmapData.GetLength(2)];
    //    for (int xx = 0; xx < Range; xx++)
    //    {
    //        for (int yy = 0; yy < Range; yy++)
    //        {
    //            for (int k = 0; k < Ta.terrainData.terrainLayers.Length; k++)
    //            {
    //                splatmapData[yy, xx, k] = 0;       // 多层混合
    //            }
    //            splatmapData[yy, xx, Num] = 1f;
    //            //splatmapData[yy, xx, 0] = 1;       // 多层混合a
    //            //splatmapData[yy, xx, 1] = 0;
    //            //splatmapData[yy, xx, 2] = 0;
    //            //splatmapData[yy, xx, 3] = 0;
    //            //splatmapData[yy, xx, 4] = 0;
    //            //Todo ……
    //        }
    //    }
    //    Ta.terrainData.SetAlphamaps(PosX, PosY, splatmapData);
    //}
    public static void ChangeTexture(Terrain t, Vector3 Pos, int Range)
    {
        //int width = t.terrainData.alphamapWidth / 2;
        //int height = t.terrainData.alphamapHeight / 2;
        int PosX = GetHeightmapIndex(t, Pos)[0];
        int PosY = GetHeightmapIndex(t, Pos)[1];
        float[,,] map = t.terrainData.GetAlphamaps(PosX - Range / 2, PosY - Range / 2, Range, Range);
        //float[,,] map= new float[width, height, 5];

        int middleX = map.GetLength(1) / 2;
        int middleY = map.GetLength(0) / 2;

        // For each point on the alphamap...
        for (var y = 0; y < map.GetLength(0); y++)
        {
            for (var x = 0; x < map.GetLength(1); x++)
            {
                //// Get the normalized terrain coordinate that
                //// corresponds to the the point.
                //var normX = x * 1.0 / (t.terrainData.alphamapWidth - 1);
                //var normY = y * 1.0 / (t.terrainData.alphamapHeight - 1);

                //// Get the steepness value at the normalized coordinate.
                //var angle = t.terrainData.GetSteepness((float)normX, (float)normY);

                //// Steepness is given as an angle, 0..90 degrees. Divide
                //// by 90 to get an alpha blending value in the range 0..1.
                //float frac = (float)angle / 90.0f;
                float radius = Mathf.Sqrt((y - middleY) * (y - middleY) + (x - middleX) * (x - middleX));
                if (radius <= (float)Range / 3)
                {
                    map[x, y, 0] = 0;
                    map[x, y, 1] = 0;
                    map[x, y, 2] = 0;
                    map[x, y, 3] = 1;
                }
                else if (radius <= (float)Range / 2)
                {
                    float takePercent = ((float)Range / 2 - radius) * 6;
                    float lastTotal = 0f;
                    float[] percents = new float[3];
                    for (int i = 0; i < 3; i++)
                    {
                        lastTotal += map[x, y, i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        map[x, y, i] = map[x, y, i] / lastTotal * (1f - takePercent);
                    }
                    map[x, y, 3] = takePercent;
                }
            }
        }
        t.terrainData.SetAlphamaps(PosX - Range / 2, PosY - Range / 2, map);
    }
}

