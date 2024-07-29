using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceResourceArea : MonoBehaviour
{
    List<ResourceSlot> resList = new List<ResourceSlot>();
    List<(string, int)> resData;
    public GameObject slotPrefab;
    public Transform resContent;
    Force refForce;
    int lastResCount = -1;
    private void Awake()
    {
        CoroutineManager.StartWaitUntil(() => GameManager.instance && GameManager.instance.playerForce != null, () =>
          {
              Debug.LogError(GameManager.instance.playerForce.data.idName);
              Init(GameManager.instance.playerForce);
          });
    }

    void Init(Force force)
    {
        refForce = force;
        RefreshSlots();
    }

    void RefreshSlots()
    {
        resData = refForce.resourcePool.resourceCarry.GetDictionaryPairList();
        ArrayUtil.ListShowFit(resList, resData, slotPrefab, resContent, (slot, data) =>
        {
            slot.Init(refForce.resourcePool, data.Item1);
            slot.gameObject.SetActive(true);
        });
    }

    private void Update()
    {
        if (refForce != null)
        {
            int count = refForce.resourcePool.resourceCarry.Count;
            if (count != lastResCount)
            {
                lastResCount = count;
                RefreshSlots();
            }
        }
    }
}
