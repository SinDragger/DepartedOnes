using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPanel : UIPanel
{
    public override string uiPanelName => "BattleResultPanel";

    public Button lootButton;
    public GameObject slotPrefab;
    public Transform slotContent;
    public Transform dragLayer;
    public bool isDraggingSlot;
    List<BattleResultTroopSlot> battleResultTroopSlots = new List<BattleResultTroopSlot>();
    WarBattle war;
    [SerializeField]
    BattleResultArea raiseArea;
    [SerializeField]
    BattleResultArea recruitArea;
    BattleResultTroopSlot movingSlot;
    List<TroopControl> enermyTroops;
    List<TroopControl> raiseTroops = new List<TroopControl>();
    List<TroopControl> recruitTroops = new List<TroopControl>();
    public Text raiseCostNumText;
    public Text ableSoulPointText;
    public int willDragOnState = 0;
    public Image spriteIcon;
    bool clickAble;
    LegionControl playerLegion;
    int soulNumber;
    public void Init(WarBattle battleResult)
    {
        clickAble = false;
        war = battleResult;
        //玩家打输了
        playerLegion = war.GetPlayerLegions()[0];
        var enermies = war.GetEnermyLegions();
        if (war.winnerBelongTo != GameManager.instance.belong)
        {
            OnHide();
            playerLegion.CheckRemove();
            foreach(var legion in enermies)
            {
                legion.CheckRemove();
            }
        }
        else
        {
            soulNumber = GameManager.instance.playerData.soulPoint;
            ableSoulPointText.text = $"{soulNumber}";
            foreach (var enermy in enermies)
            {
                int lostNumber = war.originNumber[enermy] - enermy.TotalNum;
                GameManager.instance.playerForce.ChangeLimitedRes("SoulPoint", lostNumber * 3);
                soulNumber += lostNumber * 3;
            }
            var enermyLegion = enermies[0];
            enermyTroops = enermyLegion.troops;
            ArrayUtil.ListShowFit(battleResultTroopSlots, enermyTroops, slotPrefab, slotContent, (troopSlot, data) =>
            {
                troopSlot.gameObject.SetActive(true);
                troopSlot.selfAlpha.alpha = 0f;
                //troopSlot.slot.gameObject.SetActive(false);
                troopSlot.Init(data, war.originTroopNumber[data]);
                troopSlot.SetDrag(() =>
                {
                    isDraggingSlot = true;
                    movingSlot = troopSlot;
                    troopSlot.slot.transform.SetParent(dragLayer);
                    slotContent.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    raiseArea.content.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    recruitArea.content.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }, CheckDraggingSlot, () =>
                {
                    isDraggingSlot = false;
                    slotContent.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    raiseArea.content.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    recruitArea.content.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    return OnDragEnd();
                });
            });

            for (int i = 0; i < enermyTroops.Count; i++)
            {
                var troopData = enermyTroops[i];
                if (troopData.troopEntity.originData.resContain != null)
                {
                    for (int j = 0; j < troopData.troopEntity.originData.resContain.Length; j++)
                    {
                        playerLegion.resourcePool.ChangeResource(troopData.troopEntity.originData.resContain[j].idName, troopData.troopEntity.originData.resContain[j].num * war.originTroopNumber[troopData]);
                    }
                }
            }

            lootButton.gameObject.SetActive(true);
            lootButton.SetBtnEvent(() =>
            {
                ComfirmOperation();
                OnHide();
            });
            CoroutineManager.instance.StartCoroutine(PlayRecieveAnim());
        }
    }

    IEnumerator PlayRecieveAnim()
    {
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < battleResultTroopSlots.Count; i++)
        {
            var troopSlot = battleResultTroopSlots[i];
            troopSlot.selfAlpha.DOFade(1f, 0.3f);
            yield return new WaitForSeconds(0.2f);
            CoroutineManager.instance.StartCoroutine(PlaySoulReceive(troopSlot.slot.transform.position));
            if (troopSlot.slot.troopData.troopEntity.speciesType == "Beast")
            {
                var data = troopSlot.slot.troopData;
                if (data.troopEntity.originData.resContain != null)
                {
                    List<EntityStack> resList = new List<EntityStack>();
                    CoroutineManager.DelayedCoroutine(0.7f, () =>
                    {
                        if (gameObject.activeSelf)
                            troopSlot.ShowRes(resList, war.originTroopNumber[data]);
                    });
                    for (int j = 0; j < data.troopEntity.originData.resContain.Length; j++)
                    {
                        resList.Add(data.troopEntity.originData.resContain[j]);
                    }
                }
            }
        }
        yield return new WaitForSeconds(1f);
        clickAble = true;
    }

    IEnumerator PlaySoulReceive(Vector3 worldPos)
    {
        for (int i = 0; i < 5; i++)
        {
            var g = Instantiate(spriteIcon.gameObject, spriteIcon.transform.parent);
            g.transform.position = worldPos;
            g.transform.DOLocalMove(spriteIcon.transform.localPosition, 0.4f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                Destroy(g);
                ableSoulPointText.text = $"{soulNumber}";
            });
            yield return new WaitForSeconds(0.1f);
        }
    }


    void ComfirmOperation()
    {
        for (int i = 0; i < raiseTroops.Count; i++)
        {
            var troop = raiseTroops[i];
            if (troop.troopEntity.speciesType != "Human") continue;
            playerLegion.ReinforceUnits(troop.idName, "Zombie", war.originTroopNumber[troop]);
            //挂个消逝debuff,随着时间会消亡
        }
        GameManager.instance.playerForce.ChangeLimitedRes("SoulPoint", -nowRaiseCost);
        //获取所有死者的装备份额
        GetAllBorkenEquip();
        LootEnermies(playerLegion, war.GetEnermyLegions());
        foreach (var legion in war.GetEnermyLegions())
        {
            legion.CheckRemove();
        }
        foreach (var legion in war.GetPlayerLegions())
        {
            legion.CheckRemove();
        }
        nowRaiseCost = 0;
        for (int i = battleResultTroopSlots.Count - 1; i >= 0; i--)
        {
            Destroy(battleResultTroopSlots[i].slot.gameObject);
            Destroy(battleResultTroopSlots[i].gameObject);
        }
        raiseArea.OnReset();
        raiseTroops.Clear();
        recruitTroops.Clear();
        battleResultTroopSlots.Clear();
    }
    void GetAllBorkenEquip()
    {
        Dictionary<EquipSetData, int> brokenEquipWeaponDic = new Dictionary<EquipSetData, int>();
        Dictionary<EquipSetData, int> brokenEquipArmourDic = new Dictionary<EquipSetData, int>();
        for (int i = 0; i < playerLegion.troops.Count; i++)
        {
            int num = war.originTroopNumber[playerLegion.troops[i]] - playerLegion.troops[i].nowNum;
            if (num > 0)
            {
                EquipSetData equipWeapon = playerLegion.troops[i].troopEntity.weaponEquipSet.data;
                if (equipWeapon.isUnReachable == 0)
                {
                    if (brokenEquipWeaponDic.ContainsKey(equipWeapon))
                    {
                        brokenEquipWeaponDic[equipWeapon] += num;
                    }
                    else
                    {
                        brokenEquipWeaponDic[equipWeapon] = num;
                    }
                }
                EquipSetData equipArmour = playerLegion.troops[i].troopEntity.armourEquipSet.data;
                if (equipArmour.isUnReachable == 0)
                {
                    if (brokenEquipArmourDic.ContainsKey(equipArmour))
                    {
                        brokenEquipArmourDic[equipArmour] += num;
                    }
                    else
                    {
                        brokenEquipArmourDic[equipArmour] = num;
                    }
                }
            }
        }
        enermyTroops.RemoveAll((troop) => raiseTroops.Contains(troop));
        for (int i = 0; i < enermyTroops.Count; i++)
        {
            int num = war.originTroopNumber[enermyTroops[i]] - enermyTroops[i].nowNum;
            if (num > 0)
            {
                EquipSetData equipWeapon = enermyTroops[i].troopEntity.weaponEquipSet.data;
                if (equipWeapon.isUnReachable == 0)
                {
                    if (brokenEquipWeaponDic.ContainsKey(equipWeapon))
                    {
                        brokenEquipWeaponDic[equipWeapon] += num;
                    }
                    else
                    {
                        brokenEquipWeaponDic[equipWeapon] = num;
                    }
                }
                EquipSetData equipArmour = enermyTroops[i].troopEntity.armourEquipSet.data;
                if (equipArmour.isUnReachable == 0)
                {
                    if (brokenEquipArmourDic.ContainsKey(equipArmour))
                    {
                        brokenEquipArmourDic[equipArmour] += num;
                    }
                    else
                    {
                        brokenEquipArmourDic[equipArmour] = num;
                    }
                }
            }
        }
        playerLegion.ReceiveEquip(brokenEquipWeaponDic, true);
        playerLegion.ReceiveEquip(brokenEquipArmourDic, false);
    }

    void CheckDraggingSlot()
    {
        if (raiseArea.isFocus)
        {
            if (willDragOnState == 0)
            {
                willDragOnState = 1;
            }
            //进行PreFill
        }
        else
        {
            if (willDragOnState == 1)
            {
                willDragOnState = 0;
            }
        }
        if (recruitArea.isFocus)
        {
            if (willDragOnState == 0)
            {
                willDragOnState = 2;
            }
            //进行PreFill
        }
        else
        {
            if (willDragOnState == 2)
            {
                willDragOnState = 0;
            }
        }
    }

    bool OnDragEnd()
    {
        bool willBeProcessed = willDragOnState != 0;
        if (willBeProcessed)
        {
            switch (willDragOnState)
            {
                case 1:
                    if (IsAbleRaise(movingSlot))
                    {
                        raiseArea.ReceiveSlot(movingSlot.slot);
                        raiseTroops.Add(movingSlot.slot.troopData);
                        int costNumber = 10 * movingSlot.number;
                        nowRaiseCost += costNumber;
                        raiseCostNumText.text = $"{nowRaiseCost}";
                        movingSlot.ShowPreview(raiseArea.icon.sprite, costNumber);
                    }
                    else
                    {
                        willBeProcessed = false;
                    }
                    break;
                case 2:
                    recruitArea.ReceiveSlot(movingSlot.slot);
                    movingSlot.preview.GetComponent<Image>().sprite = recruitArea.icon.sprite;
                    movingSlot.preview.gameObject.SetActive(true);
                    break;
            }
        }
        if (!willBeProcessed)
        {
            movingSlot.slot.transform.SetParent(movingSlot.transform);
            movingSlot.slot.transform.localPosition = Vector3.zero;
            movingSlot.slot.transform.localScale = Vector3.one;
            movingSlot.preview.gameObject.SetActive(false);
            if (raiseTroops.Remove(movingSlot.slot.troopData))
            {
                raiseArea.RemoveSlot(movingSlot.slot);
                int costNumber = 10 * movingSlot.number;
                nowRaiseCost -= costNumber;
                raiseCostNumText.text = $"{nowRaiseCost}";
            }
        }
        movingSlot = null;
        return willBeProcessed;
    }

    int nowRaiseCost = 0;
    public bool IsAbleRaise(BattleResultTroopSlot resultSlot)
    {
        if (resultSlot.slot.troopData.troopEntity.speciesType != "Human") return false;
        if (resultSlot.number * 10 + nowRaiseCost > GameManager.instance.playerForce.GetLimitedRes(Constant_AttributeString.RES_SOULPOINT))
        {
            GameManager.instance.ShowTip("灵魂能量不足");
            return false;
        }
        return true;
    }

    public void LootEnermies(LegionControl legion, List<LegionControl> enermies)
    {
        for (int i = 0; i < enermies.Count; i++)
        {
            foreach (var resPair in enermies[i].resourcePool.resourceCarry)
            {
                legion.resourcePool.ChangeResource(resPair.Key, resPair.Value);
            }
        }
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
        CameraControl.Instance.BlockInput();
    }
    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
        TimeManager.Instance.SetToRecovery();
        CameraControl.Instance.RestoreInput();
    }
}
