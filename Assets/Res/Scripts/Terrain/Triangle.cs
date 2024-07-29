using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Triangle : IAstarAvaliable
{
    private Vector2 CircumCentre;
    private float CircumRadius;

    public Vector2[] vector2Array => new Vector2[3] {
        vertA.pos,
        vertB.pos,
        vertC.pos
    };
    public Point[] vertices { get; } = new Point[3];
    public Point vertA { get { return vertices[0]; } }
    public Point vertB { get { return vertices[1]; } }
    public Point vertC { get { return vertices[2]; } }
    public Vector2 circumCentre { get { return CircumCentre; } }
    public float circumRadius { get { return CircumRadius; } }

    public Triangle(Point pointA, Point pointB, Point pointC)
    {
        bool isCounterClockwise = IsCounterClockwise(pointA, pointB, pointC);

        vertices[0] = pointA;
        vertices[1] = isCounterClockwise ? pointB : pointC;
        vertices[2] = isCounterClockwise ? pointC : pointB;

        CircumCentre = ComputeCircumCentre();
        CircumRadius = ComputeCircumRadius();
    }

    public Edge[] GetEdges()
    {
        return new Edge[]
        {
            new Edge(vertA, vertB),
            new Edge(vertB, vertC),
            new Edge(vertA, vertC),
        };
    }

    /// <summary> Returns true if the provided points are in counter clockwise orientation, false if clockwise </summary>
    private bool IsCounterClockwise(Point pointA, Point pointB, Point pointC)
    {
        float result = (pointB.x - pointA.x) * (pointC.y - pointA.y) - (pointC.x - pointA.x) * (pointB.y - pointA.y);
        return result > 0;
    }

    public bool ContainsEdge(Edge edge)
    {
        int sharedVerts = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].EqualsPoint(edge.vertexA) || vertices[i].EqualsPoint(edge.vertexB))
            {
                sharedVerts++;
            }
        }
        return sharedVerts == 2;
    }

    public Vector2 ComputeCircumCentre()
    {
        ///Given that all verticies on a triangle must touch the outside of the CircumCircle.
        ///We can deduce that DA = DB = DC (Distances from each vertex to the center are equal).
        ///Formulae reference - https://en.wikipedia.org/wiki/Circumscribed_circle#Circumcircle_equations

        Vector2 A = vertA.pos;
        Vector2 B = vertB.pos;
        Vector2 C = vertC.pos;
        Vector2 SqrA = new Vector2(Mathf.Pow(A.x, 2f), Mathf.Pow(A.y, 2f));
        Vector2 SqrB = new Vector2(Mathf.Pow(B.x, 2f), Mathf.Pow(B.y, 2f));
        Vector2 SqrC = new Vector2(Mathf.Pow(C.x, 2f), Mathf.Pow(C.y, 2f));

        float D = (A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) * 2f;
        float x = ((SqrA.x + SqrA.y) * (B.y - C.y) + (SqrB.x + SqrB.y) * (C.y - A.y) + (SqrC.x + SqrC.y) * (A.y - B.y)) / D;
        float y = ((SqrA.x + SqrA.y) * (C.x - B.x) + (SqrB.x + SqrB.y) * (A.x - C.x) + (SqrC.x + SqrC.y) * (B.x - A.x)) / D;
        return new Vector2(x, y);
    }

    public float ComputeCircumRadius()
    {
        //Radius is the distance from any vertex to the CircumCentre
        Vector2 circumCentre = ComputeCircumCentre();
        return Vector2.Distance(circumCentre, vertices[0].pos);
    }

    public Vector2 GravityPoint => (vertA.pos + vertB.pos + vertC.pos) / 3;

    public bool IsWithinPolygon(Vector2[] points)
    {
        //给予一个极小扰动避免恰好卡在两行线之中，端点反复运算
        return GraphicUtil.IsPointInPolygon((vertA.pos + vertB.pos + vertC.pos) / 3 + Vector2.one * 0.01f, points);
    }

    public int Cost(IAstarAvaliable target)
    {
        return (int)(Vector2.Distance(GravityPoint, (target as Triangle).GravityPoint) * 100);
    }

    public int Heuristic(IAstarAvaliable target)
    {
        return (int)(Vector2.Distance(GravityPoint, (target as Triangle).GravityPoint) * 100);
    }

    public IAstarAvaliable[] Neighbours()
    {
        return SectorBlockManager.Instance.GetTriNeighbour(this);
    }

    public Vector2[] EdgePoints(Triangle other)
    {
        int flag = 0;
        Vector2[] result = new Vector2[2];
        if (vertA.EqualsPoint(other.vertA) || vertA.EqualsPoint(other.vertB) || vertA.EqualsPoint(other.vertC)) {
            result[flag] = vertA.pos;
            flag++;
        }
        if (vertB.EqualsPoint(other.vertA) || vertB.EqualsPoint(other.vertB) || vertB.EqualsPoint(other.vertC))
        {
            result[flag] = vertA.pos;
            flag++;
        }
        if (vertC.EqualsPoint(other.vertA) || vertC.EqualsPoint(other.vertB) || vertC.EqualsPoint(other.vertC))
        {
            result[flag] = vertA.pos;
            flag++;
        }
        return result;
    }

    public bool IsNeighbour(Triangle other)
    {
        int count = 0;
        //判断边是否有两处
        if (vertA.EqualsPoint(other.vertA) || vertA.EqualsPoint(other.vertB) || vertA.EqualsPoint(other.vertC)) count++;
        if (vertB.EqualsPoint(other.vertA) || vertB.EqualsPoint(other.vertB) || vertB.EqualsPoint(other.vertC)) count++;
        if (vertC.EqualsPoint(other.vertA) || vertC.EqualsPoint(other.vertB) || vertC.EqualsPoint(other.vertC)) count++;
        return count>=2;
    }

}