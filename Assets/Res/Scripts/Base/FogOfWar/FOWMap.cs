using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{

    public class FOWMap
    {
        protected List<FOWTile> map = new List<FOWTile>();
        protected int mapWidth;
        protected int mapHeight;
        public Color32[] colorBuffer;
        public Color32[] blurBuffer;
        public Material blurMat;
        private Texture2D texBuffer;
        private RenderTexture renderBuffer;
        private RenderTexture renderBuffer2;
        private RenderTexture nextTexture;
        private RenderTexture curTexture;
        /// <summary>
        /// 迷雾贴图对外接口
        /// </summary>
        public Texture FogTexture
        {
            get
            {
                return curTexture;
            }
        }
        public bool fix = true;
        public Color32 this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
                {
                    return colorBuffer[x + y * mapWidth];
                }
                else
                {
                    return new Color32();
                }
            }
            set
            {
                if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
                {
                    colorBuffer[x + y * mapWidth] = value;
                }
            }
        }
        public FOWTile GetTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
            {
                return map[x + y * mapWidth];
            }
            else
            {
                return null;
            }
        }
        public int Index(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
            {
                return x + y * mapWidth;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 对计算出的迷雾贴图进行lerp缓动处理
        /// </summary>
        public void Lerp()
        {
            Graphics.Blit(curTexture, renderBuffer);
            blurMat.SetTexture("_LastTex", renderBuffer);
            Graphics.Blit(nextTexture, curTexture, blurMat, 1);
        }
        /// <summary>
        /// 将颜色信息转换为贴图并进行高斯模糊
        /// </summary>
        protected void Blur()
        {
            foreach (var tile in map)
            {
                var color = colorBuffer[Index(tile.x, tile.y)];
                if (color.r == 0)
                {
                    blurBuffer[Index(tile.x, tile.y)].a = color.b == 255 ? (byte)120 : (byte)255;

                }
                else
                {
                    blurBuffer[Index(tile.x, tile.y)].a = (byte)(255 - color.r);
                }
            }
            texBuffer.SetPixels32(blurBuffer);
            texBuffer.Apply();
            Graphics.Blit(texBuffer, renderBuffer, blurMat, 0);
            for (int i = 0; i < 1; i++)
            {
                Graphics.Blit(renderBuffer, renderBuffer2, blurMat, 0);
                Graphics.Blit(renderBuffer2, renderBuffer, blurMat, 0);
            }
            Graphics.Blit(renderBuffer, nextTexture);

        }
        public void ComputeFog(Collider2D collider, int[] pos)
        {
            int centerX = pos[0];
            int centerY = pos[1];
            int minX = (int)(collider.bounds.min.x - 0.5f);
            int minY = (int)(collider.bounds.min.y - 0.5f);
            int maxX = (int)(collider.bounds.max.x + 0.5f);
            int maxY = (int)(collider.bounds.max.y + 0.5f);
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    var tile = GetTile(centerX + i, centerY + j);
                    if (tile != null && collider.OverlapPoint(new Vector2(i, j)))
                    {
                        int index = Index(centerX + i, centerY + j);
                        if (index >= 0)
                        {
                            colorBuffer[index].r = 255;
                        }
                    }
                }
            }
            Blur();
        }


        /// <summary>
        /// 获取范围内所有障碍物区域
        /// </summary>
        public List<FOWTile> GetObstacle(int x, int y, float range)
        {
            var obs = new List<FOWTile>();
            var rangeS = (int)range * range;
            for (int i = (int)-range; i <= range; i++)
            {
                for (int j = (int)-range; j <= range; j++)
                {
                    if (i == 0 && i == j) continue;
                    if (i * i + j * j <= rangeS)
                    {
                        var tile = GetTile(x + i, y + j);
                        if (tile != null)
                        {
                            if (tile.type == 1)
                            {
                                obs.Add(tile);
                            }
                        }
                    }
                }
            }
            obs.Sort((a, b) =>
            {
                return a.Distance(x, y) - b.Distance(x, y);
            });

            return obs;
        }


        /// <summary>
        /// 刷新迷雾
        /// </summary>
        public void FreshFog()
        {
            foreach (var tile in map)
            {
                var c = colorBuffer[Index(tile.x, tile.y)];
                if (c.r == 255)
                {
                    colorBuffer[Index(tile.x, tile.y)].r = 0;
                }

            }
        }
        /// <summary>
        /// 初始化迷雾信息
        /// </summary>
        /// <param name="mapData">障碍物信息</param>
        public void InitMap(int[,] mapData)
        {
            map.Clear();
            mapWidth = mapData.GetLength(0);
            mapHeight = mapData.GetLength(1);
            colorBuffer = new Color32[mapWidth * mapHeight];
            blurBuffer = new Color32[mapWidth * mapHeight];
            //colorInfo = new Color32[mapWidth * mapHeight];
            blurMat = new Material(Shader.Find("ImageEffect/AverageBlur"));
            texBuffer = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, false);
            texBuffer.wrapMode = TextureWrapMode.Clamp;
            renderBuffer = RenderTexture.GetTemporary((int)(mapWidth * 1.5f), (int)(mapHeight * 1.5f), 0);
            renderBuffer2 = RenderTexture.GetTemporary((int)(mapWidth * 1.5f), (int)(mapHeight * 1.5f), 0);
            nextTexture = RenderTexture.GetTemporary((int)(mapWidth * 1.5f), (int)(mapHeight * 1.5f), 0);
            curTexture = RenderTexture.GetTemporary((int)(mapWidth * 1.5f), (int)(mapHeight * 1.5f), 0);
            for (int j = 0; j < mapHeight; j++)
            {
                for (int i = 0; i < mapWidth; i++)
                {

                    map.Add(new FOWTile(mapData[i, j], i, j));
                    colorBuffer[i] = new Color32(0, 0, 0, 255);
                }

            }


        }
        /// <summary>
        /// 释放缓存资源
        /// </summary>
        public void Release()
        {
            RenderTexture.ReleaseTemporary(renderBuffer);
            RenderTexture.ReleaseTemporary(renderBuffer2);
            RenderTexture.ReleaseTemporary(nextTexture);
            RenderTexture.ReleaseTemporary(curTexture);
        }
    }
    /// <summary>
    /// 战争迷雾网格信息类
    /// </summary>
    public class FOWTile
    {
        /// <summary>
        /// 1表示障碍物 0表示非障碍物
        /// </summary>
        public int type;
        public int x;
        public int y;
        public FOWTile(int type, int x, int y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }

        public int Distance(int ox, int oy)
        {
            var tx = ox - x;
            var ty = oy - y;
            return tx * tx + ty * ty;
        }

    }
    public static class FOWTool
    {
        public static bool InMap(int x, int y, int mapWidth, int mapHeight)
        {
            return (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight);

        }
        /// <summary>
        /// 计算区域是否被障碍物阻挡视野
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z">视角角度</param>
        /// <returns>是否被阻挡</returns>
        public static bool CantDisplay(int x1, int y1, int x2, int y2, float z)
        {
            if ((x1 == 0 && y1 == 0) || (x2 == 0 && y2 == 0)) return true;

            //if (x1 == 0 && y1 == 0) return true;
            if (x1 == 0 || x2 == 0)
            {
                var t = y1;
                y1 = x1;
                x1 = t;
                t = y2;
                y2 = x2;
                x2 = t;
            }
            var k1 = y1 * 1f / x1;
            var k2 = y2 * 1f / x2;
            var dot = x1 * x2 + y1 * y2;
            if (dot > 0)
            {

                return Angle(k1, k2) < (z);

            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 根据斜率计算两直线角度
        /// </summary>
        public static float Angle(float k1, float k2)
        {
            return Mathf.Abs((k2 - k1) / (1 + k1 * k2));
        }


    }

}