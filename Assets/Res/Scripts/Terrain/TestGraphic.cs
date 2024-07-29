using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGraphic : Graphic
{
    // Start is called before the first frame update
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        vh.AddVert(new Vector3(0, 0), Color.red, Vector2.zero);
        vh.AddVert(new Vector3(0, 100), Color.green, Vector2.zero);
        vh.AddVert(new Vector3(100, 100), Color.black, Vector2.zero);
        vh.AddTriangle(0, 1, 2);
    }
}
