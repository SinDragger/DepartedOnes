using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class DeployPrefab : MonoBehaviour
{
    [BoxGroup("GeneralSetting"), SerializeField] private string prefabPath;
    [BoxGroup("GeneralSetting"), SerializeField] private int density;
    [BoxGroup("GeneralSetting"), SerializeField] private float posOffsetX;
    [BoxGroup("GeneralSetting"), SerializeField] private float posOffsetZ;
    [BoxGroup("GeneralSetting"), SerializeField] private float rotOffsetY;
    [BoxGroup("GeneralSetting"), SerializeField] private float maxScaleOffset;
    [BoxGroup("GeneralSetting"), SerializeField] private float minScaleOffset;


    private void Start()
    {
        CoroutineManager.DelayedCoroutine(0.2f, Generate);
    }

    private void Generate()
    {
        Collider collider = GetComponent<Collider>();
        Transform genParent = GetComponentInParent<Terrain>().GetComponent<Transform>();

        float minX = collider.bounds.min.x - 0.5f;
        float minZ = collider.bounds.min.z - 0.5f;
        float maxX = collider.bounds.max.x + 0.5f;
        float maxZ = collider.bounds.max.z + 0.5f;


        //int flag = 0;
        for (float i = minX; i < maxX; i += density)
        {
            for (float j = minZ; j < maxZ; j += density)
            {
                //flag++;
                //if (flag > 100)
                //{
                //    Debug.LogError("TooMany");
                //    return;
                //}
                Vector3 genPos = Vector3.zero;
                genPos.x = i + Random.Range(-posOffsetX, posOffsetX);
                genPos.z = j + Random.Range(-posOffsetZ, posOffsetZ);

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(genPos.x, 100, genPos.z), Vector3.down, out hit, 150, LayerMask.GetMask("Map")))
                {
                    genPos.y = hit.point.y;
                }
                else
                {
                    //Debug.LogError("ray cant cast map");
                    continue;
                }


                GameObject go = GameObjectPoolManager.Instance.Spawn("Prefab/" + prefabPath);
                go.transform.SetParent(genParent);
                Quaternion rot = Quaternion.Euler(new Vector3(0, Random.Range(-rotOffsetY, rotOffsetY), 0));
                go.transform.SetPositionAndRotation(genPos, rot);
                float scale = Random.Range(minScaleOffset, maxScaleOffset);
                go.transform.localScale = Vector3.one * scale;
            }
        }
        Destroy(gameObject);
        BattleManager.instance.UpdateMiniMap();
    }

}
