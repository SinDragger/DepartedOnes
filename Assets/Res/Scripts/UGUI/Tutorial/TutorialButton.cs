using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialButton : MonoBehaviour
{
    public UnityEvent OnLeftClick;
    public UnityEvent OnRightClick;

    private void OnGUI()
    {
        Event current = Event.current;
        if (EventType.MouseDown == current.type)
        {
            if (current.button == 0)
            {
                OnLeftClick.Invoke();
            }
            else if (current.button == 1)
            {
                OnRightClick.Invoke();
            }
        }
    }
}
