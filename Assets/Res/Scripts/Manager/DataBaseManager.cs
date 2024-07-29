using System;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using UnityEditor;
/// <summary>
/// 内存层面 唯一性数据表管理。
/// </summary>
public partial class DataBaseManager : Singleton<DataBaseManager>
{
    /// <summary>
    /// 实际数据内容实体
    /// </summary>
    Dictionary<string, List<AggregationEntity>> dataSerialDics = new Dictionary<string, List<AggregationEntity>>();
    /// <summary>
    /// 列表临时存储
    /// </summary>
    Dictionary<string, object> listTempStore = new Dictionary<string, object>();
    /// <summary>
    /// 资源的临时存储
    /// </summary>
    Dictionary<string, object> resTempStore = new Dictionary<string, object>();
    /// <summary>
    /// Sprite的临时存储-防Texture重名
    /// </summary>
    Dictionary<string, Sprite> spriteTempStore = new Dictionary<string, Sprite>();
    /// <summary>
    /// 路径的path的存储
    /// </summary>
    Dictionary<string, string> idNameToDicPath = new Dictionary<string, string>();
    /// <summary>
    ///  目前时陈翔使用用于通过prefab名获取对应的prefab
    /// </summary>
    public Dictionary<string, string> configMap = new Dictionary<string, string>();

    Dictionary<string, object> runningTempDataStore = new Dictionary<string, object>();

    string outputDirectory = "Assets/StreamingAssets/Core_DeparterOnes/";
    /// <summary>
    /// Mod加载对应的XML文件夹
    /// </summary>
    string[] modLoadFiles =
    {
        "EquipAbleModelData",//模型数据
        "EquipData",//装备数据
        "EquipSetData",//兵种套装
        "Species",//兵源物种
        "SubSpecies",//生物细分
        "UnitData",//单位数据
        "ForceData",//势力数据
        "TipText",//描述介绍文件
        
        "Construction",//建筑构造
        "Work",//运转岗位
        "GeneralData",//将领数据

        "Resource",//资源种类
        "Landform",//地形
        "LegionData",//军团
        "SkillData", //TODO 技能数据
        "CastableSpellData",//法术数据
        "StandardStatus",
        //SectorMapData:区域地图

        "RogueData",//肉鸽数据
        "QuestData",//任务数据

        "BattleMapData",//战斗地图数据
    };
    /// <summary>
    /// Mod加载对应载入的资源文件夹
    /// </summary>
    string[] modLoadTextureDics =
    {
        "Constraction",//建筑构造
        "Resource",//资源图标
        "Work",//工作图标
        "Equip",//装备图标
        //TODO 技能图标
    };

    public T GetTempData<T>(string idName, T defaultValue = default)
    {
        if (runningTempDataStore.ContainsKey(idName))
            return (T)runningTempDataStore[idName];
        return defaultValue;
    }

    public void SetTempData<T>(string idName, T value)
    {
        runningTempDataStore[idName] = value;
    }

    public T GetIdNameDataFromList<T>(string idName) where T : AggregationEntity
    {
        return GetTargetDataList<T>().Find((item) => item.idName.Equals(idName));
    }

    public void AddNewIdNameDataToList<T>(string idName, T target) where T : AggregationEntity
    {
        target.idName = idName;
        GetTargetDataList<T>().Add(target);
    }

    public List<T> GetTargetDataList<T>() where T : AggregationEntity
    {
        string typeName = typeof(T).Name;
        if (listTempStore.ContainsKey(typeName))
        {
            return listTempStore[typeName] as List<T>;//转换数组返回
        }
        if (dataSerialDics.ContainsKey(typeName))
        {
            object result = dataSerialDics[typeName].Cast<T>().ToList();
            listTempStore.Add(typeName, result);
            return result as List<T>;//转换数组返回
        }
        return null;
    }

    /// <summary>
    /// 对于硬盘路径资源的加载
    /// </summary>
    public T ResourceLoad<T>(string path) where T : UnityEngine.Object
    {
        if (resTempStore.ContainsKey(path)) return resTempStore[path] as T;
        resTempStore[path] = Resources.Load<T>(path);
        return resTempStore[path] as T;
    }

    public Texture2D LoadMapTexture(string mapName)
    {
        if (!idNameToDicPath.ContainsKey(mapName)) return null;
        return LoadTextureByPath(idNameToDicPath[mapName]);
    }

    public Texture2D LoadTextureByPath(string path)
    {
        if (resTempStore.ContainsKey(path)) return resTempStore[path] as Texture2D;
        //创建文件长度缓冲区
        byte[] _bytes = File.ReadAllBytes(path);
        //创建Texture
        Texture2D _texture2D = new Texture2D(1, 1);
        _texture2D.LoadImage(_bytes);
        _texture2D.wrapMode = TextureWrapMode.Clamp;
        resTempStore[path] = _texture2D;
        return _texture2D;
    }

    protected override void Init()
    {
        //加载目标基础
        LoadBasicInit();
    }

    private string GetCorePath()
    {
        return Application.streamingAssetsPath + "/Core_DeparterOnes";
    }

    /// <summary>
    /// 测试加载
    /// </summary>
    public void LoadBasicInit()
    {
        string corePath = Application.streamingAssetsPath + "/Core_DeparterOnes";
        LoadModFromDir(corePath);
        //测试输出地图数据的加载
        var l = GetTargetDataList<EquipSetData>();
    }

    [Obsolete]
    public List<string> LoadStoryTest(string fileName)
    {

        string corePath = Application.streamingAssetsPath + "/Core_DeparterOnes/StoryTexts";
        DirectoryInfo info = new DirectoryInfo(corePath);
        if (!info.Exists) return null;

        var result = info.GetFiles();
        List<string> res = new List<string>();
        foreach (var file in result)
        {
#if UNITY_EDITOR
            if (file.Name.EndsWith("meta")) continue;
#endif
            if (file.Name.StartsWith(fileName))
            {

                var configData = File.ReadAllText(file.FullName);
                // if (string.IsNullOrEmpty(configData)) continue;
                string line;

                using (StringReader reader = new StringReader(configData))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        res.Add(line);
                    }
                }//当程序退出using代码块时自动调用 reader.Dispose()方法
            }
        }


        return res;
    }
    [Obsolete]
    public List<string> LoadDialogBubbleTest(string fileName)
    {
        string corePath = Application.streamingAssetsPath + "/Core_DeparterOnes/StoryTexts";
        DirectoryInfo info = new DirectoryInfo(corePath);
        if (!info.Exists) return null;
        var result = info.GetFiles();
        List<string> res = new List<string>();
        foreach (var file in result)
        {
#if UNITY_EDITOR
            if (file.Name.EndsWith("meta")) continue;
#endif
            if (file.Name.StartsWith(fileName))
            {

                var configData = File.ReadAllText(file.FullName);
                // if (string.IsNullOrEmpty(configData)) continue;
                string line;

                using (StringReader reader = new StringReader(configData))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        res.Add(line);
                    }
                }//当程序退出using代码块时自动调用 reader.Dispose()方法
            }
        }
        return res;
    }



    string loadPath;
    /// <summary>
    /// 对目标Mod文件夹进行数据导入
    /// </summary>
    public void LoadModFromDir(string dirPath)
    {
        loadPath = dirPath;
        DirectoryInfo info = new DirectoryInfo(dirPath);
        if (!info.Exists) return;
        var result = info.GetFiles();
        foreach (var file in result)
        {
#if UNITY_EDITOR
            if (file.Name.EndsWith("meta")) continue;
#endif
            foreach (var initTarget in modLoadFiles)
            {
                if (file.Name.StartsWith(initTarget))
                {

                    LoadTargetXML(initTarget, File.ReadAllText(file.FullName));
                    //对可读取目标进行了读取
                }
            }
            if (file.Name.StartsWith("ConfigMap"))
            {
                var configData = File.ReadAllText(file.FullName);
                if (string.IsNullOrEmpty(configData)) continue;
                string line;
                using (StringReader reader = new StringReader(configData))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] keyValue = line.Split('=');
                        if (keyValue.Length >= 2)
                            configMap[keyValue[0]] = keyValue[1];
                    }
                }//当程序退出using代码块时自动调用 reader.Dispose()方法
            }
        }
        //载入目标Map数据与相关图片
        DirectoryInfo mapDirInfo = new DirectoryInfo(dirPath + "/Map");
        if (mapDirInfo.Exists)
        {
            foreach (var file in mapDirInfo.GetFiles())
            {
#if UNITY_EDITOR
                if (file.Name.EndsWith("meta")) continue;
#endif
                if (file.Name.EndsWith(".png"))
                {
                    string mapName = file.Name.Replace(".png", "");
                    idNameToDicPath[mapName] = file.FullName;
                }
                else if (file.Name.EndsWith(".xml"))
                {
                    //进行Load
                    LoadTargetXML("SectorBlockMap", File.ReadAllText(file.FullName));
                }
            }
        }
        //预载入目标的资源相关图片的路径-用于之后载入
        LoadTargetDirIntoPathDic(dirPath + "/Texture");
        foreach (var item in modLoadTextureDics)
        {
            LoadTargetDirIntoPathDic(dirPath + $"/Texture/{item}");
        }
#if UNITY_EDITOR
        SystemSettingManager.Instance.language = "English";
#endif
        string localizeDirPath = dirPath + $"/Languages/{SystemSettingManager.Instance.language}";
        if (Directory.Exists(localizeDirPath))
        {
            DirectoryInfo localizeDirInfo = new DirectoryInfo(localizeDirPath);
            foreach (var file in localizeDirInfo.GetFiles())
            {
                foreach (var initTarget in modLoadFiles)
                {
#if UNITY_EDITOR
                    if (file.Name.EndsWith("meta")) continue;
#endif
                    if (file.Name.Contains(initTarget))
                    {
                        LoadTargetXML(initTarget, File.ReadAllText(file.FullName));
                        //对可读取目标进行了读取
                    }
                }
            }
        }

    }
    /// <summary>
    /// 加载目标文件夹的图片进Res
    /// </summary>
    void LoadTargetDirIntoPathDic(string dirPath)
    {
        DirectoryInfo mapDirInfo = new DirectoryInfo(dirPath);
        if (mapDirInfo.Exists)
        {
            foreach (var file in mapDirInfo.GetFiles())
            {
                if (file.Name.EndsWith(".png"))
                {
                    string resName = file.Name.Replace(".png", "");
                    if (idNameToDicPath.ContainsKey(resName))
                    {
                        DebugManager.LogError($"{resName} Is Duplicate");
                    }
                    else
                    {
                        idNameToDicPath[resName] = file.FullName;
                    }
                }
            }
        }

    }


    /// <summary>
    /// 加载目标XML
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <param name="context">实际数据内容</param>
    void LoadTargetXML(string typeName, string context)
    {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(context);
        Type t = Type.GetType(typeName);
        //构建T的内部情况
        if (!dataSerialDics.ContainsKey(typeName)) dataSerialDics[typeName] = new List<AggregationEntity>();
        List<AggregationEntity> targetList = dataSerialDics[typeName];
        XmlNode data = null;
        //第一层：目标的数组文件
        for (int i = 0; i < xml.ChildNodes.Count; i++)
        {
            if (xml.ChildNodes[i].Name == "data")
            {
                data = xml.ChildNodes[i];
            }
        }
        //第二层：属性赋值/ 添加/索引修改
        //第三层：Type存在？迭代构建赋值。不存在？Aggration迭代附加
        for (int i = 0; i < data.ChildNodes.Count; i++)
        {
            if (data.ChildNodes[i].Name == typeName)
            {
                var nameTag = data.ChildNodes[i].Attributes.GetNamedItem("name");
                string targetName = "";
                if (nameTag != null)
                {
                    targetName = nameTag.Value;
                }
                object targetObj = targetList.Find((target) => target.idName.Equals(targetName));
                //索引是否有已有数据，则对已有数据进行注入更替
                //ID后续的mod支持采用拼接形式：mod的ID+拼接ID|两int合成long
                if (targetObj == null)
                {
                    targetObj = Activator.CreateInstance(t);
                    (targetObj as AggregationEntity).idName = targetName;
                    (targetObj as AggregationEntity).SetStringValue(Constant_AttributeString.DATA_DIR, loadPath);
                    targetList.Add(targetObj as AggregationEntity);
                }
                //读取其中数据
                foreach (XmlNode node in data.ChildNodes[i].ChildNodes)
                {
                    InjectData(targetObj, t, node);// node.Name, node.InnerText);
                }
            }
        }
    }


    public void SaveXMLByType<T>(string targetName = null) where T : AggregationEntity, IXMLPrintable
    {
        if (string.IsNullOrEmpty(targetName))
        {
            targetName = typeof(T).Name;
        }
        SaveTargetXML(targetName, GetTargetDataList<T>().ToArray());
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    [Obsolete]
    /// <param name="typeName"></param>
    /// <param name="context"></param>
    public void SaveTargetXML(string fileRouteName, params IXMLPrintable[] context)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
        stringBuilder.AppendLine("<data>");
        foreach (IXMLPrintable data in context)
        {
            stringBuilder.Append(data.PrintXML());
        }
        stringBuilder.Append("</data>");

        //TODO:增加根据路径的save
        string dataPath = Application.dataPath + "/Resources" + $"/{fileRouteName}.xml";
        // 判断路径文件
        StreamWriter sw = new StreamWriter(dataPath);
        sw.Write(stringBuilder.ToString());
        sw.Close();
    }

    public void SaveMapXML(string fileName, params IXMLPrintable[] context)
    {
        string s = $"{GetCorePath()}/Map/{fileName}.xml";
        SaveXML(s, context);
    }

    /// <param name="typeName"></param>
    /// <param name="context"></param>
    public void SaveXML(string fileRouteName, params IXMLPrintable[] context)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
        stringBuilder.AppendLine("<data>");
        foreach (IXMLPrintable data in context)
        {
            stringBuilder.Append(data.PrintXML());
        }
        stringBuilder.Append("</data>");
        File.WriteAllText(fileRouteName, stringBuilder.ToString());
#if UNITY_EDITOR
        Debug.LogError(stringBuilder.ToString());
        AssetDatabase.Refresh();
#endif
    }

    public void AddAggregationTempData<T>(T target) where T : AggregationEntity
    {
        string typeName = typeof(T).Name;
        if (dataSerialDics.ContainsKey(typeName)) dataSerialDics[typeName].Add(target);//转换数组返回
    }

    void InjectData(object obj, Type t, XmlNode node)
    {
        bool extraInject = true;
        foreach (var f in t.GetFields())
        {
            if (f.Name.Equals(node.Name))
            {
                if (TryTransferString(f.FieldType, node.InnerText, out object result))
                {
                    f.SetValue(obj, result);
                }
                else
                {
                    //自定义结构需要解析构建
                    Type subNodeType = f.FieldType;

                    if (subNodeType.IsArray)//数组类型
                    {
                        Type elementType = subNodeType.GetElementType();//数组的内部类型
                        object[] subArray = new object[node.ChildNodes.Count];
                        for (int i = 0; i < subArray.Length; i++)
                        {
                            Type nodeElement = elementType;
                            if (elementType.Name != node.ChildNodes[i].Name)
                            {
                                nodeElement = Type.GetType(node.ChildNodes[i].Name);
                            }
                            //需要满足抽象类的创建 目前无法满足
                            try
                            {
                                Activator.CreateInstance(nodeElement);
                            }
                            catch
                            {
#if UNITY_EDITOR
                                Debug.LogError(node.ChildNodes[i].Name + " Class Not Found");
#endif
                                continue;
                            }
                            object arrayObject = Activator.CreateInstance(nodeElement);
                            foreach (XmlNode inSideNode in node.ChildNodes[i].ChildNodes)
                            {
                                InjectData(arrayObject, nodeElement, inSideNode);
                            }
                            subArray[i] = arrayObject;
                        }
                        f.SetValue(obj, CreateArrayUsingReflection(elementType, subArray));
                    }
                    else if (subNodeType.IsEnum)
                    {
                        if (Enum.TryParse(subNodeType, node.InnerText, out object result1))
                        {
                            //Debug.Log("枚举转换成功");
                            f.SetValue(obj, result1);
                        }
                        else
                        {
                            //Debug.Log("枚举转换失败");
                        }
                    }
                    else
                    {
                        object subObject = Activator.CreateInstance(t);
                        //读取其中数据
                        foreach (XmlNode subNode in node.ChildNodes)
                        {
                            InjectData(subObject, t, subNode);// node.Name, node.InnerText);
                        }
                        f.SetValue(obj, subObject);
                    }
                }
                extraInject = false;
                break;
            }
        }
        if (extraInject)
        {
            if (int.TryParse(node.InnerText, out int intResult))
            {
                (obj as AggregationEntity).SetIntValue(node.Name, intResult);
            }
            else if (float.TryParse(node.InnerText, out float floatResult))
            {
                (obj as AggregationEntity).SetFloatValue(node.Name, floatResult);
            }
            else
            {

                //TODO:使用的时候自行解析？
                (obj as AggregationEntity).SetStringValue(node.Name, node.InnerText);
            }
        }
    }

    //TODO:转换规则的附加扩容
    bool TryTransferString(Type fieldType, string originData, out object result)
    {
        if (fieldType.Equals(typeof(int)))
        {
            result = int.Parse(originData);
            return true;
        }
        if (fieldType.Equals(typeof(float)))
        {
            result = float.Parse(originData);
            return true;
        }
        if (fieldType.Equals(typeof(string[])))
        {
            var array = originData.Split('\n');
            List<string> list = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrEmpty(array[i].Trim())) continue;
                list.Add(array[i].Trim());
            }
            result = list.ToArray();
            return true;
        }
        if (fieldType.Equals(typeof(string)))
        {
            result = originData;
            return true;
        }
        if (fieldType.Equals(typeof(Vector2)))
        {
            var array = originData.Split(',');
            result = new Vector2(float.Parse(array[0]), float.Parse(array[1]));
            return true;
        }
#if UNITY_EDITOR
        //if (fieldType.IsArray)
        //{
        //    Debug.LogError("Array:" + fieldType.BaseType);
        //}
        //else
        //{
        //    Debug.LogError("UnRecognizeType" + fieldType);
        //}
#endif
        result = originData;
        return false;
    }

    private object[] CreateArrayUsingReflection(Type elementType, object[] elements)
    {
        Type listType = typeof(List<>).MakeGenericType(elementType);
        object list = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
        foreach (var element in elements)
        {
            listType.InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { element });
        }
        return (object[])listType.InvokeMember("ToArray", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, null);
    }

    public Sprite GetSpriteByIdName(string resName)
    {
        if (spriteTempStore.ContainsKey(resName)) return spriteTempStore[resName];
        if (idNameToDicPath.ContainsKey(resName))
        {
            spriteTempStore[resName] = LoadTextureByPath(idNameToDicPath[resName]).ToSprite();
            return spriteTempStore[resName];
        }
        return null;
    }

    public Texture2D GetTexByIdName(string resName)
    {
        if (idNameToDicPath.ContainsKey(resName))
        {
            return LoadTextureByPath(idNameToDicPath[resName]);
        }
        return null;
    }

    public T GetTargetAggregationData<T>(string idName) where T : AggregationEntity
    {
        return GetTargetDataList<T>().Find((f) => f.idName.Equals(idName));
    }

    //通过泛型获取泛型的类别 type 再通过反射创建对应的XML
    [Obsolete("没有验证过该方法是否真的可以通过反射生成XML表格")]
    public void PrintXML<T>(T obj) where T : IXMLPrintable

    {
        Type type = typeof(T);
        string outputFileName = type.Name + ".xml";

        // 创建 XML 文档
        XmlDocument xmlDoc = new XmlDocument();

        // 创建根元素
        XmlElement rootElement = xmlDoc.CreateElement(type.Name);
        xmlDoc.AppendChild(rootElement);

        // 使用反射获取属性和字段信息
        PropertyInfo[] properties = type.GetProperties();
        FieldInfo[] fields = type.GetFields();

        // 遍历属性，创建对应的 XML 元素
        foreach (PropertyInfo property in properties)
        {
            // 创建属性元素
            XmlElement propertyElement = xmlDoc.CreateElement(property.Name);
            object propertyValue = property.GetValue(obj);

            // 设置元素内容为属性值
            propertyElement.InnerText = propertyValue != null ? propertyValue.ToString() : string.Empty;

            // 将属性元素添加到根元素
            rootElement.AppendChild(propertyElement);
        }

        // 遍历字段，创建对应的 XML 元素
        foreach (FieldInfo field in fields)
        {
            // 创建字段元素
            XmlElement fieldElement = xmlDoc.CreateElement(field.Name);
            object fieldValue = field.GetValue(obj);

            // 设置元素内容为字段值
            fieldElement.InnerText = fieldValue != null ? fieldValue.ToString() : string.Empty;

            // 将字段元素添加到根元素
            rootElement.AppendChild(fieldElement);
        }

        // 输出 XML 文档
        xmlDoc.Save(outputDirectory + outputFileName);



    }
}

public interface IXMLPrintable
{
    string PrintXML();
}