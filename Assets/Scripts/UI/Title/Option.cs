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
    [HideInInspector] public Dictionary<GameObject, dele> DataSaveAndApplyDeleByGODict;
    

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
        DataSaveAndApplyDeleByGODict = new Dictionary<GameObject, dele>()
        {
            { GameSetting, new dele(GameSettingManager.Apply_Gs) },
            { AudioSetting, new dele(GameSettingManager.Apply_As) },
            { VideoSetting, new dele(GameSettingManager.Apply_Vs) },
            { CreditSetting, new dele(GameSettingManager.Apply_Cs) }
        };
        GOByBtns = new Dictionary<GameObject, List<List<Button>>>
        {
            { VideoSetting, new List<List<Button>> {
                new List<Button> { fullScreen_BtnPanel },
                new List<Button> { resolution_BtnPanel },
                new List<Button> { FPSLimit_BtnPanel },
                new List<Button> { showFPS_BtnPanel }}}
            
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
                    Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[SettingBtn]], this);

                    FindAndTurnOn_Setting(ButtonByGODict[SettingBtn]);
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
                Apply_OptionDetail();
            });
        
    }

    public void Interact()
    {
        if(CheckIsOnOptionDetail() != null)
        {
            if(Title.TitleInputController.SelectBtn.TryGetComponent(out ToggleInteractBtn TIB))
            {
                bool ToggleOn = TIB.thisToggle.isOn;
                if (ToggleOn) { TIB.thisToggle.isOn = false; }
                else { TIB.thisToggle.isOn = true; }
                TIB.SetToggleUI(TIB.thisToggle.isOn);
            }

            return;
        }

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
                Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[selected]], this);

                FindAndTurnOn_Setting(ButtonByGODict[selected]);
            }
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
        Debug.Log("Video Setting On");
        // 현재 값 출력
        GameSetting_Video GS_V = GameSettingManager.GameSetting.GameSetting_Video;
        fullScreen.IsOnOff(GS_V.FullScreen);
        resolution.setText(
            GS_V.GetDisplayValueByEnum_Reso(GS_V.display_Resolution)[0] + " X " + 
            GS_V.GetDisplayValueByEnum_Reso(GS_V.display_Resolution)[1]);
        FPSLimit.setText(GS_V.GetDisplayValueByEnum_Fps(GS_V.display_FPSLimit) + "fps");
        showFPS.IsOnOff(GS_V.ShowFPS);

        /*// 현재 값에 따른 InteractBtn DataValue 삽입
        List<Button> Btns = Title.TitleInputController.AllSectionBtns();
        foreach(Button Btn in Btns)
        {
            if(Btn.TryGetComponent(out ArrowLRInteractBtn ALR_IB))
            {
                ALR_IB.SetEnumValue();
            }
            else if(Btn.TryGetComponent(out ToggleInteractBtn T_IB))
            {
                T_IB.SetToggleUI();
            }
        }*/
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

        GameObject OnDetailOptionWindow = CheckIsOnOptionDetail();
        if (OnDetailOptionWindow != null)
        {
            OnDetailOptionWindow.SetActive(false);
            OnEnable();
            return false;
        }
        else 
        { 
            return true; 
        }
        
    }
    public GameObject CheckIsOnOptionDetail()
    {
        foreach (KeyValuePair<Button, GameObject> BtnByGo in ButtonByGODict)
        {
            if (BtnByGo.Value.gameObject.activeSelf)
            {
                return BtnByGo.Value;
            }
        }
        return null;
    }

    #endregion

    #region Btn
    public void Apply_OptionDetail()
    {
        foreach (KeyValuePair<GameObject, dele> BtnByGo in DataSaveAndApplyDeleByGODict)
        {
            if (BtnByGo.Key.gameObject.activeSelf)
            {
                BtnByGo.Value();
            }
        }
    }
    public void Cancel_OptionDetail()
    {

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