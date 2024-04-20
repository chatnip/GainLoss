using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ArrowLRInteractBtn : BasicInteractBtn
{
    Option Option;

    [Header("*UI")]
    [SerializeField] public ArrowLRValueType arrowLRValueType;
    [SerializeField] public TMP_Text valueTxt;
    [SerializeField] Button LeftBtn;
    [SerializeField] Button RightBtn;


    public override void Awake()
    {
        base.Awake();
        if (GameObject.Find("OptionWindow").TryGetComponent(out Option option))
        { Option = option; }

        LeftBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetEnumValue(false);
                Option.CanSave(true);
            });
        RightBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetEnumValue(true);
                Option.CanSave(true);
            });
    }

    public void ResetUI(string s)
    {
        valueTxt.text = s;
    }

    public void SetEnumValue(bool IsNext)
    {
        if(arrowLRValueType == ArrowLRValueType.Resolution)
        {
            Display_Resolution TempEnum = GetReso(valueTxt.text);
            Display_Resolution ResultValue;
            if (IsNext)
            { ResultValue = Enum_Extensions.Next(TempEnum); }
            else
            { ResultValue = Enum_Extensions.Before(TempEnum); }
            List<int> resultValue = Enum_Extensions.GetValue_Resolution(ResultValue);
            valueTxt.text = resultValue[0] + " X " + resultValue[1];
            return;
        }
        else if (arrowLRValueType == ArrowLRValueType.FPSLimit)
        {
            FramePerSecond Temp_DR = GetFpsLimit(valueTxt.text);
            FramePerSecond ResultValue;
            if (IsNext)
            { ResultValue = Enum_Extensions.Next(Temp_DR); }
            else
            { ResultValue = Enum_Extensions.Before(Temp_DR); }
            int resultValue = Enum_Extensions.GetValue_Fps(ResultValue);
            valueTxt.text = resultValue + "fps";
            return;
        }

    }

    #region Check

    public Display_Resolution GetReso(string txt)
    {
        string[] s = txt.Split();
        List<int> reso = new List<int>() { Convert.ToInt32(s[0]), Convert.ToInt32(s[2]) };
        return Enum_Extensions.GetValue_Resolution(reso);
    }
    public FramePerSecond GetFpsLimit(string txt)
    {
        int fps = Convert.ToInt32(txt.Replace("fps", ""));
        Debug.Log(fps);
        return Enum_Extensions.GetValue_Fps(fps);
    }


    #endregion

}

public enum ArrowLRValueType
{
    Resolution, FPSLimit
}


