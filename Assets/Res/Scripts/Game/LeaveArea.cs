using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveArea : MonoBehaviour
{
    public Collider self;
    public string nextArea;
    bool hasTrigger;
    private void Update()
    {
        if (hasTrigger) return;
        if (ARPGManager.Instance != null && ARPGManager.Instance.currentGeneralControl != null)
        {
            if (self.bounds.Contains(ARPGManager.Instance.currentGeneralControl.transform.position))
            {
                hasTrigger = true;
                GameManager.instance.LeaveArea(nextArea);
                UIManager.Instance.HideUI("TutorialPanel");
            }
        }
    }
}
