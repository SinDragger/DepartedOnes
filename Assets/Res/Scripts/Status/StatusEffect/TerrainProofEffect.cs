using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainProofEffect : StatusEffectTerm
{
    public override void Execution(AggregationEntity target)
    {
        target.SetBoolValue(Constant_AttributeString.STATUS_TERRAIN_PROOF, true);
    }
}