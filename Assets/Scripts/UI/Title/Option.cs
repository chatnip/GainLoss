
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;

public class Option : MonoBehaviour
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

    [Header("*Result")]
    [SerializeField] CanvasGroup DoneCG;
    [SerializeField] RectTransform DoneFrameRT;
    [SerializeField] TMP_Text TypeTxt;
    [SerializeField] TMP_Text AnnoTxt;

    [Header("*OptionBtn")]
    [SerializeField] Button GameBtn;
    [SerializeField] Button AudioBtn;
    [SerializeField] Button VideoBtn;
    [SerializeField] Button CreditBtn;
    [SerializeField] Button BackBtn;
    [SerializeField] Button OkBtn;

    [Header("*Game")]
    [SerializeField] Button showPadGuide_BtnPanel;
    [SerializeField] Button showTutorialGuide_BtnPanel;
    [SerializeField] Button IsOnBG_of3D_BtnPanel;
    [SerializeField] Button MainUIScale_BtnPanel;

    [Header("*Video")]
    [SerializeField] Button fullScreen_BtnPanel;
    [SerializeField] Button resolution_BtnPanel;
    [SerializeField] Button FPSLimit_BtnPanel;
    [SerializeField] Button showFPS_BtnPanel;


    Dictionary<GameObject, List<List<Button>>> GOByBtns;

    [Header("*SetBtn")]
    [SerializeField] Button CancelBtn;
    [SerializeField] Button ApplyBtn;
    [SerializeField] TMP_Text IsChangedTxt;
    

    public delegate void dele();
    [HideInInspector] public Dictionary<Button, GameObject> ButtonByGODict;
    [HideInInspector] public Dictionary<GameObject, dele> WhenTurnOn_DeleByGODict;
    [HideInInspector] public Dictionary<GameObject, dele> Apply_DeleByGODict;
    [HideInInspector] public Dictionary<GameObject, dele> Save_DeleByGODict;


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
        WhenTurnOn_DeleByGODict = new Dictionary<GameObject, dele>
        {
            { GameSetting, new dele(TurnOn_GameSetting) },
            { AudioSetting, new dele(TurnOn_AudioSetting) },
            { VideoSetting, new dele(TurnOn_VideoSetting) },
            { CreditSetting, new dele(TurnOn_CreditSetting) }
        };
        Apply_DeleByGODict = new Dictionary<GameObject, dele>()
        {
            { GameSetting, new dele(GameSettingManager.Apply_Gs) },
            { AudioSetting, new dele(GameSettingManager.Apply_As) },
            { VideoSetting, new dele(GameSettingManager.Apply_Vs) },
            { CreditSetting, new dele(GameSettingManager.Apply_Cs) }
        };
        Save_DeleByGODict = new Dictionary<GameObject, dele>()
        {
            { GameSetting, new dele(InitData_Gs) },
            { AudioSetting, new dele(InitData_As) },
            { VideoSetting, new dele(InitData_Vs) },
            { CreditSetting, new dele(InitData_Cs) }
        };
        GOByBtns = new Dictionary<GameObject, List<List<Button>>>
        {
            { GameSetting, new List<List<Button>> {
                new List<Button> { showPadGuide_BtnPanel },
                new List<Button> { showTutorialGuide_BtnPanel },
                new List<Button> { IsOnBG_of3D_BtnPanel },
                new List<Button> { MainUIScale_BtnPanel }}},
            { AudioSetting, new List<List<Button>> {
                new List<Button> {  },
                new List<Button> {  },
                new List<Button> {  },
                new List<Button> {  }}},
            { VideoSetting, new List<List<Button>> {
                new List<Button> { fullScreen_BtnPanel },
                new List<Button> { resolution_BtnPanel },
                new List<Button> { FPSLimit_BtnPanel },
                new List<Button> { showFPS_BtnPanel }}}
        };
    }
    public void CanSave(bool _b)
    {
        Debug.Log("CanSave" + _b);
        IsChangedTxt.gameObject.SetActive(_b);
        IsInteractable(ApplyBtn, _b);
    }
    private void IsInteractable(Button btn, bool _b)
    {
        btn.interactable = _b;
        if (btn.transform.GetChild(0).TryGetComponent(out Image img))
        {
            if (_b) { img.DOFade(1f, 0f); }
            else { img.DOFade(0.25f, 0f); }
        }
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
                    IsInteractable(CancelBtn, true);
                    IsInteractable(ApplyBtn, false);
                    //Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[SettingBtn]], this);

                    WhenTurnOn_DeleByGODict[ButtonByGODict[SettingBtn]]();
                    //FindAndTurnOn_Setting(ButtonByGODict[SettingBtn]);
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
                Cancel_OptionDetail();
            });
        ApplyBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Apply_OptionDetail();
            });
        OkBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Disappear_Window();
            });
        
    }

    /*public void Interact()
    {
        // 적용 or 적용안됨 창이 켜져있을 때
        if(Title.TitleInputController.SelectBtn == OkBtn && OkBtn.interactable)
        {
            Disappear_Window();
        }

        
        if(CheckIsOnOptionDetail() != null)
        {
            //토글형식
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
                IsInteractable(CancelBtn, true);
                IsInteractable(ApplyBtn, false);
                Title.TitleInputController.SetSectionBtns(GOByBtns[ButtonByGODict[selected]], this);

                WhenTurnOn_DeleByGODict[ButtonByGODict[selected]]();
                //FindAndTurnOn_Setting(ButtonByGODict[selected]);
            }
        }
    }*/

    private void OnEnable()
    {
        foreach(KeyValuePair<Button, GameObject> ButtonAndGO in ButtonByGODict)
        {
            IsInteractable(ButtonAndGO.Key, true);
            IsInteractable(BackBtn, true);

            ButtonAndGO.Value.gameObject.SetActive(false);

            IsInteractable(CancelBtn, false); 
            IsInteractable(ApplyBtn, false);

        }

        /*Title.TitleInputController.SetSectionBtns(new List<List<Button>>
        { 
            new List<Button> { GameBtn },
            new List<Button> { AudioBtn },
            new List<Button> { VideoBtn },
            new List<Button> { CreditBtn }
        }, this);*/
    }

    #endregion

    #region Init InData

    // 데이터 삽입 후 Json 저장
    // 적용은 GameSettingManager에서 진행

    public void InitData_Gs()
    {
        if (showPadGuide_BtnPanel.TryGetComponent(out ToggleInteractBtn spg_TIB))
        { GameSettingManager.GameSetting.GameSetting_Game.ShowGuidePadUI = spg_TIB.thisToggle.isOn; }
        if (showTutorialGuide_BtnPanel.TryGetComponent(out ToggleInteractBtn stg_AIB))
        { GameSettingManager.GameSetting.GameSetting_Game.ShowGuideTutorial = stg_AIB.thisToggle.isOn; }
        if (IsOnBG_of3D_BtnPanel.TryGetComponent(out ToggleInteractBtn iob_AIB))
        { GameSettingManager.GameSetting.GameSetting_Game.IsOnBG_of3D = iob_AIB.thisToggle.isOn; }
        if (MainUIScale_BtnPanel.TryGetComponent(out SliderInteractBtn mus_SIB))
        { GameSettingManager.GameSetting.GameSetting_Game.MainUIScale = mus_SIB.GetPercentValue_fromSilderValue(mus_SIB.thisSlider.value); }

        JsonManager.Instance.JsonSave(JsonManager.Instance.json_SettingFileName, GameSettingManager.GameSetting);

        Appear_Window_WhenApply("게임 세팅", "저장 및 적용");
    }
    public void InitData_As()
    {

    }
    public void InitData_Vs()
    {
        if (fullScreen_BtnPanel.TryGetComponent(out ToggleInteractBtn fs_TIB))
        { GameSettingManager.GameSetting.GameSetting_Video.FullScreen = fs_TIB.thisToggle.isOn; }
        if (resolution_BtnPanel.TryGetComponent(out ArrowLRInteractBtn re_AIB))
        { GameSettingManager.GameSetting.GameSetting_Video.display_Resolution = re_AIB.GetReso(re_AIB.valueTxt.text); }
        if (FPSLimit_BtnPanel.TryGetComponent(out ArrowLRInteractBtn fl_AIB))
        { GameSettingManager.GameSetting.GameSetting_Video.display_FPSLimit = fl_AIB.GetFpsLimit(fl_AIB.valueTxt.text); }
        if (showFPS_BtnPanel.TryGetComponent(out ToggleInteractBtn sf_TIB))
        { GameSettingManager.GameSetting.GameSetting_Video.ShowFPS = sf_TIB.thisToggle.isOn; }

        JsonManager.Instance.JsonSave(JsonManager.Instance.json_SettingFileName, GameSettingManager.GameSetting);

        Appear_Window_WhenApply("비디오 세팅", "저장 및 적용");
    }
    public void InitData_Cs()
    {

    }

    #endregion

    #region Turn On

    // 타입별 킬때 초기화

    private void TurnOn_GameSetting()
    {
        Debug.Log("Video Setting On");
        // 현재 값 출력
        GameSetting_Game GS_G = GameSettingManager.GameSetting.GameSetting_Game;
        if (showPadGuide_BtnPanel.TryGetComponent(out ToggleInteractBtn spg_TIB))
        { spg_TIB.ResetUI(GS_G.ShowGuidePadUI); }
        if (showTutorialGuide_BtnPanel.TryGetComponent(out ToggleInteractBtn stg_AIB))
        { stg_AIB.ResetUI(GS_G.ShowGuideTutorial); }
        if (IsOnBG_of3D_BtnPanel.TryGetComponent(out ToggleInteractBtn iob_AIB))
        { iob_AIB.ResetUI(GS_G.IsOnBG_of3D); }
        if (MainUIScale_BtnPanel.TryGetComponent(out SliderInteractBtn mus_SIB)) 
        { mus_SIB.ResetUI(GS_G.MainUIScale); }
    }
    private void TurnOn_AudioSetting()
    {

    }
    private void TurnOn_VideoSetting()
    {
        Debug.Log("Video Setting On");
        // 현재 값 출력
        GameSetting_Video GS_V = GameSettingManager.GameSetting.GameSetting_Video;
        if(fullScreen_BtnPanel.TryGetComponent(out ToggleInteractBtn fs_TIB)) 
        { fs_TIB.ResetUI(GS_V.FullScreen); }
        if (resolution_BtnPanel.TryGetComponent(out ArrowLRInteractBtn re_AIB))
        { re_AIB.ResetUI(GS_V.GetDisplayValueByEnum_Reso()[0] + " X " + GS_V.GetDisplayValueByEnum_Reso()[1]); }
        if (FPSLimit_BtnPanel.TryGetComponent(out ArrowLRInteractBtn fl_AIB))
        { fl_AIB.ResetUI(GS_V.GetDisplayValueByEnum_Fps(GS_V.display_FPSLimit) + "fps"); }
        if (showFPS_BtnPanel.TryGetComponent(out ToggleInteractBtn sf_TIB))
        { sf_TIB.ResetUI(GS_V.ShowFPS); }
    }
    private void TurnOn_CreditSetting()
    {

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
        if (!IsChangedTxt.gameObject.activeSelf)
        { return; }

        foreach (GameObject WindowGO in Apply_DeleByGODict.Keys)
        {
            if (WindowGO.gameObject.activeSelf)
            {
                CanSave(false);
                Save_DeleByGODict[WindowGO]();
                Apply_DeleByGODict[WindowGO]();
            }
        }
    }
    public bool Cancel_OptionDetail()
    {
        GameObject OnDetailOptionWindow = CheckIsOnOptionDetail();
        if (OnDetailOptionWindow != null)
        {
            CanSave(false);
            OnDetailOptionWindow.SetActive(false);
            OnEnable();
            return false;
        }
        else
        { return true; }
    }

    private void Appear_Window_WhenApply(string Type, string Anno)
    {
        DoneCG.alpha = 0;
        DoneCG.gameObject.SetActive(true);
        TypeTxt.text = "<#7F18FF>" + Type + "</color>";
        AnnoTxt.text = "<#7F0000><b>" + Anno + "</b></color>\n되었습니다!";
        DoneFrameRT.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        DoneCG.DOFade(1f, 0.12f);
        DoneFrameRT.DOScale(1f, 0.12f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                OkBtn.interactable = true;  
            });

        //Title.TitleInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { OkBtn } }, this);
    }
    private void Appear_Window_WhenCancel()
    {

    }
    private void Disappear_Window()
    {
        OkBtn.interactable = false; 
        DoneCG.DOFade(0f, 0.12f);
        DoneFrameRT.DOScale(0.7f, 0.12f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                DoneCG.gameObject.SetActive(false);
            });
        OnEnable();
    }
    

    private void setOffBtns(List<List<Button>> setOffBtns)
    {
        foreach (List<Button> btnList in setOffBtns)
        {
            foreach (Button btn in btnList)
            {
                if(btn.TryGetComponent(out Outline ol))
                { ol.enabled = false; }
                IsInteractable(btn, false);
                //btn.interactable = false;
            }
        }
    }

    #endregion
}