using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class Option : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] Title Title;
    [SerializeField] GameSettingManager GameSettingManager;

    [Header("*Window")]
    [SerializeField] GameObject GameSetting;
    [SerializeField] GameObject AudioSetting;
    [SerializeField] GameObject VideoSetting;
    [SerializeField] GameObject CreditSetting;

    [Header("*OptionBtn")]
    [SerializeField] Button GameBtn;
    [SerializeField] Button AudioBtn;
    [SerializeField] Button VideoBtn;
    [SerializeField] Button CreditBtn;
    [SerializeField] Button BackBtn;

    [Header("*Video")]
    [SerializeField] Button fullScreen_BtnPanel;
    [SerializeField] Button resolution_BtnPanel;
    [SerializeField] Button FPSLimit_BtnPanel;
    [SerializeField] Button showFPS_BtnPanel;
    [Header("*VideoDetail")]
    [SerializeField] ToggleValue fullScreen;
    [SerializeField] LeftRightBtnValue resolution;
    [SerializeField] LeftRightBtnValue FPSLimit;
    [SerializeField] ToggleValue showFPS;


    Dictionary<GameObject, List<List<Button>>> GOByBtns;

    [Header("*SetBtn")]
    [SerializeField] Button CancelBtn;
    [SerializeField] Button ApplyBtn;

    public delegate void dele();
    [HideInInspector] public Dictionary<Button, GameObject> ButtonByGODict;
    [HideInInspector] public Dictionary<GameObject, dele> LoadAndSetDeleByGODict;
    

    #endregion

    #region Main
    private void setData()
    {
        ButtonByGODict = new Dictionary<Button, GameObject>()
        {
            { GameBtn, GameSetting},
            { AudioBtn, AudioSetting},
            { VideoBtn, VideoSetting},
            { CreditBtn, CreditSetting}
        };
        LoadAndSetDeleByGODict = new Dictionary<GameObject, dele>()
        {
            { GameSetting, new dele(GameSettingManager.LoadAndSet_GameSetting) },
            { AudioSetting, new dele(GameSettingManager.LoadAndSet_AudioSetting) },
            { VideoSetting, new dele(GameSettingManager.LoadAndSet_VedioSetting) },
            { CreditSetting, new dele(GameSettingManager.LoadAndSet_CreditSetting) }
        };
        GOByBtns = new Dictionary<GameObject, List<List<Button>>>
        {
            { VideoSetting, new List<List<Button>> { 
                new List<Button> {fullScreen_BtnPanel },
                new List<Button> {resolution_BtnPanel },
                new List<Button> {FPSLimit_BtnPanel },
                new List<Button> {showFPS_BtnPanel }}}
            
        };
    }

    private void Awake()
    {
        setData();

        // 왼쪽 리스트 클릭 셋
        foreach (Button SettingBtn in ButtonByGODict.Keys)
        {
            SettingBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    setOffBtns(Title.TitleInputController.SectionBtns);
                    foreach (GameObject SettingGO in ButtonByGODict.Values)
                    {
                        SettingGO.gameObject.SetActive(false);
                    }
                    ButtonByGODict[SettingBtn].gameObject.SetActive(true);
                    CancelBtn.interactable = true;
                    ApplyBtn.interactable = true;
                    FindAndTurnOn_Setting(ButtonByGODict[SettingBtn]);

                    Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[SettingBtn]], this);
                    
                });
        }
        BackBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetOffOption();
            });

        // 오른쪽 Cancel, Apply
        CancelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetOffOptionDetail();
            });
        ApplyBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ApplyOptionDetail();
            });
        
    }

    public void Interact()
    {
        // 왼쪽 리스트 클릭 셋
        ApplyBtnSet(Title.TitleInputController.SelectBtn);
        void ApplyBtnSet(Button selected)
        {
            if (Title.TitleInputController.SelectBtn == selected && selected.interactable)
            {
                setOffBtns(Title.TitleInputController.SectionBtns);
                foreach (GameObject SettingGO in ButtonByGODict.Values)
                {
                    SettingGO.gameObject.SetActive(false);
                }
                ButtonByGODict[selected].gameObject.SetActive(true);
                CancelBtn.interactable = true;
                ApplyBtn.interactable = true;
                FindAndTurnOn_Setting(ButtonByGODict[selected]);

                Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[selected]], this);
            }
        }


        if (Title.TitleInputController.SelectBtn == BackBtn && BackBtn.interactable)
        {
            SetOffOption();
            Title.SetButtonThisTitle();
        }
    }

    private void OnEnable()
    {
        foreach(KeyValuePair<Button, GameObject> ButtonAndGO in ButtonByGODict)
        {
            ButtonAndGO.Key.interactable = true;
            BackBtn.interactable = true;

            ButtonAndGO.Value.gameObject.SetActive(false);

            CancelBtn.interactable = false;
            ApplyBtn.interactable = false;

        }

        Title.TitleInputController.SetSectionBtns(new List<List<Button>>
        { 
            new List<Button> { GameBtn },
            new List<Button> { AudioBtn },
            new List<Button> { VideoBtn },
            new List<Button> { CreditBtn }
        }, this);
    }

    #endregion

    #region Turn On

    private void FindAndTurnOn_Setting(GameObject go)
    {
        if(go == VideoSetting)
        { TurnOn_VideoSetting(); }
    }

    private void TurnOn_VideoSetting()
    {
        GameSetting_Video GS_V = GameSettingManager.GameSetting.GameSetting_Video;
        fullScreen.IsOnOff(GS_V.FullScreen);
        resolution.setText(GS_V.GetDisplayValueByEnum(GS_V.display_Resolution)[0] + " X " + GS_V.GetDisplayValueByEnum(GS_V.display_Resolution)[1]);
        FPSLimit.setText(GS_V.GetDisplayValueByEnum(GS_V.display_FPSLimit) + "fps");
        showFPS.IsOnOff(GS_V.ShowFPS);
    }

    #endregion

    #region Turn Off

    public void SetOffOption()
    {
        EffectfulWindow.DisappearEffectful(this.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
        Title.SetButtonThisTitle();
        Title.TitleInputController.SelectBtn = Title.OptionBtn;
        Title.TitleInputController.OnOffSelectedBtn(Title.OptionBtn);
    }
    public bool SetOffOptionDetail()
    {
        foreach (KeyValuePair<Button, GameObject> BtnByGo in ButtonByGODict)
        {
            if (BtnByGo.Value.gameObject.activeSelf)
            {
                BtnByGo.Value.gameObject.SetActive(false);
                OnEnable();
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Btn
    public void ApplyOptionDetail()
    {
        foreach (KeyValuePair<GameObject, dele> BtnByGo in LoadAndSetDeleByGODict)
        {
            if (BtnByGo.Key.gameObject.activeSelf)
            {
                BtnByGo.Value();
            }
        }
    }

    private void setOffBtns(List<List<Button>> setOffBtns)
    {
        foreach (List<Button> btnList in setOffBtns)
        {
            foreach (Button btn in btnList)
            {
                btn.interactable = false;
            }
        }
    }

    #endregion
}

[System.Serializable]
public class LeftRightBtnValue
{
    public TMP_Text ValueTxt;
    public Button LeftBtn;
    public Button RightBtn;
    public void setText(string s)
    {
        ValueTxt.text = s;
    }
}

[System.Serializable]
public class ToggleValue
{
    public TMP_Text ValueTxt;
    public Toggle Toggle;
    public void IsOnOff(bool Is)
    {
        Toggle.isOn = Is;
        if (Is) { ValueTxt.text = "ON"; }
        else { ValueTxt.text = "OFF"; }
    }
}