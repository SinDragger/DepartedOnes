using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    protected virtual string EventId => "";

    private void OnEnable()
    {
        EventManager.Instance.RegistEvent(EventId, OnTrigger);
    }

    protected virtual void OnTrigger(EventData data)
    {
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegistEvent(EventId, OnTrigger);
    }
}
