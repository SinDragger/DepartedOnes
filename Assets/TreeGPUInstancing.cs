using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeGPUInstancing : MonoBehaviour
{
    public Material mat;
    public Mesh mesh;
    public int count;
    public Vector3 maxPos;
    private List<ObjData> batch = new List<ObjData>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var obj = new ObjData()
            {
                pos = new Vector3(
                    Random.Range(-maxPos.x, maxPos.x),
                    Random.Range(-maxPos.x, maxPos.x),
                    Random.Range(-maxPos.x, maxPos.x)
                    ),
                rot = Quaternion.identity,
                scale = Vector3.one * 2,
                clr = Random.ColorHSV(),
            };
            obj.mtrl = Instantiate<Material>(mat);
            obj.mtrl.SetColor("_Color", obj.clr);
            batch.Add(obj);
        };
    }

    // Update is called once per frame
    void Update()
    {
        DrawGPUInstancing();
    }
    List<ObjData> curBatch = new List<ObjData>();
    void DrawGPUInstancing()
    {
        for (int i = 0; i < batch.Count; i++)
        {
            var obj = batch[i];
            curBatch.Add(obj);
            if (curBatch.Count == 1023 || i == batch.Count - 1)
            {
                Graphics.DrawMeshInstanced(mesh, 0, mat, curBatch.Select(x => x.matrix).ToList());
                curBatch.Clear();
            }
        }
    }
}
public class ObjData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Color clr;
    public Material mtrl;
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }
}