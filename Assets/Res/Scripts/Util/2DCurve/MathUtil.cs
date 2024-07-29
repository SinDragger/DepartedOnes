using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return a + (b - a) * t;
    }
    public static Vector2 QuadraticCurve(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p0 = Lerp(a, b, t);
        Vector2 p1 = Lerp(b, c, t);
        return Lerp(p0, p1, t);
    }
    public static Vector2 CubicCurve(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        Vector2 p0 = QuadraticCurve(a, b, c, t);
        Vector2 p1 = QuadraticCurve(b, c, d, t);
        return Lerp(p0, p1, t);
    }

    public static T[] AStarPathFind<T>(T startObject, T goal) where T : IAstarAvaliable
    {
        List<(int, T)> needSortList = new List<(int, T)>();
        needSortList.Add((0, startObject));
        //SortedList<int, T> frontier = new SortedList<int, T>();
        //frontier.Add(0, startObject);
        Dictionary<T, T> came_from = new Dictionary<T, T>();
        Dictionary<T, int> cost_so_far = new Dictionary<T, int>();
        came_from[startObject] = default;
        cost_so_far[startObject] = 0;

        while (needSortList.Count != 0)
        {
            var current = needSortList[0].Item2;
            needSortList.RemoveAt(0);
            if (current.Equals(goal))
            {
                break;
            }
            foreach (T next in current.Neighbours())
            {
                int new_cost = cost_so_far[current] + current.Cost(next);
                //Debug.LogError("Checked");
                //Debug.LogError(new_cost);
                //Debug.LogError(cost_so_far[next]);
                if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                {
                    cost_so_far[next] = new_cost;
                    int priority = new_cost + current.Heuristic(goal);
                    //frontier.Add(priority, next);
                    needSortList.Add((priority, next));
                    needSortList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                    came_from[next] = current;
                }
            }
        }
        List<T> resultList = new List<T>(10);
        T result = goal;
        while (result != null)
        {
            resultList.Add(result);
            result = came_from[result];
        }
        resultList.Reverse();
        return resultList.ToArray();
    }

    public static Vector2[] TriRoute(Vector2 startPoint, Triangle[] tris, Vector2 endPoint)
    {
        List<Vector2> result = new List<Vector2>();
        result.Add(startPoint);
        Vector2 nowPoint = startPoint;
        Vector2 leftPoint = default;
        Vector2 rightPoint = default;
        for (int i = 0; i < tris.Length - 1; i++)
        {
            //获得下俩个点
            Vector2[] edges = tris[i].EdgePoints(tris[i + 1]);
            //顺成左到右
            if (!GraphicUtil.IsTriPointsSequence(nowPoint, edges[0], edges[1]))
            {
                Vector2 temp = edges[0];
                edges[0] = edges[1];
                edges[1] = temp;
            }
            if (i == 0)
            {
                leftPoint = edges[0];
                rightPoint = edges[1];
            }
            else
            {
                //新点在左点 右侧——顺时针
                bool isLeftInLeftRight = GraphicUtil.IsTriPointsSequence(nowPoint, leftPoint, edges[0]);
                //新点在右点 左侧——逆时针
                bool isLeftInRightLeft = !GraphicUtil.IsTriPointsSequence(nowPoint, rightPoint, edges[0]);
                bool isRightInLeftRight = GraphicUtil.IsTriPointsSequence(nowPoint, leftPoint, edges[1]);
                bool isRightInRightLeft = !GraphicUtil.IsTriPointsSequence(nowPoint, rightPoint, edges[1]);
                //两点都在左侧
                if (!isLeftInLeftRight && !isRightInLeftRight)
                {
                    //Left点更新为新拐点
                    nowPoint = leftPoint;
                    result.Add(nowPoint);
                    continue;
                }
                if (!isLeftInRightLeft && !isRightInRightLeft)
                {
                    //Left点更新为新拐点
                    nowPoint = rightPoint;
                    result.Add(nowPoint);
                    continue;
                }

                if (isRightInLeftRight && isRightInRightLeft)//在夹角之间
                {
                    rightPoint = edges[1];
                    //确定点，步进
                }
                if (isLeftInLeftRight && isLeftInRightLeft)//在夹角之间
                {
                    leftPoint = edges[0];
                    //确定点，步进
                }

            }
        }
        result.Add(endPoint);
        return result.ToArray();
    }



    public static Vector2 MidPointOfLine(Point p1, Point p2)
    {
        return new Vector2((p1.x + p2.x) / 2, (p1.y + p2.y) / 2);
    }

    public static float GradientOfLine(Point p1, Point p2)
    {
        return (p2.y - p1.y) / (p2.x - p1.x);
    }

    public static float NegativeReciprocal(float value)
    {
        return -(1 / value);
    }

    ///<summary> Returns the intersection point of two lines given the form Ax + By = C </summary>
    public static Vector2 LineIntersection(float A1, float B1, float C1, float A2, float B2, float C2)
    {
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
        {
            throw new System.Exception("Lines are parallel");
        }

        float x = (B2 * C1 - B1 * C2) / delta;
        float y = (A1 * C2 - A2 * C1) / delta;
        return new Vector2(x, y);
    }
    private const float Margin = 3f;

    /// <summary> Generates a 'Supra/Super Triangle' which encapsulates all points held within set bounds </summary>
    public static Triangle GenerateSupraTriangle(PointBounds bounds)
    {
        float dMax = Mathf.Max(bounds.maxX - bounds.minX, bounds.maxY - bounds.minY) * Margin;
        float xCen = (bounds.minX + bounds.maxX) * 0.5f;
        float yCen = (bounds.minY + bounds.maxY) * 0.5f;

        ///The float 0.866 is an arbitrary value determined for optimum supra triangle conditions.
        float x1 = xCen - 0.866f * dMax;
        float x2 = xCen + 0.866f * dMax;
        float x3 = xCen;

        float y1 = yCen - 0.5f * dMax;
        float y2 = yCen - 0.5f * dMax;
        float y3 = yCen + dMax;

        Point pointA = new Point(x1, y1);
        Point pointB = new Point(x2, y2);
        Point pointC = new Point(x3, y3);

        return new Triangle(pointA, pointB, pointC);
    }

    /// <summary> Returns a set of bounds encolsing a point set </summary>
    public static PointBounds GetPointBounds(List<Point> points)
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float maxY = Mathf.NegativeInfinity;

        for (int i = 0; i < points.Count; i++)
        {
            Point p = points[i];
            if (minX > p.x)
            {
                minX = p.x;
            }
            if (minY > p.y)
            {
                minY = p.y;
            }
            if (maxX < p.x)
            {
                maxX = p.x;
            }
            if (maxY < p.y)
            {
                maxY = p.y;
            }
        }
        return new PointBounds(minX, minY, maxX, maxY);
    }

    /// <summary> Triangulates a set of points utilising the Bowyer Watson Delaunay technique </summary>
    public static List<Triangle> Delaun(Vector2[] points)
    {
        List<Point> pointsList = new List<Point>();
        for (int i = 0; i < points.Length; i++)
        {
            pointsList.Add(new Point(points[i].x, points[i].y));
        }
        List<Triangle> result = Delaun(pointsList);
        result.RemoveAll((t) => !t.IsWithinPolygon(points));
        //移除
        return result;
    }
    public static List<Triangle> Delaun(List<Point> points)
    {
        ///TODO - Plenty of optimizations for this algorithm to be implemented
        points = new List<Point>(points);

        //Create an empty triangle list
        List<Triangle> triangles = new List<Triangle>();

        //Generate supra triangle to ecompass all points and add it to the empty triangle list
        PointBounds bounds = GetPointBounds(points);
        Triangle supraTriangle = GenerateSupraTriangle(bounds);
        triangles.Add(supraTriangle);

        //Loop through points and carry out the triangulation
        for (int pIndex = 0; pIndex < points.Count; pIndex++)
        {
            Point p = points[pIndex];
            List<Triangle> badTriangles = new List<Triangle>();

            //Identify 'bad triangles'
            for (int triIndex = triangles.Count - 1; triIndex >= 0; triIndex--)
            {
                Triangle triangle = triangles[triIndex];

                //A 'bad triangle' is defined as a triangle who's CircumCentre contains the current point
                float dist = Vector2.Distance(p.pos, triangle.circumCentre);
                if (dist < triangle.circumRadius)
                {
                    badTriangles.Add(triangle);
                }
            }

            //Contruct a polygon from unique edges, i.e. ignoring duplicate edges inclusively
            List<Edge> polygon = new List<Edge>();
            for (int i = 0; i < badTriangles.Count; i++)
            {
                Triangle triangle = badTriangles[i];
                Edge[] edges = triangle.GetEdges();

                for (int j = 0; j < edges.Length; j++)
                {
                    bool rejectEdge = false;
                    for (int t = 0; t < badTriangles.Count; t++)
                    {
                        if (t != i && badTriangles[t].ContainsEdge(edges[j]))
                        {
                            rejectEdge = true;
                        }
                    }

                    if (!rejectEdge)
                    {
                        polygon.Add(edges[j]);
                    }
                }
            }

            //Remove bad triangles from the triangulation
            for (int i = badTriangles.Count - 1; i >= 0; i--)
            {
                triangles.Remove(badTriangles[i]);
            }

            //Create new triangles
            for (int i = 0; i < polygon.Count; i++)
            {
                Edge edge = polygon[i];
                Point pointA = new Point(p.x, p.y);
                Point pointB = new Point(edge.vertexA);
                Point pointC = new Point(edge.vertexB);
                triangles.Add(new Triangle(pointA, pointB, pointC));
            }
        }

        //Finally, remove all triangles which share verticies with the supra triangle
        for (int i = triangles.Count - 1; i >= 0; i--)
        {
            Triangle triangle = triangles[i];
            for (int j = 0; j < triangle.vertices.Length; j++)
            {
                bool removeTriangle = false;
                Point vertex = triangle.vertices[j];
                for (int s = 0; s < supraTriangle.vertices.Length; s++)
                {
                    if (vertex.EqualsPoint(supraTriangle.vertices[s]))
                    {
                        removeTriangle = true;
                        break;
                    }
                }

                if (removeTriangle)
                {
                    triangles.RemoveAt(i);
                    break;
                }
            }
        }

        return triangles;
    }

    public static Mesh CreateMeshFromTriangulation(List<Triangle> triangulation)
    {
        Mesh mesh = new Mesh();

        int vertexCount = triangulation.Count * 3;

        Vector3[] verticies = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[vertexCount];

        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int i = 0; i < triangulation.Count; i++)
        {
            Triangle triangle = triangulation[i];

            verticies[vertexIndex] = new Vector3(triangle.vertA.x, triangle.vertA.y, 0f);
            verticies[vertexIndex + 1] = new Vector3(triangle.vertB.x, triangle.vertB.y, 0f);
            verticies[vertexIndex + 2] = new Vector3(triangle.vertC.x, triangle.vertC.y, 0f);

            uvs[vertexIndex] = triangle.vertA.pos;
            uvs[vertexIndex + 1] = triangle.vertB.pos;
            uvs[vertexIndex + 2] = triangle.vertC.pos;

            triangles[triangleIndex] = vertexIndex + 2;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex;

            vertexIndex += 3;
            triangleIndex += 3;
        }

        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        return mesh;
    }
}

public interface IAstarAvaliable
{
    int Cost(IAstarAvaliable target);
    int Heuristic(IAstarAvaliable target);
    IAstarAvaliable[] Neighbours();
}