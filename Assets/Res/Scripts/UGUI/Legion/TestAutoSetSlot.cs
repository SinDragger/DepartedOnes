using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAutoSetSlot : MonoBehaviour
{
    LegionTroopSlot target;
    public SkeletonDataAsset targetDataAsset;
    public string targetModelName = "Model_A_HumanMelee";
    public string targetEquipSetName;
    private void Start()
    {
        if (target == null)
            target = GetComponent<LegionTroopSlot>();
        //CoroutineManager.StartWaitUntil(() => { return GameManager.instance != null && GameManager.instance.hasInit; }, () => {

        //});
        CoroutineManager.StartDelayedCoroutine(1f, () =>
        {
            target.GraphicInit(targetDataAsset, targetEquipSetName, "", "", targetModelName);
        });
    }

}
