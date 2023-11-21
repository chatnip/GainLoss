using System.Collections;
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
    [TextArea]
    public string script;

    public Fragment(string id, string script)
    {
        this.animationID = id;
        this.script = script;
    }

    public string AnimationID
    {
        get { return animationID; }
    }
    public string Script
    {
        get { return script; }
    }
}