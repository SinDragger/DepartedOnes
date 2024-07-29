using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoguePlayer
{
    public int initSoulPoint;
    public int initArmyPoint;
    public int armyPoint;
    public int soulPoint;

    public List<BattleMapTroopData> selfLegionDatas;
    public List<UnitData> buyableUnitList;
    public Dictionary<UnitData, int> needGetDic = new Dictionary<UnitData, int>();
    public List<string> unlockAblespellArray = new List<string>() {
        "CreateMud",
        //"Haste",
    };
    public List<string> spellArray = new List<string>()
    {
        "RaiseCorpse",
#if UNITY_EDITOR
        "FireExplosion",
#endif
    };
    /// <summary>
    /// 可解锁的亡者特性
    /// </summary>
    public List<string> unlockAbleTraits = new List<string>()
    {
        "SkeletonDamageReflect",
        "SkeletonDried",
        "RibArrow",
        "ArrowRain",
        "ZombieInfection",
        "ScatteredSoul",
        "InterspersedTrounce",
        "BattleFuror",
        "RevengeAnger",
    };
    /// <summary>
    /// 已解锁的亡者特性
    /// </summary>
    public List<string> traits = new List<string>();

    /// <summary>
    /// 可解锁的法术钻研
    /// </summary>
    public List<string> unlockAbleSpellProgresses = new List<string>()
    {
        "RebirthFrenzy",
        "RebirthSpeedy",
    };
    /// <summary>
    /// 已解锁的法术钻研
    /// </summary>
    public List<string> spellProgresses = new List<string>();

    /// <summary>
    /// 特性与单位解锁的关联性
    /// </summary>
    public Dictionary<string, string> traitsToUnitDic = new Dictionary<string, string>(){
        {"SkeletonDamageReflect","DriedSkeleton" },
        {"SkeletonDried","DriedSkeleton" },
        {"RibArrow","SkeletonArcher" },
        {"ArrowRain","SkeletonArcher" },
        {"ZombieInfection","AxeSheildZombie" },
        {"ScatteredSoul","GhostSwordGuard" },
        {"InterspersedTrounce","GhostSwordGuard" },
        {"BattleFuror","BlackKnight" },
        {"RevengeAnger","BlackKnight" },
    };
    public Dictionary<string, string> progressToSpellDic = new Dictionary<string, string>() {
        {"RebirthFrenzy","RaiseCorpse" },
        {"RebirthSpeedy","RaiseCorpse" },
    };
    public List<string> techUnlockList = new List<string>();
    public List<string> ableTroopList = new List<string>()
    {
        "DriedSkeleton",
        "GhostSwordGuard",
        "SkeletonArcher",
        "AxeSheildZombie",
        "BlackKnight",
        "SkeletonWolf",
    };

    public RoguePlayer()
    {
    }

    public void AddNewAbleTroop(UnitData data, int number)
    {
        if (!needGetDic.ContainsKey(data))
        {
            needGetDic[data] = number;
        }
        else
        {
            needGetDic[data] += number;
        }
    }

    public int RemoveNewAbleTroop(UnitData data, int number = 1)
    {
        if (!needGetDic.ContainsKey(data))
        {
            return 0;
        }
        else
        {
            needGetDic[data] -= number;
            if (needGetDic[data] <= 0)
            {
                needGetDic.Remove(data);
                return 0;
            }
            return needGetDic[data];
        }
    }

    public int GetAbleNumber(UnitData data)
    {
        if (needGetDic.ContainsKey(data))
        {
            return needGetDic[data];
        }
        return 0;
    }

    public List<UnitData> GetAbleList()
    {
        List<UnitData> result = new List<UnitData>();
        foreach (var value in needGetDic)
        {
            result.Add(value.Key);
        }
        return result;
    }


    /// <summary>
    /// 获取随机奥秘
    /// </summary>
    public (int, string)[] GetRandomAbleMystery(int number)
    {
        List<(int, string)> ableList = new List<(int, string)>();
        if (unlockAblespellArray.Count > 0)
        {
            for (int i = 0; i < unlockAblespellArray.Count; i++)
            {
                ableList.Add((0, unlockAblespellArray[i]));
            }
        }
        if (unlockAbleSpellProgresses.Count > 0)
        {
            for (int i = 0; i < unlockAbleSpellProgresses.Count; i++)
            {
                ableList.Add((1, unlockAbleSpellProgresses[i]));
            }
        }
        if (unlockAbleTraits.Count > 0)
        {
            for (int i = 0; i < unlockAbleTraits.Count; i++)
            {
                //TODO:增加部队情况的检索？

                var relatedUnit = traitsToUnitDic[unlockAbleTraits[i]];
                bool able = false;
                if (selfLegionDatas.Find((legion) => legion.unitIdName == relatedUnit) != null)
                {
                    able = true;
                }
                if (!able)
                {
                    foreach (var pair in needGetDic)
                    {
                        if (pair.Key.idName == relatedUnit)
                        {
                            able = true;
                            break;
                        }
                    }
                }
                if (able)
                    ableList.Add((2, unlockAbleTraits[i]));
            }
        }
        ArrayUtil.Shuffle(ableList);
        (int, string)[] result = new (int, string)[number];
        for (int i = 0; i < number; i++)
        {
            result[i] = ableList[i];
        }
        return result;
    }

    public string GetAbleNewSpell()
    {
        string result = "";
        if (unlockAblespellArray.Count > 0)
        {
            result = unlockAblespellArray[Random.Range(0, unlockAblespellArray.Count)];
        }
        return result;
    }

    public string GetAbleTroopTrait()
    {
        string result = "";
        if (unlockAbleTraits.Count > 0)
        {
            result = unlockAbleTraits[Random.Range(0, unlockAbleTraits.Count)];
        }
        return result;
    }

    public string GetSpellProgress()
    {
        string result = "";
        if (unlockAbleSpellProgresses.Count > 0)
        {
            result = unlockAbleSpellProgresses[Random.Range(0, unlockAbleSpellProgresses.Count)];
        }
        return result;
    }

    public void ChooseNewSpell(string idName)
    {
        unlockAblespellArray.Remove(idName);
        spellArray.Add(idName);
    }

    public void ChooseTroopTrait(string idName)
    {
        unlockAbleTraits.Remove(idName);
        traits.Add(idName);
        GameManager.instance.SetTechAble(idName);
    }

    public void ChooseSpellProgress(string idName)
    {
        unlockAbleSpellProgresses.Remove(idName);
        spellProgresses.Add(idName);
        GameManager.instance.SetTechAble(idName);
    }

}
