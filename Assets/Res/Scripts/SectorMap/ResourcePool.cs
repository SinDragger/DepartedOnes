using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePool
{
    /// <summary>
    /// 资源携带
    /// </summary>
    public Dictionary<string, int> resourceCarry = new Dictionary<string, int>();

    /// <summary>
    /// 从行军携带库存之中进行资源消耗
    /// </summary>
    /// <param name="idName"></param>
    /// <param name="num">剩余的数字量</param>
    /// <returns></returns>
    public int ResourceConsume(string idName, int num)
    {
        if (num == 0) return 0;
        //优先消耗已有的
        if (resourceCarry.ContainsKey(idName))
        {
            int store = resourceCarry[idName];
            if (store > num)
            {
                resourceCarry[idName] -= num;
                return 0;
            }
            else
            {
                num -= resourceCarry[idName];
                resourceCarry.Remove(idName);
            }
        }
        return num;
    }
    public int ResourceConsume(EntityStack[] stacks, int num)
    {
        if (stacks == null || num == 0) return 0;
        for (int i = 0; i < stacks.Length; i++)
        {
            ChangeResource(stacks[i].idName, -stacks[i].num * num);
        }
        return 0;
    }

    public void ChangeResource(params string[] resArray)
    {
        for (int i = 0; i < resArray.Length - 1; i += 2)
        {
            ChangeResource(resArray[i], int.Parse(resArray[i + 1]));
        }
    }

    public void ChangeResource(string resName, int resNum)
    {
        if (resourceCarry.ContainsKey(resName))
        {
            resourceCarry[resName] += resNum;
        }
        else
        {
            resourceCarry[resName] = resNum;
        }
    }

    public void SetResource(string resName, int resNum)
    {
        resourceCarry[resName] = resNum;
    }


    public int GetResourceStore(string idName)
    {
        if (resourceCarry.ContainsKey(idName)) return resourceCarry[idName];
        return 0;
    }


    public void GetResource(Dictionary<string, int> otherResPool)
    {
        foreach (var pair in otherResPool)
        {
            if (pair.Value > 0)
            {
                ChangeResource(pair.Key, pair.Value);
            }
        }
        otherResPool.Clear();
    }


    public void GetResource(ResourcePool otherResPool)
    {
        foreach (var pair in otherResPool.resourceCarry)
        {
            if (pair.Value > 0)
            {
                ChangeResource(pair.Key, pair.Value);
            }
        }
        otherResPool.resourceCarry.Clear();
    }

    public bool Contains(Dictionary<string, int> otherResPool)
    {
        foreach (var pair in otherResPool)
        {
            if (GetResourceStore(pair.Key) >= pair.Value) continue;
            else return false;
        }
        return true;
    }
    public bool Comsumes(Dictionary<string, int> otherResPool)
    {
        foreach (var pair in otherResPool)
        {
            ChangeResource(pair.Key, -pair.Value);
        }
        return false;
    }

    public int MaxAble(EntityStack[] entityStacks)
    {
        int result = int.MaxValue;
        if (entityStacks == null || entityStacks.Length == 0) return result;
        for (int i = 0; i < entityStacks.Length; i++)
        {
            int ableNum = GetResourceStore(entityStacks[i].idName) / entityStacks[i].num;
            if (result > ableNum)
            {
                result = ableNum;
            }
        }
        return result;
    }
}
