using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 区块地图与区块其下的建筑管理
/// </summary>
public class SectorBlockManager : MonoSingleton_AutoCreate<SectorBlockManager>, ITimeUpdatable
{
    Texture2D sourceImage;
    public Image bottomImage;
    /// <summary>
    /// 根据颜色索引的字典
    /// </summary>
    Dictionary<Color, SectorBlock> blockDic = new Dictionary<Color, SectorBlock>();
    Dictionary<Color, SectorUI> uiDic = new Dictionary<Color, SectorUI>();

    /// <summary>
    /// 区域地图实例
    /// </summary>
    public SectorBlockMap map;
    SectorBlock tempUse;
    List<Triangle> ableTris;

    protected override void Init()
    {
        if (hasInit) return;
        hasInit = true;
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.LEFT_DOWN, 0, InputLeftDOWN);
        GameManager.timeRelyMethods += OnUpdate;
    }

    public Triangle[] GetTriNeighbour(Triangle tri)
    {
        List<Triangle> result = new List<Triangle>();
        var block = GetBlock(tri.GravityPoint);
        if (tempUse != block && block != null)
        {
            tempUse = block;
            ableTris = new List<Triangle>();
            ableTris.AddRange(block.tris);
            foreach (var neighbour in block.Neighbours())
            {
                ableTris.AddRange((neighbour as SectorBlock).tris);
            }
        }
        //tri.Print();
        foreach (var ableTri in ableTris)
        {
#if UNITY_EDITOR
            //ableTri.graphic.color = Color.white;
#endif
            if (tri.IsNeighbour(ableTri))
            {
                //ableTri.Print();
                result.Add(ableTri);
            }
        }
        return result.ToArray();
    }

    public void LoadTargetMap(string mapName)
    {
        //根据mapName进行配置与加载
        var texture = DataBaseManager.Instance.LoadMapTexture(mapName);
        if (texture == null)
        {
            Debug.LogError($"Do Not Have {mapName}");
            return;
        }
        map = DataBaseManager.Instance.GetTargetDataList<SectorBlockMap>().Find((f) => f.idName.Equals(mapName));
        if (map == null)
        {
            map = new SectorBlockMap();
            map.idName = mapName;
        }
        //初始化当前地图配置
        SectorMapCreator.instance.OnCreateBounds(texture);
        //依据设置 配置边界与地图块颜色
        foreach (var ui in uiDic)
        {
            var data = map.GetTargetSectorBlockData(ui.Key);
            string landform = data.landform;
            ui.Value.InitTargetColor(GetColorFromLandform(landform));
        }
        if (map.buildingDeployDatas != null)
        {
            for (int i = 0; i < map.buildingDeployDatas.Length; i++)
            {
                ConstructionManager.Instance.DeployConstruction(map.buildingDeployDatas[i]);
            }
        }
        if (map.legionDeployDatas != null)
        {
            for (int i = 0; i < map.legionDeployDatas.Length; i++)
            {
                LegionManager.Instance.DeployTargetLegionData(map.legionDeployDatas[i]);
            }
        }
        if (map.blockDatas != null)
        {
            for (int i = 0; i < map.blockDatas.Length; i++)
            {
                var array = map.blockDatas[i].controlProcess;
                if (array != null && array.Length > 0)
                {
                    ColorUtility.TryParseHtmlString("#" + map.blockDatas[i].recognizeColor, out Color blockColor);
                    var block = blockDic[blockColor];
                    for (int j = 0; j < array.Length; j++)
                    {
                        block.SetProgress(array[j].idName, array[j].num);
                    }
                }
            }
        }
        if (map.CameraX != default && map.CameraY != default)
        {
            CameraControl.Instance.SetCameraPos(map.CameraX, map.CameraY);
        }
        if (map.blockDatas != null)
        {
            HashSet<Border> borderSets = new HashSet<Border>();
            foreach (var pair in blockDic)
            {
                foreach (var border in pair.Value.borders)
                {
                    borderSets.Add(border);
                }
            }
            List<Vector2[]> resultList = new List<Vector2[]>();
            foreach (var border in borderSets)
            {
                Vector2[] result = new Vector2[border.pointsNum];
                int flag = 0;
                border.PointsAction((xy) =>
                {
                    result[flag] = new Vector2(xy.Item1, xy.Item2);
                    flag++;
                });
                resultList.Add(result);
            }
        }
        RefreshBorderAndSector();
    }

    public void UpdateToNextShowLevel()
    {
        foreach(var pair in blockDic)
        {
                SectorRevealHideLevel(pair.Value);
        }
        RefreshBorderAndSector();
    }

    void SectorRevealHideLevel(SectorBlock sectorBlock)
    {
        if (sectorBlock.hideLevel > 0)
        {
            sectorBlock.hideLevel--;
            foreach (var construction in sectorBlock.constructions)
            {
                if (construction.hideLevel > 0)
                {
                    construction.hideLevel--;
                    if (construction.hideLevel <= 0)
                    {
                        ConstructionManager.Instance.GetMapUI(construction)?.OnShow();
                    }
                }
            }
        }
    }


    void RefreshBorderAndSector()
    {
        HashSet<Border> borders = new HashSet<Border>();
        foreach (var ui in uiDic)
        {
            var block = GetBlock(ui.Key);
            if (block.hideLevel > 0)
            {
                ui.Value.OnHide();
                foreach (var b in block.borders)
                {
                    if (!borders.Contains(b))
                        b.borderObject.gameObject.SetActive(false);
                }
            }
            else
            {
                ui.Value.OnShow();
                foreach (var b in block.borders)
                {
                    b.borderObject.gameObject.SetActive(true);
                    borders.Add(b);
                }
            }
        }
    }

    /// <summary>
    /// 区域控制者变更
    /// </summary>
    public void SectorControlChange(SectorBlock sector, int lastBelong, int nowBelong)
    {

    }

    private Color GetColorFromLandform(string landform)
    {
        if (string.IsNullOrEmpty(landform)) return ColorUtil.test_PlaneColor;
        ColorUtility.TryParseHtmlString(DataBaseManager.Instance.GetIdNameDataFromList<Landform>(landform).color, out Color result);
        return result;
    }

    //左键点击
    bool InputLeftDOWN(object param)
    {
        //获取目标的点击
        if (bottomImage == null || sourceImage == null) return false;
        var t = InputManager.Instance.NowRaycastFind<Image>();
        if (t == bottomImage)
        {
            var block = GetBlock(GetNowPointPosition());
            if (block == null || block.hideLevel > 0) return false;
            TargetBlockOnSelect(block.recognizeColor);
            return true;
        }
        return false;
    }

    public bool IsInSameSector(Vector2 a, Vector2 b)
    {
        return GetRegColor(a) == GetRegColor(b);
    }

    /// <summary>
    /// 可以合并进InputManager逻辑中
    /// </summary>
    public Vector3 GetNowPointPosition()
    {
        var result = InputManager.Instance.NowRaycastSearch((raycast) =>
        {
            return raycast.gameObject.Equals(bottomImage.gameObject);
        });
        if (!result.Equals(default))
        {
            return result.worldPosition;
        }
        return default;
    }

    public void RegisterNowMapData(Dictionary<Color, SectorBlock> blockDic, Texture2D soureImage)
    {
        this.sourceImage = soureImage;
        this.blockDic = blockDic;
        if (map == null)
        {
            map = new SectorBlockMap();
        }
        map.SynNewMapBlocks(blockDic);
    }

    public void RegisterNowUIData(Dictionary<Color, SectorUI> uiDic)
    {
        this.uiDic = uiDic;
    }

    Color selectIndex;

    public void TargetBlockOnSelect(Color index)
    {
        if (index.Equals(selectIndex)) return;
        selectIndex = index;
        if (!blockDic.ContainsKey(index))
        {
            EventManager.Instance.DispatchEvent(new EventData(MapEventType.SECTOR_SELECT, "SectorColor", index, "SectorBlock", null));
        }
        else
        {
            EventManager.Instance.DispatchEvent(new EventData(MapEventType.SECTOR_SELECT, "SectorColor", index, "SectorBlock", blockDic[index]));
        }
    }

    public void PrintMapData()
    {
        Debug.LogError(map.PrintXML());
        DataBaseManager.Instance.SaveMapXML(map.idName, map);
    }

    public Color GetRegColor(Vector3 position)
    {
        return sourceImage.GetPixel((int)position.x, (int)position.z);
    }

    public Color GetRegColor(Vector2 position)
    {
        return sourceImage.GetPixel((int)position.x, (int)position.y);
    }

    public SectorBlock GetBlock(Vector3 position)
    {
        return GetBlock(new Vector2(position.x, position.z));
    }
    public SectorBlock GetBlock(Vector2 position)
    {
        var color = GetRegColor(position);
        if (color != Color.black)
        {
            return blockDic[color];
        }
        return null;
    }

    public SectorBlock GetBlock(Color color)
    {
        if (color != Color.black)
        {
            return blockDic[color];
        }
        return null;
    }

    public Vector2 GetNowChooseBlockCenter()
    {
        if (selectIndex != default)
        {
            return blockDic[selectIndex].gravityPoint;
        }
        else
        {
            return default;
        }
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var pair in blockDic)
        {
            pair.Value.OnUpdate(deltaTime);
        }
        foreach (var pair in uiDic)
        {
            pair.Value.UpdateColor();
        }
    }

    public void OnReset()
    {
        foreach (var pair in uiDic)
        {
            Destroy(pair.Value.gameObject);
        }
        uiDic.Clear();
        foreach (var pair in blockDic)
        {
            foreach (Border border in pair.Value.borders)
            {
                if (border.borderObject)
                    Destroy(border.borderObject);
            }
        }
        blockDic.Clear();
        map = null;
    }

}
