using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gradient : BaseMeshEffect
{
    public Color32 topColor = Color.white;
    public Color32 bottomColor = Color.white;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }
        var count = vh.currentVertCount;
        if (count == 0) return;
        var vertexs = new List<UIVertex>(count);
        for (int i = 0; i < count; i++)
        {
            var vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }

        var topY = vertexs[0].position.y;
        var bottomY = vertexs[0].position.y;

        for (int i = 0; i < count; i++)
        {
            var y = vertexs[i].position.y;
            if (y > topY)
            {
                topY = y;
            } else if (y < bottomY)
            {
                bottomY = y;
            }
        }
        var height = topY - bottomY;
        for (int i = 0; i < count; i++)
        {
            var vertex = vertexs[i];
            var color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / height);
            vertex.color = color;
            vh.SetUIVertex(vertex, i);
        }
    }
}
