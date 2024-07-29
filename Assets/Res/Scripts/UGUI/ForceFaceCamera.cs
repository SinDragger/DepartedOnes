using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFaceCamera : MonoBehaviour
{
    public bool isReverse = false;
    bool nowReverse;

    public void Update()
    {
        transform.forward = -CameraControl.Instance.CameraForward * (isReverse ? -1 : 1);
    }
    int x = 0;
    public void Flip()
    {
        isReverse = !isReverse;
    }
    public void FaceToLeft()
    {
        isReverse = false;
    }
    public void FaceToRight()
    {
        isReverse = true;
    }
}
