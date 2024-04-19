using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ArrowLRInteractBtn : BasicInteractBtn
{
    GameSettingManager GameSettingManager;

    [Header("*UI")]
    [SerializeField] public ArrowLRValueType arrowLRValueType;
    [SerializeField] TMP_Text valueTxt;
    [SerializeField] Button LeftBtn;
    [SerializeField] Button RightBtn;


    public override void Awake()
    {
        base.Awake();
        if (GameObject.Find("SettingManager").TryGetComponent(out GameSettingManager GSM))
        { GameSettingManager = GSM; }

        LeftBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetEnumValue(false);
            });
        RightBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetEnumValue(true);
            });
    }


    public void SetEnumValue(bool IsNext)
    {
        if(arrowLRValueType == ArrowLRValueType.Resolution)
        {
            Display_Resolution Temp_DR = GetReso(valueTxt.text);
            Display_Resolution NextValue;
            if (IsNext)
            { NextValue = Enum_Extensions.Next(Temp_DR); }
            else
            { NextValue = Enum_Extensions.Before(Temp_DR); }

            List<int> ResultReso = GameSettingManager.GameSetting.GameSetting_Video.GetDisplayValueByEnum_Reso(NextValue);
            valueTxt.text = ResultReso[0] + " X " + ResultReso[1];
        }

    }

    #region Check

    private Display_Resolution GetReso(string txt)
    {
        string[] s = txt.Split();
        List<int> reso = new List<int>() { Convert.ToInt32(s[0]), Convert.ToInt32(s[2]) };
        Debug.Log(Convert.ToInt32(s[0]) + " " + Convert.ToInt32(s[2]));
        return GameSettingManager.GameSetting.GameSetting_Video.GetDisplayEnumByValue_Reso(reso);
    }

    #endregion

}

public enum ArrowLRValueType
{
    Resolution, FPSLimit
}


