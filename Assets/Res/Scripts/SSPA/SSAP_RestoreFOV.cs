using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAP_RestoreFOV : StoryScriptPlayerAction
{
    public override string processHead => "RestoreFOV";

    public override void ProcessParamLine(string[] paramLine) => CameraControl.Instance.RestoreFOV();
}
