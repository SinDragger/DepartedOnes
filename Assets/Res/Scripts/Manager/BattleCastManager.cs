using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗中施法系统
/// </summary>
public class BattleCastManager : MonoSingleton<BattleCastManager>, IBattleManageSingleton
{
    public int priority => 2;

    public ChooseWheel chooseWheel;
    public GameObject indicate;
    public RectTransform indicateUI;
    public Text indicateUIText;
    public GameObject effectPrefab;

    public BattleCastManager_InputModule inputModule = new BattleCastManager_InputModule();
    public ChooseWheel_InputModule chooseWheel_Input = new ChooseWheel_InputModule();
    public TroopSpell_InputModule troopSpell_Input = new TroopSpell_InputModule();

    public Vector3 determinePos = default;
    public bool determineTransfrom = false;
    const float AUTO_CAST_WAIT_TIME = 0.2f;
    float mulCount = 0f;
    CommandUnit castCommandUnit;

    int randomMulCast;
    float randomExpend;

    public void Init(Action callback = null)
    {
        inputModule.Init();
        chooseWheel_Input.Init();
        troopSpell_Input.Init();
    }

    /// <summary>
    /// 初始化施法轮
    /// </summary>
    public void InitSpellWheel(string[] spellArray)
    {
        chooseWheel.gameObject.SetActive(true);
        //nowCastSpell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(spellIdName);
        chooseWheel.Init(spellArray, (idName) =>
        {
            return DataBaseManager.Instance.GetSpriteByIdName(idName);
        });
    }

    public void InitSubBtn(ItemSlot button, string idName, Func<string, Sprite> iconFrom)
    {
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        button.GetComponent<SpellPanelTip>().spell = spell;
        button.GetComponent<Button>().SetBtnEvent(() =>
        {
            BattleCastManager.instance.SwitchModeIntoCastSpell(idName);
        });
        button.iconImage.sprite = iconFrom.Invoke(spell.iconResIdName);
    }

    Vector3 GetRandomRange(float radius)
    {
        return new Vector3(UnityEngine.Random.Range(-radius, radius), 0f, UnityEngine.Random.Range(-radius, radius));
    }

    public void StartContinousSpeelCast()
    {
        //TODO:加判断
        if (nowCastSpell.releaseType != 0)
        {
            UpdateIndicatePos();
            StartCoroutine(SpeelCast(nowCastSpell, determinePos));
        }
    }

    public void StartSpeelCast()
    {
        CompleteReset();
        DataReset();
        indicate.gameObject.SetActive(true);
        bool isAbleToCast = IsAbleToCast(nowCastSpell);
        if (!isAbleToCast)
        {
            GameManager.instance.ShowTip("灵魂能量不足");
            return;
        }
        else
        {
            int cost = nowCastSpell.soulPointCost;
            GameManager.instance.playerForce.ChangeLimitedRes("SoulPoint", -cost);
        }
        StartCoroutine(SpeelCast(nowCastSpell, determinePos));
    }

    public void TroopCastSpell(CommandUnit from, int targetType, string spellId)
    {
        if (targetType == 1)
        {
            CastableSpellData spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(spellId);
            var pos = UnitControlManager.instance.GetMostDenseEnermyPos(from.belong);
            StartCoroutine(SpeelCast(spell, pos));
        }
    }

    IEnumerator SpeelCast(CastableSpellData castSpell, Vector3 pos)
    {
        int castNum = castSpell.multiCast;
        if (castNum == 0) castNum = 1;
        for (int i = 0; i < castNum; i++)
        {
            Vector3 effectPos = pos + new Vector3(0, 0.1f, 0) + GetRandomRange(castSpell.areaRandom);
            foreach (var effect in castSpell.effects)
            {
                effect.effectPos = effectPos;
                effect.Execution();
            }
            yield return new WaitForSeconds(0.06f);
        }
    }

    bool IsAbleToCast(CastableSpellData castSpell)
    {
        int cost = castSpell.soulPointCost;
        if (cost > 0)
        {
            if (GameManager.instance.playerData.soulPoint < cost)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 施法蓄力
    /// </summary>
    public void PowerUpMul()
    {
        //做好技能蓄力准备
        mulCount += Time.deltaTime;
        if (mulCount > AUTO_CAST_WAIT_TIME)
        {
            if (randomMulCast < castCommandUnit.aliveCount)
                randomMulCast++;

            if (randomMulCast > 1)
            {
                indicateUIText.text = $"X{randomMulCast}";
                indicateUIText.gameObject.SetActive(true);
            }
            mulCount -= AUTO_CAST_WAIT_TIME;
        }
        if (randomMulCast < castCommandUnit.aliveCount)
        {
            randomExpend += Time.deltaTime;
            DecalSizeReset(randomExpend);
        }
    }

    public void CompleteReset()
    {
        mulCount = 0;
        randomMulCast = 1;
        randomExpend = 0;
        indicateUIText.gameObject.SetActive(false);
        indicate.gameObject.SetActive(false);
    }

    void DataReset()
    {
        indicateUIText.gameObject.SetActive(false);
        Vector3 target = Vector3.one * (4f + randomExpend * 2f);
        target.z = 100f;
        DecalSizeReset(randomExpend);
        indicate.transform.localScale = target;
        determineTransfrom = false;
    }

    void DecalSizeReset(float random)
    {
        var decal = indicate.GetComponent<UnityEngine.Rendering.Universal.DecalProjector>();
        var size = decal.size;
        float radius = 2;
        if (nowCastSpell != null)
        {
            radius = nowCastSpell.radius * 2;
        }
        size.x = radius + random * 2f;
        size.y = radius + random * 2f;
        decal.size = size;
    }

    public void OnUpdate()
    {
        if (determineTransfrom)
        {
            //确定移动位置之后
            determineTransfrom = indicate.activeSelf;
            return;
        }
        if (indicate.activeSelf)
        {
            UpdateIndicatePos();
            if (castCommandUnit != null && castCommandUnit.aliveCount == 0)
            {
                //取消释放
                Debug.LogError("如果出错就是施法部队死亡了之后的重置施法出了问题");
                DataReset();
                indicate.gameObject.SetActive(false);
            }
        }
    }

    public void SetCastCommandUnit(CommandUnit commandUnit, string[] spellArray, Vector2 pos)
    {
        castCommandUnit = commandUnit;
        InitSpellWheel(spellArray);
        //TODO:改动为当前Flag的位置
        var flag = UnitControlManager.instance.GetControlFlag(commandUnit);
        (chooseWheel.transform as RectTransform).anchoredPosition = (flag.transform as RectTransform).anchoredPosition;
        //激活技能轮盘InputModule 激活 left right
        chooseWheel_Input.Active();
        Time.timeScale = 0.05f;

    }
    public void SetCastCommandUnit(CommandUnit commandUnit, string[] spellArray)
    {
        castCommandUnit = commandUnit;
        InitSpellWheel(spellArray);

        var flag = UnitControlManager.instance.GetControlFlag(commandUnit);
        // (chooseWheel.transform as RectTransform).anchoredPosition = (flag.transform as RectTransform).anchoredPosition;
        //激活技能轮盘InputModule 激活 left right
        // chooseWheel_Input.Active();
        Time.timeScale = 0.05f;

    }
    public void CloseChooseWheel()
    {
        chooseWheel.gameObject.SetActive(false);
        chooseWheel_Input.Deactive();
        Time.timeScale = 1f;
    }


    void UpdateIndicatePos()
    {
        indicate.transform.position = InputManager.Instance.mouseWorldPos + new Vector3(0, 50, 0);
        indicateUI.anchoredPosition = Input.mousePosition;
    }

    CastableSpellData nowCastSpell;
    public void SwitchModeIntoCastSpell(string spellIdName)
    {
        //判断是否有资源可以释放技能 直接退出 return;
        //真正的施法inputModule在这里Active
        DataReset();
        CompleteReset();
        CloseChooseWheel();
        nowCastSpell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(spellIdName);
        DecalSizeReset(0f);
        indicate.transform.position = InputManager.Instance.mouseWorldPos + new Vector3(0, 50, 0);
        troopSpell_Input.Active();
        indicate.gameObject.SetActive(true);
    }

    public void OnActive()
    {
        inputModule.Active();
    }

    public void OnDeactive()
    {
        inputModule.Deactive();
        troopSpell_Input.Deactive();
        CompleteReset();
    }
}
