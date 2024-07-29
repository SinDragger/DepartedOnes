using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TerrainDecal : MonoBehaviour
{
    public Texture tex;
    private List<DecalProjector> projectors = new List<DecalProjector>();

    public void DecalTexture(Texture texture, Vector3 worldPos, string materialName)
    {
        ////对于图片的网格切分
        //List<Vector2Int> gridPos = new List<Vector2Int>();
        //int gridCount_x = texture.width / TextureDrawer.TextureSize;
        //if (gridCount_x * TextureDrawer.TextureSize < texture.width)
        //    gridCount_x++; 
        //int gridCount_y = texture.height / TextureDrawer.TextureSize;
        //if (gridCount_y * TextureDrawer.TextureSize < texture.height)
        //    gridCount_y++;
        //for (int x = 0; x < gridCount_x; ++x)
        //{
        //    for (int y = 0; y < gridCount_y; y++)
        //    {
        //        Vector2Int pos = new Vector2Int(x * TextureDrawer.TextureSize, y * TextureDrawer.TextureSize);
                
        //    }
        //}

        float size = texture.width / TextureDrawer.TextureSize;
        Material material = DataBaseManager.Instance.ResourceLoad<Material>("Material/" + materialName);
        var projector = GameObjectPoolManager.Instance.Spawn("Prefab/DecalProjector").GetComponent<DecalProjector>();
        projector.transform.SetParent(transform);
        projector.transform.position = worldPos;
        projector.size = new Vector3(size, size, 100);


        //var render = GetComponentInChildren<Renderer>();
        //MaterialPropertyBlock block = render.GetComponent<MaterialPropertyBlock>();
        //block.SetTexture("Base_Map", texture);
        //render.SetPropertyBlock(block);

        //TODO 优化新建材质
        material = Instantiate(material);
        material.SetTexture("Base_Map", texture);

        tex = texture;
        projector.material = material;
    }
}
