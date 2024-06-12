using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioBase
{
    [SerializeField] List<StreamingFragment> fragments;

    public ScenarioBase(List<StreamingFragment> fragments)
    {
        this.fragments = fragments;
    }

    public List<StreamingFragment> Fragments
    {
        get { return fragments; }
    }
}

[System.Serializable]
public class StreamingFragment
{
    public string name;
    public string script;
    public string animationID;

    public StreamingFragment(string _name, string _script, string _animationID)
    {
        name = _name;
        this.script = _script;
        this.animationID = _animationID;
    }
}

public class DialogBase
{
    public List<DialogFragment> fragments { get; }
    public bool isTutorial { get; } 

    public DialogBase(List<DialogFragment> fragments, bool _isTutorial)
    {
        this.fragments = fragments;
        isTutorial = _isTutorial;
    }

}

[System.Serializable]
public class DialogFragment
{
    public string dialog;
    public string talkingOwn;

    public Sprite illust;
    public string illustType;

    public bool isPlayerAnim;
    public string animID;


    public DialogFragment(string dialogID)
    {
        string _dialog = DataManager.Instance.Get_DialogText(dialogID);
        dialog = _dialog;
        string _talkingOwn = DataManager.Instance.Get_DialogSpeaker(dialogID);
        talkingOwn = _talkingOwn;
        string _animID = DataManager.Instance.Get_DialogAnim(dialogID);
        animID = _animID;
        bool _isPlayerAnim = DataManager.Instance.Get_IsPlayerAnimationDialog(dialogID);
        isPlayerAnim = _isPlayerAnim;
        string _illustType = DataManager.Instance.Get_TypeForIllust(DataManager.Instance.Get_DialogIllust(dialogID));
        illustType = _illustType;
        string _illustID = DataManager.Instance.Get_DialogIllust(dialogID);
        illust = GameSystem.Instance.Get_IllustToID(DataManager.Instance.Get_TypeForIllust(DataManager.Instance.Get_DialogIllust(dialogID)), _illustID);
    }

}

