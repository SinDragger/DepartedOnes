using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CastUtil
{

    public static Vector2[] Depress(this Vector3[] from)
    {
        Vector2[] result = new Vector2[from.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = from[i];
        }
        return result;
    }

    public static Vector3 V2TV3(Vector2 vector, float height)
    {
        return new Vector3(vector.x, height, vector.y);
    }


    public static float Cross(this Vector2 vector, Vector2 target)
    {
        return vector.x * target.y - target.x * vector.y;
    }

    public static string OutPutXML(object obj)
    {
        Type t = obj.GetType();
        string basicName = t.Name;
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"<{basicName}>");

        foreach (var f in t.GetFields())
        {
            stringBuilder.Append($"<{f.Name}>");
            stringBuilder.Append($"{f.GetValue(obj)}");
            stringBuilder.AppendLine($"</{f.Name}>");
            //if (f.Name.Equals(node.Name))
            //{
            //    if (TryTransferString(f.FieldType, node.InnerText, out object result))
            //    {
            //        f.SetValue(obj, result);
            //    }
            //    else
            //    {
            //        //自定义结构需要解析构建
            //        Type subNodeType = f.FieldType;
            //        if (subNodeType.IsArray)//数组类型
            //        {
            //            Type elementType = subNodeType.GetElementType();//数组的内部类型
            //            object[] subArray = new object[node.ChildNodes.Count];
            //            for (int i = 0; i < subArray.Length; i++)
            //            {
            //                object arrayObject = Activator.CreateInstance(elementType);
            //                foreach (XmlNode inSideNode in node.ChildNodes[i].ChildNodes)
            //                {
            //                    InjectData(arrayObject, elementType, inSideNode);
            //                }
            //                subArray[i] = arrayObject;
            //            }
            //            f.SetValue(obj, CreateArrayUsingReflection(elementType, subArray));
            //        }
            //        else
            //        {
            //            object subObject = Activator.CreateInstance(t);
            //            //读取其中数据
            //            foreach (XmlNode subNode in node.ChildNodes)
            //            {
            //                InjectData(subObject, t, subNode);// node.Name, node.InnerText);
            //            }
            //            f.SetValue(obj, subObject);
            //        }
            //    }
            //    extraInject = false;
            //    break;
            //}
        }

        stringBuilder.AppendLine($"</{basicName}>");
        return stringBuilder.ToString();
    }
}
