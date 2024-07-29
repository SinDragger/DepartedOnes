using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AuraFlagEntity : MonoBehaviour
{
    private TerrainDecal terrainDecal;
    private GameObject particle;

    private EffectTermAreaAura dataRef;

    public void Init(EffectTermAreaAura ara)
    {
        dataRef = ara;
        transform.position = ara.effectPos;
        if (ara.isPraticle == 1)
        {
            terrainDecal = GameObjectPoolManager.Instance.Spawn("Prefab/TerrainDecal").GetComponent<TerrainDecal>();
            Texture tex = null;
            switch (ara.materialEffect)
            {
                case MaterialEffect.None:
                    int size = (int)(ara.radius * 2);
                    if (size % 2 != 1) size++;
                    tex = RenderTexture.GetTemporary(size, size);
                    break;
                case MaterialEffect.Noise:
                    tex = TextureDrawer.DrawNoiseCircleTexture(ara.radius);
                    break;
            }
            terrainDecal.DecalTexture(TextureDrawer.DrawNoiseCircleTexture(ara.radius), ara.effectPos, ara.materialName);
        }
        else
        {
            particle = GameObjectPoolManager.Instance.Spawn("Prefab/Effect/" + ara.effectName);

            foreach (var ps in particle.GetComponentsInChildren<ParticleSystem>())
            {
                var shape = ps.shape;
                shape.radius = ara.radius;
            }

            particle.transform.position = ara.effectPos + new Vector3(0, 3, 0);
        }
        
        AggregationEntity attachEntity;
        for (int i = 0; i < ara.halos.Length; ++i)
        {
            var worldEffect = StatusManager.Instance.worldEffectsEntityStatus.Find((e) => e.originStatus.idName.Equals(ara.halos[i]));
            if (worldEffect == null)
            {
                attachEntity = new AggregationEntity();
                attachEntity.SetObjectValue(Constant_AttributeString.BELONG, -1);
                StatusManager.Instance.worldEffectsEntityStatus.Add(StatusManager.Instance.RequestStatus(ara.halos[i], attachEntity));
            }
            else
            {
                attachEntity = worldEffect.dataModel;
            }
            var gridSet = attachEntity.GetObjectValue<Dictionary<BaseGrid, int>>(Constant_AttributeString.STATUS_EFFECT_AREA);
            if (gridSet == null)
            {
                gridSet = new Dictionary<BaseGrid, int>();
                attachEntity.SetObjectValue(Constant_AttributeString.STATUS_EFFECT_AREA, gridSet);
            }
            float radius = ara.radius - 2f;
            if (radius > 0)
            {
                var grids = GridMapManager.instance.gridMap.GetGridsInCircle(ara.effectPos, radius);
                foreach (var grid in grids)
                {
                    if (gridSet.ContainsKey(grid))
                        gridSet[grid]++;
                    else
                        gridSet.Add(grid, 1);
                }
            }
            //从Entity中拿取BaseGridSet 并 更新
        }
        BattleManager.instance.UpdateMiniMap();
        //System.Action selfDestroy = default;
        //selfDestroy = () =>
        //{
        //    Destroy(gameObject);
        //    Destroy(terrainDecal.gameObject);
        //    BattleManager.instance.battleToMap -= selfDestroy;
        //};
        BattleManager.instance.battleToMap += EndEffect;
    }

    public void EndEffect()
    {
        Destroy(gameObject);
        if(terrainDecal != null)
            Destroy(terrainDecal.gameObject);
        if (particle != null)
            Destroy(particle);

        AggregationEntity attachEntity;
        for (int i = 0; i < dataRef.halos.Length; ++i)
        {
            var worldEffect = StatusManager.Instance.worldEffectsEntityStatus.Find((e) => e.originStatus.idName.Equals(dataRef.halos[i]));
            if (worldEffect != null)
            {
                attachEntity = worldEffect.dataModel;
            }
            else
            {
                Debug.LogError("Cant find effect in world");
                continue;
            }
            var gridSet = attachEntity.GetObjectValue<Dictionary<BaseGrid, int>>(Constant_AttributeString.STATUS_EFFECT_AREA);
            if (gridSet == null)
            {
                Debug.LogError("Cant find grid data in entity");
                continue;
            }
            float radius = dataRef.radius - 2f;
            if (radius > 0)
            {
                var grids = GridMapManager.instance.gridMap.GetGridsInCircle(dataRef.effectPos, radius);
                foreach (var grid in grids)
                {
                    if (gridSet.ContainsKey(grid))
                    {
                        gridSet[grid]--;
                        if (gridSet[grid] <= 0)
                            gridSet.Remove(grid);
                    }
                }
            }
        }


        BattleManager.instance.battleToMap -= EndEffect;
    }
}
