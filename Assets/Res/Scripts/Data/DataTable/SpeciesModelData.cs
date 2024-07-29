using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesModelData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 导向的模型
    /// </summary>
    public string targetModelName;

    public string PrintXML()
    {
        throw new System.NotImplementedException();
    }
}
