using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChangeClick : MonoBehaviour
{
    public Terrain self;
    public Camera target;

    private void OnEnable()
    {
        StoreOriginTerrianData();
    }
    private void OnDisable()
    {
        ResetTerrian();
    }
    int degree;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = Input.mousePosition;
            var ray = target.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.Equals(this.gameObject))
                    {
                        degree += 10;
                        ChangePosition(hit.point, degree);
                    }
                }
            }

        }
    }
    void ChangePosition(Vector3 position)
    {
        //Debug.Log("ChangeStart:" + position);
        int[] mapIndex = GetHeightmapIndex(self, position);//是起始点
        //需要进行偏移

        self.terrainData.SetHeights(mapIndex[0], mapIndex[1], GetRectRange(20, 50, 0f));//未做角度偏移
        //点阵偏移与取值
        //TODO:需要进行平滑
    }
    float[,] GetRectRange(float width, float length, float height)
    {

        float[,] result = new float[(int)((width / self.terrainData.size.x) * self.terrainData.heightmapResolution), (int)((length / self.terrainData.size.z) * self.terrainData.heightmapResolution)];
        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = height;
            }
        }
        return result;
    }

    void ChangePosition(Vector3 position, float degree)
    {
        //Debug.Log("ChangeStart:" + position);//是起始点
        //需要进行偏移
        float width = 5;
        float length = 10;
        float height = 0;//未做角度偏移


        //点阵偏移与取值
        //TODO:需要进行平滑
        //}
        //float[,] GetRectRange(float width, float length, float degree, float height)
        //{
        //if (degree == 0f) return GetRectRange(width, length, height);


        Vector2[] rounds = new Vector2[] {
                new Vector2(-width/2,-length/2),
                new Vector2(width/2,-length/2),
                new Vector2(width/2,length/2),
                new Vector2(-width/2,length/2)
            };


        float widthFix = 0;
        float lengthFix = 0;
        for (int i = 0; i < rounds.Length; i++)
        {
            rounds[i] = PointRotate(rounds[i], degree);
            if (rounds[i].x < widthFix) { widthFix = rounds[i].x; }
            if (rounds[i].y < lengthFix) { lengthFix = rounds[i].y; }
        }
        //将4个点进行修正
        for (int i = 0; i < rounds.Length; i++)
        {
            rounds[i].x -= widthFix;
            rounds[i].y -= lengthFix;
        }
        width = Mathf.Abs(rounds[0].x - rounds[2].x);
        length = Mathf.Abs(rounds[0].y - rounds[2].y);
        if (width < Mathf.Abs(rounds[1].x - rounds[3].x)) { width = Mathf.Abs(rounds[1].x - rounds[3].x); }
        if (length < Mathf.Abs(rounds[1].y - rounds[3].y)) { length = Mathf.Abs(rounds[1].y - rounds[3].y); }
        //中心点根据宽高进行左下点的校准
        position.x -= width / 2;
        position.z -= length / 2;//反了？
        int[] mapIndex = GetHeightmapIndex(self, position);
        //拿修正过后的进行平移中心点，左下点
        for (int i = 0; i < rounds.Length; i++)
        {
            rounds[i].x = rounds[i].x / self.terrainData.size.x * self.terrainData.heightmapResolution;
            rounds[i].y = rounds[i].y / self.terrainData.size.z * self.terrainData.heightmapResolution;
        }
        //拿之前中心点位置和现在中心点位置做对比出边界值
        //出现4个点，然后将4个点进行归一化平移移动并得到最大矩形情况
        //float[,] result = new float[(int)((width / self.terrainData.size.x) * self.terrainData.heightmapResolution), (int)((length / self.terrainData.size.z) * self.terrainData.heightmapResolution)];
        float[,] result = self.terrainData.GetHeights(mapIndex[0], mapIndex[1], (int)((width / self.terrainData.size.x) * self.terrainData.heightmapResolution), (int)((length / self.terrainData.size.z) * self.terrainData.heightmapResolution));

        Debug.Log(result.GetLength(0) + "/" + result.GetLength(1));

        for (int i = 0; i < rounds.Length; i++)
        {
            Debug.Log(rounds[i]);
        }
        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                if (SurfaceWithin(new Vector2(j, i), rounds))
                {
                    result[i, j] = height;
                }
                else
                {
                    //不修正布局内的
                    //result[i, j] = height + 0.05f;
                    //result[i, j] = self.terrainData.GetHeight(mapIndex[0] + i, mapIndex[1] + j);//逆反
                    //Debug.Log(result[i, j]);
                }
            }
        }

        self.terrainData.SetHeights(mapIndex[0], mapIndex[1], result);
        //return result;
    }
    (float, float) PointRotate((float, float) point, float degree)
    {
        float newX = point.Item1 * Mathf.Cos(degree) - point.Item2 * Mathf.Sin(degree);
        float newY = point.Item1 * Mathf.Sin(degree) + point.Item2 * Mathf.Cos(degree);
        point.Item1 = newX;
        point.Item2 = newY;
        return point;
    }
    Vector2 PointRotate(Vector2 point, float degree)
    {
        degree = Mathf.Deg2Rad * degree;
        float newX = point.x * Mathf.Cos(degree) - point.y * Mathf.Sin(degree);
        float newY = point.x * Mathf.Sin(degree) + point.y * Mathf.Cos(degree);
        point.x = newX;
        point.y = newY;
        return point;
    }
    //只算外包轮廓，不算内包
    bool SurfaceWithin(Vector2 point, params Vector2[] borderPoints)
    {
        if (borderPoints.Length < 3) return false;
        int crossCount = 0;
        for (int i = 0; i < borderPoints.Length - 1; i++)
        {
            if (!IsPointCanCrossLine(point, borderPoints[i], borderPoints[i + 1])) continue;//剔除不交的线段
            if (IsPointAtLineRight(point, borderPoints[i], borderPoints[i + 1])) crossCount++;
        }
        if (IsPointCanCrossLine(point, borderPoints[borderPoints.Length - 1], borderPoints[0]))
        {
            if (IsPointAtLineRight(point, borderPoints[borderPoints.Length - 1], borderPoints[0])) crossCount++;
        }
        return crossCount % 2 == 1;//奇数就交错入，没交错出
    }
    bool IsPointAtLineRight(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        bool result = ((startPoint.x - point.x) * (endPoint.y - point.y) - (endPoint.x - point.x) * (startPoint.y - point.y)) > 0;
        if (startPoint.y < endPoint.y)
        {
            result = !result;
        }
        return result;
    }
    bool IsPointCanCrossLine(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        if (startPoint.y < point.y && endPoint.y < point.y) return false;
        if (startPoint.y > point.y && endPoint.y > point.y) return false;
        return true;
    }

    public static int[] GetHeightmapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引·左下角0/0
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.heightmapResolution);
        int y = (int)((point.z - terrain.GetPosition().z) / length * tData.heightmapResolution);

        return new int[2] { x, y };
    }

    public static float GetHeight(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引·左下角0/0
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.heightmapResolution);
        int y = (int)((point.z - terrain.GetPosition().z) / length * tData.heightmapResolution);
        float result = terrain.terrainData.GetHeight(x, y);
        return result;
    }
    float[,] terrianHeightMap;
    public void StoreOriginTerrianData()
    {
        terrianHeightMap = self.terrainData.GetHeights(0, 0, self.terrainData.heightmapResolution, self.terrainData.heightmapResolution);
    }
    public void ResetTerrian()
    {
        self.terrainData.SetHeights(0, 0, terrianHeightMap);
    }
}
