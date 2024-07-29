using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BlockProcessor : MonoBehaviour, IBlockTriggerable
{
    public GameObject[] relatedModels;
    public GameObject particle;

    public int damageAmount;
    public float damageInterval;
    public float damageRadius;

    private float timeCounter;

    private Coroutine burnCoroutine;
    private WaitForSeconds forSeconds = new WaitForSeconds(0.2f);

    bool triggerBurn = false;
    public virtual void OnBlock(object[] param)
    {
        if (param != null && param.Length >= 1)
        {
            if (triggerBurn) return;
            //触发火焰
            foreach (var paramValue in param)
            {
                if (paramValue.Equals("Fire"))
                {
                    triggerBurn = true;
                    particle.gameObject.SetActive(true);
                    burnCoroutine = StartCoroutine(Burning());
                    var terrain = BattleManager.instance.nowBattleMap.mapterrain;
                    TerrainUtil.ChangeTexture(terrain, transform.position, 9);
                    BattleManager.instance.UpdateMiniMap();
                    CoroutineManager.DelayedCoroutine(5f, () =>
                    {
                        relatedModels[1].gameObject.SetActive(true);
                        relatedModels[0].gameObject.SetActive(false);
                        StopCoroutine(burnCoroutine);
                        //TerrainUtil.ChangeTexture(terrain, transform.position, 7);
                    });
                    CoroutineManager.DelayedCoroutine(5.2f, () =>
                    {
                        foreach (var paticleSystem in particle.GetComponentsInChildren<ParticleSystem>())
                        {
                            var main = paticleSystem.main;
                            main.maxParticles = 0;
                        }
                        BattleManager.instance.UpdateMiniMap();
                    });
                    CoroutineManager.DelayedCoroutine(7f, () =>
                    {
                        BattleManager.instance.UpdateMiniMap();
                    });



                    break;
                }
            }
            //记载记录箭矢——方便进行模型切换时的移除？——或者父物体变更
        }
    }


    private IEnumerator Burning()
    {
        timeCounter = damageInterval;

        while (true)
        {
            timeCounter -= 0.2f;
            if (timeCounter <= 0)
            {
                GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<SoldierModel>(transform.position, damageRadius, (model) =>
                {
                    if (model.DistanceTo(transform.position) < damageRadius)
                    {
                        float time = model.nowStatus.GetFloatValue(Constant_AttributeString.STATUS_FIRE_INTERVAL);
                        if (time == 0 || Time.realtimeSinceStartup - time > damageInterval)
                        {
                            model.nowStatus.GetDamage(damageAmount);
                            model.nowStatus.SetFloatValue(Constant_AttributeString.STATUS_FIRE_INTERVAL, Time.realtimeSinceStartup);
                        }
                    }
                });
                if (Random.Range(0, 1f) > 0.9f)
                {
                    GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<IBlockTriggerable>(transform.position, damageRadius, (block) =>
                    {
                        block.OnBlock("Fire");
                    });
                }
                timeCounter = damageInterval;
            }

            yield return forSeconds;
        }

    }


    private void Awake()
    {
        foreach (var model in relatedModels)
        {
            model.AddComponent<BlockReciever>().target = this;
        }
    }
    HashSet<BaseGrid> gridOccupy = new HashSet<BaseGrid>();
    private void OnEnable()
    {
        GridMapManager.instance.gridMap.GridRegist<IBlockTriggerable>(transform.position, 1f, this, ref gridOccupy);
    }
}
