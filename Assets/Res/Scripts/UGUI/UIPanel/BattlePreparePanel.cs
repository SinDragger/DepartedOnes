using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreparePanel : UIPanel
{
    public override string uiPanelName => "BattlePreparePanel";

    public BattleMiniMap battleMiniMap;
    public System.Action callback;
    public Button startBattle;
    public Text armyPointText;
    public ItemSlot general;
    public GameObject originPrefab;
    public Transform content;
    public PrepareTroopSlot dragSlot;
    List<PrepareTroopSlot> slots = new List<PrepareTroopSlot>();
    List<TroopControl> datas = new List<TroopControl>();
    BattleMap battleMap;

    public GameObject unitObjectPrefab;
    public Transform unitContent;
    List<UnitData> unitDataList;
    List<UnitPanelSlot> unitPanelSlotList = new List<UnitPanelSlot>();

    public List<ItemSlot> spellSlots;

    //public int[,] controlMap = new int[45, 31];
    public int[,] occuyMap = new int[45, 31];
    int gridXMax = 44;
    int gridYMax = 30;
    public Sprite[] blockSprites;
    public int[,] blockArea;
    public Image[,] blockImageArea;
    int xDelta = 22;
    int yDelta = 15;
    const float DELTA_SCALE = 6f;
    const float GRID_SIZE = 25f;

    public Transform shadowContent;
    public GameObject shadowMap;
    public CanvasGroup canvas;

    int xMax = 16;
    int xMin = -16;
    int yMax = 0;
    int yMin = -8;

    Vector2 startPos = new Vector2(0, GRID_SIZE);

    /// <summary>
    /// 已经购买的数量
    /// </summary>
    Dictionary<UnitData, int> hasBuy = new Dictionary<UnitData, int>();
    /// <summary>
    /// 拥有免费的数量
    /// </summary>
    Dictionary<UnitData, int> hasFree = new Dictionary<UnitData, int>();

    BattleMapData battleMapData;

    public static bool isCreateMode = true;
    private void OnEnable()
    {
        canvas.alpha = 1;
        CoroutineManager.DelayedCoroutine(0.3f, () =>
        {
            canvas.DOFade(0f, 0.06f);
        });
    }
    UnitData nowChooseUnitData;
    public void Init(BattleMap battleMap)
    {
        isCreateMode = false;
#if UNITY_EDITOR
        isCreateMode = true;
#endif
        datas.Clear();
        this.battleMap = battleMap;
        if (isCreateMode)
        {
            GameManager.instance.armyPointCount = 10000;
            unitDataList = GameManager.instance.playerForce.unitList;
        }
        else
            unitDataList = GameManager.instance.playerData.GetAbleList();
        for (int i = 0; i < GameManager.instance.playerData.spellArray.Count; i++)
        {
            var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(GameManager.instance.playerData.spellArray[i]);
            spellSlots[i].GetComponent<SpellPanelTip>().spell = spell;
            spellSlots[i].SetImage(DataBaseManager.Instance.GetSpriteByIdName(spell.iconResIdName));
            spellSlots[i].iconImage.color = Color.white;
            spellSlots[i].GetComponent<SpellPanelTip>().enabled = true;
        }
        for (int i = GameManager.instance.playerData.spellArray.Count; i < spellSlots.Count; i++)
        {
            spellSlots[i].iconImage.color = Color.gray;
            spellSlots[i].GetComponent<SpellPanelTip>().enabled = false;
        }
        string targetMapData = "Test_1";
        if (!string.IsNullOrEmpty(battleMap.targetBattleMapId))
        {
            targetMapData = battleMap.targetBattleMapId;
        }
        battleMapData = DataBaseManager.Instance.GetIdNameDataFromList<BattleMapData>(targetMapData);
        armyPointText.text = GameManager.instance.armyPointCount.ToString();
        if (battleMapData != null)
        {
            battleMap.lineStore.Clear();
            battleMap.linePosDic.Clear();
            battleMap.finalPosDic.Clear();
            foreach (var data in battleMapData.legionDatas)
            {
                var newTroop = new TroopControl(data.unitIdName, data.belong);
                newTroop.SetIntValue("MapPosX", data.posX);
                newTroop.SetIntValue("MapPosY", data.posY);
                newTroop.SetIntValue("State", data.state);
                datas.Add(newTroop);
                battleMap.linePosDic[newTroop] = default;
                battleMap.lineStore.Add(newTroop);
            }
            foreach (var data in GameManager.instance.playerData.selfLegionDatas)
            {
                var newTroop = new TroopControl(data.unitIdName, data.belong);
                newTroop.SetIntValue("MapPosX", data.posX);
                newTroop.SetIntValue("MapPosY", data.posY);
                newTroop.SetIntValue("State", data.state);
                datas.Add(newTroop);
                battleMap.linePosDic[newTroop] = default;
                battleMap.lineStore.Add(newTroop);
            }
            //增加之前战前部署的内容信息
            ArrayUtil.ListShowFit(slots, datas, originPrefab, content, (slot, data) =>
            {
                slot.gameObject.SetActive(true);
                slot.Init(data);
                int posX = data.GetIntValue("MapPosX");
                int posY = data.GetIntValue("MapPosY");
                int attackState = data.GetIntValue("State");
                if (attackState > 0)
                    slot.SetAttack();
                slot.transform.localPosition = GetLocalPos(posX, posY);
                var gridPos = GetGrid(slot.transform.localPosition);
                slot.nowX = gridPos.Item1;
                slot.nowY = gridPos.Item2;
                SetPlace(gridPos.Item1, gridPos.Item2);
                var d = slot.GetComponent<DragAbleObject>();
                d.dragAble = isCreateMode || (data.belong == GameManager.instance.belong);
                d.onDragStart.AddListener(() =>
                {
                    RemovePlace(slot.nowX, slot.nowY);
                    d.transform.SetAsLastSibling();
                });
                d.onDragEnd.AddListener(() =>
                {
                    DragOver(slot, d);
                });
                battleMap.finalPosDic[data] = RealPosTransfer(slot.transform.localPosition);
            });
        }
        else
        {
            foreach (var pair in battleMap.linePosDic)
            {
                datas.Add(pair.Key);
            }
            ArrayUtil.ListShowFit(slots, datas, originPrefab, content, (slot, data) =>
            {
                slot.gameObject.SetActive(true);
                slot.Init(data);
                slot.transform.localPosition = GetLocalPos(battleMap.linePosDic[data].Item1);
                var gridPos = GetGrid(slot.transform.localPosition);
                slot.nowX = gridPos.Item1;
                slot.nowY = gridPos.Item2;
                SetPlace(gridPos.Item1, gridPos.Item2);
                var d = slot.GetComponent<DragAbleObject>();
                d.dragAble = isCreateMode || (data.belong == GameManager.instance.belong);
                d.onDragStart.AddListener(() =>
                {
                    RemovePlace(slot.nowX, slot.nowY);
                    d.transform.SetAsLastSibling();
                });
                d.onDragEnd.AddListener(() =>
                {
                    DragOver(slot, d);
                });
            });
        }
        RefreshChoosableList();
        startBattle.SetBtnEvent(() =>
        {
            if (unitDataList.Count == 0 || isCreateMode)
            {
                OnHide();
            }
            else
            {
                GameManager.instance.ShowTip("还有待部署部队");
            }
        });
    }


    void RefreshChoosableList()
    {
        ArrayUtil.ListShowFit(unitPanelSlotList, unitDataList, unitObjectPrefab, unitContent, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.Init(data);
            slot.numberText.text = GameManager.instance.playerData.GetAbleNumber(data).ToString();
            var color = GameManager.instance.GetForceColor(GameManager.instance.belong);
            slot.slot.numberColor.color = color;
            slot.SetOnClick(() =>
            {
                SetNowChoose(data);
            });
        });
    }

    void SetNowChoose(UnitData unitData)
    {
        nowChooseUnitData = unitData;
        TroopControl data = new TroopControl(unitData, 40, 1);
        dragSlot.Init(data);
        dragSlot.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (nowChooseUnitData != null)
        {
            dragSlot.transform.localPosition = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2)) / GraphicUtil.SCREEN_SIZE_FIX;
            if (Input.GetMouseButtonDown(1))
            {
                nowChooseUnitData = null;
                dragSlot.gameObject.SetActive(false);
            }
            //测试放怪用
            if (isCreateMode && Input.GetMouseButtonDown(2))
            {
                dragSlot.slot.troopData.belong++;
                if (dragSlot.slot.troopData.belong > 2)
                {
                    dragSlot.slot.troopData.belong = 1;
                }
                dragSlot.Init(dragSlot.slot.troopData);
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 nearest = new Vector2(dragSlot.transform.localPosition.x, dragSlot.transform.localPosition.y);
                nearest -= startPos;
                if (nearest.x < 0)
                {
                    nearest.x -= GRID_SIZE / 2;
                }
                else
                {
                    nearest.x += GRID_SIZE / 2;
                }
                if (nearest.y < 0)
                {
                    nearest.y -= GRID_SIZE / 2;
                }
                else
                {
                    nearest.y += GRID_SIZE / 2;
                }
                int x = (int)(nearest.x / GRID_SIZE);
                int y = (int)(nearest.y / GRID_SIZE);
                Vector2 createPos = startPos + new Vector2(GRID_SIZE * x, GRID_SIZE * y);
                int gridX = x + xDelta;
                int gridY = y + yDelta;
                if (IsAblePlace(gridX, gridY))
                {
                    UnitData unit = dragSlot.slot.troopData.troopEntity.originData;
                    //if (GameManager.instance.armyPointCount >= unit.cost)
                    //{
                    PlaceNewData(unit, gridX, gridY, dragSlot.slot.troopData.belong);
                    if (!isCreateMode)
                    {
                        int left = GameManager.instance.playerData.RemoveNewAbleTroop(unit);
                        unitDataList = GameManager.instance.playerData.GetAbleList();
                        RefreshChoosableList();
                        if (left <= 0)
                        {
                            nowChooseUnitData = null;
                            dragSlot.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    void PlaceNewData(UnitData unitData, int gridX, int gridY, int belong)
    {
        TroopControl data = new TroopControl(unitData, unitData.soldierNum, belong);
        var g = GameObject.Instantiate(originPrefab, content);
        var slot = g.GetComponent<PrepareTroopSlot>();
        slots.Add(slot);
        slot.gameObject.SetActive(true);
        slot.Init(data);
        slot.transform.localPosition = GetLocalPos(gridX, gridY);
        var gridPos = GetGrid(slot.transform.localPosition);
        slot.nowX = gridPos.Item1;
        slot.nowY = gridPos.Item2;
        SetPlace(gridPos.Item1, gridPos.Item2);
        var d = slot.GetComponent<DragAbleObject>();
        d.dragAble = belong == GameManager.instance.belong;
        if (isCreateMode)
        {
            d.dragAble = true;
        }
        d.onDragStart.AddListener(() =>
        {
            RemovePlace(slot.nowX, slot.nowY);
            d.transform.SetAsLastSibling();
        });
        d.onDragEnd.AddListener(() =>
        {
            DragOver(slot, d);
        });
        battleMap.finalPosDic[data] = RealPosTransfer(slot.transform.localPosition);
    }

    void InitMaskMap()
    {
        int lineMinX = 24 + 8;
        int lineMinY = 17 + 3;
        blockArea = new int[lineMinX * 2 + 1, lineMinY * 2 + 1];
        blockImageArea = new Image[lineMinX * 2 + 1, lineMinY * 2 + 1];
        int flagX = 0;
        int flagY = 0;
        for (int i = -lineMinX; i <= lineMinX; i++)
        {
            for (int j = -lineMinY; j <= lineMinY; j++)
            {
                var g = Instantiate(shadowMap, shadowContent);
                g.transform.localPosition = new Vector3(25f * i, 25f * j);
                blockImageArea[flagX, flagY] = g.GetComponent<Image>();
                blockImageArea[flagX, flagY].sprite = blockSprites[0];
                flagY++;
            }
            flagY = 0;
            flagX++;
        }

        int areaDeltaX = lineMinX + 1;
        int areaDeltaY = lineMinY - 5;
        int areaX = 20;
        int areaY = 8;
        for (int i = -areaX; i < areaX; i++)
        {
            for (int j = -areaY; j < areaY; j++)
            {
                SetAreaLight(i + areaDeltaX, j + areaDeltaY);
            }
        }
    }

    public void SetAreaLight(int x, int y)
    {
        OnAreaLight(x - 1, y - 1, 4);
        OnAreaLight(x, y - 1, 8);
        OnAreaLight(x - 1, y, 1);
        OnAreaLight(x, y, 2);
    }

    void OnAreaLight(int x, int y, int num)
    {
        if (x > 0 && y > 0)
        {
            blockArea[x, y] += num;
            if (blockArea[x, y] > 15)
            {
                blockArea[x, y] = 15;
            }
            blockImageArea[x, y].sprite = blockSprites[blockArea[x, y]];
        }
    }

    Vector3 GetLocalPos(Vector2 delta)
    {
        return new Vector2(delta.x * 4 * GRID_SIZE, delta.y * 2 * GRID_SIZE);
    }

    Vector3 GetLocalPos(int gridX, int gridY)
    {
        gridX -= xDelta;
        gridY -= yDelta;
        Vector3 result = startPos + new Vector2(GRID_SIZE * gridX, GRID_SIZE * gridY);
        return result;
    }

    void DragOver(PrepareTroopSlot legionTroopSlot, DragAbleObject dragAbleObject)
    {
        Vector2 nearest = dragAbleObject.endDragPos;
        nearest -= startPos;
        if (nearest.x < 0)
        {
            nearest.x -= GRID_SIZE / 2;
        }
        else
        {
            nearest.x += GRID_SIZE / 2;
        }
        if (nearest.y < 0)
        {
            nearest.y -= GRID_SIZE / 2;
        }
        else
        {
            nearest.y += GRID_SIZE / 2;
        }
        int x = (int)(nearest.x / GRID_SIZE);
        int y = (int)(nearest.y / GRID_SIZE);
        dragAbleObject.transform.localPosition = startPos + new Vector2(GRID_SIZE * x, GRID_SIZE * y);
        battleMap.finalPosDic[legionTroopSlot.slot.troopData] = RealPosTransfer(legionTroopSlot.transform.localPosition);
        int gridX = x + xDelta;
        int gridY = y + yDelta;
        if (IsAblePlace(gridX, gridY))
        {
            if (legionTroopSlot.slot.troopData.belong == GameManager.instance.belong)
            {
                if (x <= xMax && x >= xMin && y <= yMax && y >= yMin)
                {
                    SetPlace(gridX, gridY);
                    legionTroopSlot.nowX = gridX;
                    legionTroopSlot.nowY = gridY;
                }
                else
                {
                    dragAbleObject.transform.localPosition = dragAbleObject.startDragPos;
                    SetPlace(legionTroopSlot.nowX, legionTroopSlot.nowY);
                }
            }
            else
            {
                if (gridX > gridXMax - 2 || gridY > gridYMax - 1 || gridX < 0 + 2 || gridY < 0 + 1)
                {
                    dragAbleObject.transform.localPosition = dragAbleObject.startDragPos;
                    SetPlace(legionTroopSlot.nowX, legionTroopSlot.nowY);
                }
                else
                {
                    SetPlace(gridX, gridY);
                    legionTroopSlot.nowX = gridX;
                    legionTroopSlot.nowY = gridY;
                }
            }
        }
        else
        {
            if (isCreateMode && gridX > 50)
            {
                //移除目标单位
                battleMap.finalPosDic.Remove(legionTroopSlot.slot.troopData);
                slots.Remove(legionTroopSlot);
                datas.Remove(legionTroopSlot.slot.troopData);
                Destroy(legionTroopSlot.gameObject);
            }
            else
            {
                dragAbleObject.transform.localPosition = dragAbleObject.startDragPos;
                SetPlace(legionTroopSlot.nowX, legionTroopSlot.nowY);
            }
        }
    }

    (int, int) GetGrid(Vector2 pos)
    {
        pos -= startPos;
        if (pos.x < 0)
        {
            pos.x -= GRID_SIZE / 2;
        }
        else
        {
            pos.x += GRID_SIZE / 2;
        }
        if (pos.y < 0)
        {
            pos.y -= GRID_SIZE / 2;
        }
        else
        {
            pos.y += GRID_SIZE / 2;
        }
        int x = (int)(pos.x / GRID_SIZE);
        int y = (int)(pos.y / GRID_SIZE);
        int gridX = x + xDelta;
        int gridY = y + yDelta;
        return (gridX, gridY);
    }

    void SetPlace(int x, int y)
    {
        occuyMap[x, y] = 1;
        occuyMap[x - 1, y] = 1;
        occuyMap[x + 1, y] = 1;
    }

    bool IsAblePlace(int x, int y)
    {
        for (int i = x - 2; i <= x + 2; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (!CheckPoint(i, j)) return false;
            }
        }
        return true;
    }

    bool CheckPoint(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x > gridXMax) return false;
        if (y > gridYMax) return false;
        if (occuyMap[x, y] != 0) return false;
        return true;
    }

    void RemovePlace(int x, int y)
    {
        occuyMap[x, y] = 0;
        if (x > 0)
            occuyMap[x - 1, y] = 0;
        occuyMap[x + 1, y] = 0;
    }

    /// <summary>
    /// 位置改变
    /// </summary>
    /// <returns></returns>
    Vector3 PosTransfer(Vector3 originPos)
    {
        return new Vector3(originPos.x * DELTA_SCALE, originPos.z * DELTA_SCALE, 0);
    }

    Vector3 RealPosTransfer(Vector3 originPos)
    {
        return new Vector3(originPos.x / DELTA_SCALE, 0, originPos.y / DELTA_SCALE);
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
    }
    public override void OnHide(bool withAnim = true)
    {
        var result = new BattleMapData();
        var list = new List<BattleMapTroopData>();
        GameManager.instance.playerData.selfLegionDatas.Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            var data = slots[i].slot.troopData;
            var troopData = new BattleMapTroopData();
            troopData.unitIdName = data.troopEntity.originData.idName;
            troopData.belong = data.belong;
            troopData.posX = slots[i].nowX;
            troopData.posY = slots[i].nowY;
            troopData.state = slots[i].isAttackState ? 1 : 0;
            if (slots[i].isAttackState)
            {
                UnitControlManager.instance.GetCommand(data.belong).AddAutoCharge(data.troopEntity);
            }

            if (data.belong == GameManager.instance.belong)
            {
                GameManager.instance.playerData.selfLegionDatas.Add(troopData);
                continue;
            }
            list.Add(troopData);
        }
        result.legionDatas = list.ToArray();
        result.randomMapSeed = BattleMapTerrainGenerator.seed;
        Debug.LogError(result.PrintXML());
        if(!string.IsNullOrEmpty(battleMapData.relatedRogueTroop))
        {
            GameManager.instance.playerData.AddNewAbleTroop(DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(battleMapData.relatedRogueTroop), 1);
        }
        CameraControl.Instance.SetCameraPos(0, 0);
        string[] spellArray = GameManager.instance.playerData.spellArray.ToArray();
        //起始法术点
        for (int i = 0; i <= gridXMax; i++)
        {
            for (int j = 0; j <= gridYMax; j++)
            {
                occuyMap[i, j] = 0;
            }
        }
        GameManager.instance.playerData.soulPoint = GameManager.instance.playerData.initSoulPoint;
        var battleGeneralSpellListUI = UIManager.Instance.GetUI("BattleGeneralSpellListUI") as BattleGeneralSpellListUI;
        battleGeneralSpellListUI.SetCastCommandUnit(spellArray);
        battleGeneralSpellListUI.OnShow();
        callback?.Invoke();
        base.OnHide(withAnim);
        datas.Clear();
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            Destroy(slots[i].gameObject);
        }
        slots.Clear();
    }
}
