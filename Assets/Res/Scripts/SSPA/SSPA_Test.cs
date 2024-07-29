using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSPA_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public Collider self;
    public bool hasTrigger;
    private void Update()
    {
        if (hasTrigger) return;
        if (ARPGManager.Instance != null && ARPGManager.Instance.currentGeneralControl != null)
        {
            if (self.bounds.Contains(ARPGManager.Instance.currentGeneralControl.transform.position))
            {
               
                hasTrigger = true;

                StoryManager.instance.ActiveStoryMode("StoryTest");
            }
        }
    }
  
}
