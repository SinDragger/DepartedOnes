using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResourceSlot : MonoBehaviour
{
    public Image icon;
    public Text numberText;
    string targetName;
    ResourcePool resPool;
    public void Init(ResourcePool pool, string resName)
    {
        resPool = pool;
        targetName = resName;
        icon.sprite = DataBaseManager.Instance.GetSpriteByIdName(resName);
        numberText.text = resPool.GetResourceStore(targetName).ToString();
    }

    private void Update()
    {
        numberText.text = resPool.GetResourceStore(targetName).ToString();
    }
}
