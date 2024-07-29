using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDEControl : MonoBehaviour
{
    public void Action() {
        
        GameObjectPoolManager.Instance.Recycle(gameObject, DataBaseManager.Instance.configMap["Guide"]);
    }

}
