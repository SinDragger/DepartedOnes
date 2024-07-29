using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetMapLoadButton : MonoBehaviour
{
    public InputField input;
    public Button button;

    private void Awake()
    {
        var eventTrigger = input.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = input.gameObject.AddComponent<EventTrigger>();
        UnityAction<BaseEventData> selectEvent = OnInputFieldClicked;
        EventTrigger.Entry onClick = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerClick
        };
        onClick.callback.AddListener(selectEvent);
        eventTrigger.triggers.Add(onClick);

        input.onEndEdit.AddListener((s) =>
        {
            InputManager.Instance.inputingText = false;
        });
        button.onClick.AddListener(() =>
        {
            SectorBlockManager.Instance.LoadTargetMap(input.text);
        });
    }
    public void OnInputFieldClicked(BaseEventData data)
    {
        InputManager.Instance.inputingText = true;
    }
}
