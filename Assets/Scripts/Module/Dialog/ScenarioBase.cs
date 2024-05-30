using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioBase
{
    [SerializeField] List<Fragment> fragments;

    public ScenarioBase(List<Fragment> fragments)
    {
        this.fragments = fragments;
    }

    public List<Fragment> Fragments
    {
        get { return fragments; }
    }
}

[System.Serializable]
public class Fragment
{
    public string animationID;
    public string script;
    public bool leftOrRight;

    public Fragment(string animeData, string scriptData, string leftOrRightData)
    {
        this.animationID = animeData;
        this.script = scriptData;
        this.leftOrRight = Convert.ToBoolean(leftOrRightData);
    }

    public string AnimationID
    { get { return animationID; } }
    public string Script
    { get { return script; } }
    public bool LeftOrRight
    { get { return leftOrRight; } }
}

