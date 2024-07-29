using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimSerial : MonoBehaviour
{
    [SpineAnimation]
    public string idleAnimation;

    [SpineAnimation]
    public string attackAnimation;

    [SpineAnimation]
    public string moveAnimation;

    [SpineAnimation]
    public string dieAnimation;

}
