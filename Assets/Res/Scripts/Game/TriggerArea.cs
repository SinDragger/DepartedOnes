using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public int state;
    public UnityEvent onTrigger;

    private void Update()
    {
        switch (state)
        {
            case 0:
                //监控
                break;
        }
    }
}
