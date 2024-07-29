using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FogOfWar
{
    public class FowManager : MonoBehaviour
    {
        public float FogSizeX = 10;
        public float FogSizeY = 10;
        public float MapTileSize = 1;
        public PolygonCollider2D polyCollider;
        public FOWMap map;
        protected int[,] mapData;
        public float updateTime = 0.5f;
        public int[] GetCenter()
        {
            var x = (int)((0 - transform.position.x + FogSizeX / 2) / MapTileSize);
            var y = (int)((0 - transform.position.z + FogSizeY / 2) / MapTileSize);
            return new int[] { x, y };
        }
        public Vector3 GetV3(int[] pos)
        {
            return new Vector3(pos[0] * MapTileSize, 0, pos[1] * MapTileSize) + new Vector3(MapTileSize / 2, 0, MapTileSize / 2) + transform.position - new Vector3(FogSizeX / 2, 0, FogSizeY / 2);
        }
        public void InitMap(int[,] mapData)
        {
            map = new FOWMap();
            map.InitMap(mapData);
            this.mapData = mapData;
            NewFog();

        }
        public void NewFog()
        {
            map.FreshFog();
            map.ComputeFog(polyCollider, GetCenter());
            for (int i = 0; i < 30; i++)
            {
                map.Lerp();
            }
        }
        public void LerpFog()
        {
            map.FreshFog();
            map.ComputeFog(polyCollider, GetCenter());
            map.Lerp();
        }
        // Update is called once per frame
        void Update()
        {
            LerpFog();
            //if (count > 0)
            //{
            //    count--;
            //    LerpFog();
            //}
        }
        private void OnDestroy()
        {
            map.Release();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(FogSizeX, 0f, FogSizeY));
            if (mapData != null)
            {
                for (int i = 0; i < mapData.GetLength(0); i++)
                {
                    for (int j = 0; j < mapData.GetLength(1); j++)
                    {
                        Gizmos.color = mapData[i, j] == 1 ? Color.red : Color.white;
                        Gizmos.DrawWireCube(GetV3(new int[] { i, j }), new Vector3(MapTileSize - 0.02f, 0f, MapTileSize - 0.02f));
                    }
                }
            }


        }
    }
}
