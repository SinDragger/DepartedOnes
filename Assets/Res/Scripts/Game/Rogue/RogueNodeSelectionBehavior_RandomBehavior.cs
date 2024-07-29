using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueNodeSelectionBehavior_RandomBehavior : RogueNodeSelectionBehavior
{
    public int[] behaviorWeights;
    public RogueNodeSelectionBehavior[][] randomBehaviors;

    public override void Excute()
    {
        int sum = 0;
        foreach(int weight in behaviorWeights)
            sum += weight;
        int r = Random.Range(0, sum);
        int i = 0;
        for(int j = 0; i < behaviorWeights.Length - 1; j += behaviorWeights[i])
        { 
            int temp = j + behaviorWeights[i + 1];
            if (r >= j && r < temp)
                break;
            else
                i++;
        }
        foreach(var behavior in randomBehaviors[i])
            behavior.Excute();
    }
}
