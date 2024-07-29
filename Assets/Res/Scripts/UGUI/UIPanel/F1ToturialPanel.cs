using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F1ToturialPanel : MonoBehaviour
{
    public GameObject root;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            if (!root.activeSelf)
                root.SetActive(true);
        }
        else
        {
            if (root.activeSelf)
                root.SetActive(false);
        }
    }
}
