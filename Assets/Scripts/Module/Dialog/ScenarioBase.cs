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
    public string name;
    public string script;
    public string animationID;

    public Fragment(string _name, string _script, string _animationID)
    {
        name = _name;
        this.script = _script;
        this.animationID = _animationID;
    }

}

