using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayTriangle : MonoSingleton<DelaunayTriangle>
{
    public GameObject originObject;
    //List<Transform> points = new List<Transform>();//通过层级面板放入测试点
    List<Triangle> Triangulation;

    /// <summary>
    /// 正序列化的pos
    /// </summary>
    /// <param name="pos"></param>
    public void InsertNewPoint(Vector2[] pos)
    {
        if (Triangulation == null)
        {
            Triangulation = new List<Triangle>();
        }
        Triangulation.AddRange(MathUtil.Delaun(pos));

    }
    private void OnDrawGizmos()
    {
        if (Triangulation == null) return;
        for (int i = 0; i < Triangulation.Count; i++)
        {
            Triangle triangle = Triangulation[i];
            Gizmos.DrawSphere(new Vector3(triangle.GravityPoint.x,0f, triangle.GravityPoint.y), 0.1f);
            Gizmos.DrawLine(new Vector3(triangle.vertA.x, 0f, triangle.vertA.y) + transform.position, new Vector3(triangle.vertB.x, 0f, triangle.vertB.y) + transform.position);
            Gizmos.DrawLine(new Vector3(triangle.vertB.x, 0f, triangle.vertB.y) + transform.position, new Vector3(triangle.vertC.x, 0f, triangle.vertC.y) + transform.position);
            Gizmos.DrawLine(new Vector3(triangle.vertC.x, 0f, triangle.vertC.y) + transform.position, new Vector3(triangle.vertA.x, 0f, triangle.vertA.y) + transform.position);
        }
    }
}
