using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSpecialEffects : SpecialEffects
{
    public override string effectName => "ChargeSpecialEffects";
    public ParticleSystem particleSystem;

    public override void Execute()
    {
        particleSystem.Play();
    }

    public override void Recycle()
    {
        GameObjectPoolManager.Instance.Recycle(gameObject, DataBaseManager.Instance.configMap[effectName]);
    }
}
