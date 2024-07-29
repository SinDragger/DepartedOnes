using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDeployData
{
    /// <summary>
    /// 所属势力
    /// </summary>
    public int belong;
    /// <summary>
    /// 作为指挥者的将领IdName————如果地图上的DeployData里没有，则视作未部署的远程指挥
    /// </summary>
    public string commanderGeneralIdName;
    /// <summary>
    /// 受控制区块的颜色编号
    /// </summary>
    public string[] inControlSectorColors;
    /// <summary>
    /// 下属的部队部署
    /// </summary>
    public LegionDeployData[] legionDeployDatas;
}
