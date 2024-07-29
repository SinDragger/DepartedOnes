using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 线路径
/// </summary>
public class LineRendererRoute : MonoBehaviour
{
    private LineRenderer lr;
    private float length;
    private Material mat;
    private int id = Shader.PropertyToID("_MainTex");
    [SerializeField]
    private float density;
    Vector2 tilling;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        mat = lr.material;
    }

    void SetPositions(Vector3[] posArray)
    {
        lr.positionCount = posArray.Length;
        lr.SetPositions(posArray);
    }

    void Update()
    {
        //位置偏移
    }

    void UpdateTotalLength()
    {
        length = 0f;
        for (int i = 0; i < lr.positionCount-1; i++)
        {
            length += (lr.GetPosition(i + 1) - lr.GetPosition(i)).magnitude;
        }
        tilling = new Vector2(length * density, 0);
    }

}