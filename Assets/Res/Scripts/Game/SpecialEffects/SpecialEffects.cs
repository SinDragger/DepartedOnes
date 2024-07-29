using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffects : MonoBehaviour
{
    public virtual string effectName => "";


    public virtual void Execute() { 
    
    }

    public virtual void Recycle()
    {
        Destroy(gameObject);
    }

}
