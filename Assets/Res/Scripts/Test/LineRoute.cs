using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 线性路径点
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LineRoute : MonoBehaviour
{
    Path2D path;
    [Range(0.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;
    public float routeCompletePercent;
    bool isActive;
    public void InitRoutePath(Vector2[] points)
    {
        path = new Path2D(points);
        UpdateRoad();
    }

    public void SetRouteColor(Color color)
    {
        isActive = color != Color.clear;
        mat.color = color;
    }

    private void Update()
    {
        if (path == null || !isActive) return;
        UpdateRoad();
    }
    Material mat;
    public void UpdateRoad()
    {
        Vector2[] points = path.CalculateEvenlySapcedPoints(spacing);
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateRoadMesh(points, path.IsClosed, meshFilter.mesh);
        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * 0.05f);
        float result = tiling * points.Length * spacing * 0.05f;
        if (mat == null)
            mat = GetComponent<MeshRenderer>().material;
        mat.mainTextureScale = new Vector2(1, result);
    }

    Mesh CreateRoadMesh(Vector2[] points, bool isClosed, Mesh mesh)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);
        int[] tris = new int[2 * numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 forward = Vector2.zero;
            if (i < points.Length - 1 || isClosed)
            {
                forward += points[(i + 1) % points.Length] - points[i];
            }
            if (i > 0 || isClosed)
            {
                forward += points[i] - points[(i - 1 + points.Length) % points.Length];
            }
            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);
            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;
            float completionPercent = i / (float)(points.Length - 1);
            completionPercent = (1 - completionPercent);
            if (i > routeCompletePercent * points.Length)
            {
                uvs[vertIndex] = new Vector2(0, completionPercent);
                uvs[vertIndex + 1] = new Vector2(1, completionPercent);
            }
            if (i < points.Length - 1 || isClosed)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            vertIndex += 2;
            triIndex += 6;
        }
        //计算差值 补足首位
        if (mesh == null)
            mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        return mesh;
    }
}
