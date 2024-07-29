using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHide : MonoBehaviour
{
    public string prefName;
    public GameObject target;
    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        if (PlayerPrefs.GetInt(prefName) > 0)
        {
            Destroy(this);
        }
        else
        {
            target.SetActive(false);
        }
    }
}
