using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    [Header("*SetBtn")]
    [SerializeField] Button CancelBtn;
    [SerializeField] Button ApplyBtn;

    public delegate void dele();
    [HideInInspector] public Dictionary<Button, GameObject> ButtonByGODict;
    [HideInInspector] public Dictionary<GameObject, dele> DeleByGODict;
    

    #endregion

    #region Main

    private void Awake()
    {
        ButtonByGODict = new Dictionary<Button, GameObject>()
        {
            { GameBtn, GameSetting},
            { AudioBtn, AudioSetting},
            { VideoBtn, VideoSetting},
            { CreditBtn, CreditSetting}
        };
        DeleByGODict = new Dictionary<GameObject, dele>()
        {
            { GameSetting, new dele(GameSettingManager.SetGameSetting) },
            { AudioSetting, new dele(GameSettingManager.SetAudioSetting) },
            { VideoSetting, new dele(GameSettingManager.SetVedioSetting) },
            { CreditSetting, new dele(GameSettingManager.SetCreditSetting) }
        };

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
                    //Title.TitleInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { CancelBtn, ApplyBtn } }, this);
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

        if (Title.TitleInputController.SelectBtn == BackBtn && BackBtn.interactable)
        {
            SetOffOption();
            Title.SetButtonThisTitle();
        }

        // 오른쪽 Cancel, Apply

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
            }
        }
    }

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
    public void ApplyOptionDetail()
    {
        foreach (KeyValuePair<GameObject, dele> BtnByGo in DeleByGODict)
        {
            if (BtnByGo.Key.gameObject.activeSelf)
            {
                BtnByGo.Value();
            }
        }
    }
    private void setOffBtns(List<List<Button>> setOffBtns)
    {
        foreach(List<Button> btnList in setOffBtns)
        {
            foreach(Button btn in btnList)
            {
                btn.interactable = false;
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
}
