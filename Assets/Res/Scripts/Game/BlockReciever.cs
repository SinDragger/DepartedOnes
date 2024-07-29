using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockReciever : MonoBehaviour, IBlockTriggerable
{
    public IBlockTriggerable target;
    public void OnBlock(object[] param)
    {
        if (target!=null)
            target.OnBlock(param);
    }
}
