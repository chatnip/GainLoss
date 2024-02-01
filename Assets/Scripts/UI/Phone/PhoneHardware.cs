using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : MonoBehaviour, IInteract
{
    #region Value

    [Header("Property")]
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Hardware")]
    [SerializeField] GameObject phoneScreen;
    [SerializeField] GameObject phone2DCamera;
    [SerializeField] PhoneSoftware phoneSoftware;
    [SerializeField] GameObject phoneViewCamera;
    [SerializeField] GameObject quarterViewCamera;
    [SerializeField] GameObject lockScreen;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject Schedules;
    [SerializeField] GameObject ScheduleUIs;

    [Header("*On/Off Btns")]
    [SerializeField] Button PhoneListOpenBtn;
    [SerializeField] Button PhoneOnBtn;
    [SerializeField] Button PhoneOnByScheduleBtn;
    [HideInInspector] public List<Button> PhoneOnButtons;

    List<string> DoNotNeedBtns;
    [HideInInspector] public bool DoNotNeedBtns_ExceptionSituation;
    [HideInInspector] public bool sectionIsThis = false;

    #endregion

    #region Main

    public void Awake()
    {
        this.gameObject.SetActive(false);
        PhoneOnButtons = new List<Button>()
        {
            PhoneOnBtn, PhoneOnByScheduleBtn
        };
        DoNotNeedBtns = new List<string>()
        {
            "S01", "S03", "S04", "S99"
        };
        DoNotNeedBtns_ExceptionSituation = false;

        PhoneListOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SetOnOffPhoneBtn();
            });


        PhoneOnBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();

                phoneSoftware.SetCurrentScheduleUI(false);
            });
        PhoneOnByScheduleBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();

                phoneSoftware.SetCurrentScheduleUI(true);
            });

    }
    

    private void OnEnable()
    {
        PhoneOn();
    }

    private void OnDisable()
    {
        PhoneOff();
    }

    public void Interact()
    {
        if (PlayerInputController.SelectBtn == PhoneOnBtn) 
        {
            SchedulePrograss.ResetExlanation();
            ResetPhoneBtns();
            PhoneOn();

            phoneSoftware.SetCurrentScheduleUI(false);
        }
        else if (PlayerInputController.SelectBtn == PhoneOnByScheduleBtn) 
        {
            SchedulePrograss.ResetExlanation();
            ResetPhoneBtns();
            PhoneOn();

            phoneSoftware.SetCurrentScheduleUI(true);
        }
    }

    #endregion

    #region Reset

    public void ResetPhoneBtns()
    {
        SetOff(PhoneOnBtn, new Vector2(0, 75));
        SetOff(PhoneOnByScheduleBtn, new Vector2(0, 75));

        void SetOff(Button btn, Vector2 endPos)
        {
            btn.interactable = false;
            btn.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
            btn.gameObject.GetComponent<RectTransform>().anchoredPosition = endPos;
            btn.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Effectful

    public void SetOnOffPhoneBtn()
    {
        DOTween.Kill(PhoneOnBtn);
        DOTween.Kill(PhoneOnByScheduleBtn);

        if(!sectionIsThis) // 휴대폰 버튼이 상호작용 버튼들로 적용
        {
            sectionIsThis = true;

            SchedulePrograss.SetByScheduleBtnOwnTxt();
            SetOn(PhoneOnBtn, new Vector2(0, 0));
            PlayerInputController.SetSectionBtns(SetPhoneOnButtons(), this);
            foreach (string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) 
                {
                    return; 
                }
            }
            if (DoNotNeedBtns_ExceptionSituation) { return; }

            SetOn(PhoneOnByScheduleBtn, new Vector2(0, -50));
            PlayerInputController.SetSectionBtns(SetPhoneOnButtons(), this);
            
        }
        else // 휴대폰 버튼이 상호작용 버튼들로 적용 해제
        {

            sectionIsThis = false;

            SetOff(PhoneOnBtn, new Vector2(0, 75));
            PlayerInputController.SetSectionBtns(null, null);
            foreach (string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) 
                {
                    PlayerInputController.SetSectionBtns(null, null);
                    return; 
                }
            }
            if (DoNotNeedBtns_ExceptionSituation) { return; }
            SetOff(PhoneOnByScheduleBtn, new Vector2(0, 75));

            PlayerInputController.SetSectionBtns(null, null);
            PlayerInputController.interact = null;
        }

        void SetOn(Button btn, Vector2 endPos)
        {
            btn.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
            btn.gameObject.SetActive(true);
            btn.gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
            btn.gameObject.GetComponent<RectTransform>().DOAnchorPos(endPos, 0.3f)
                .OnComplete(() =>
                {
                    btn.interactable = true;
                });
        }
        void SetOff(Button btn, Vector2 endPos)
        {
            btn.interactable = false;
            btn.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
            btn.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            btn.gameObject.GetComponent<RectTransform>().DOAnchorPos(endPos, 0.3f)
                .OnComplete(() =>
                {
                    btn.gameObject.SetActive(false);
                });
        }

    }
    
    //Cacul
    

    public List<Button> SetPhoneOnButtons()
    {
        List<Button> buttons = new List<Button>();
        foreach(Button btn in PhoneOnButtons)
        {
            if (btn.gameObject.activeSelf) { buttons.Add(btn); }
            Debug.Log(btn.name);
        }
        return buttons;
    }


    #endregion

    #region Phone On/Off

    public void PhoneOn()
    {
        this.gameObject.SetActive(true);

        phone2DCamera.SetActive(true);
        phoneScreen.SetActive(true);
        phoneViewCamera.SetActive(true);
        quarterViewCamera.SetActive(false);

        phoneSoftware.ResetUI();

        InteractionUI3D.SetActive(false);

        Schedules.gameObject.SetActive(false);

        PhoneListOpenBtn.gameObject.SetActive(false);

        PlayerInputController.CanMove = false;
    }

    public void PhoneOff()
    {
        phone2DCamera.SetActive(false);
        phoneScreen.SetActive(false);
        phoneViewCamera.SetActive(false);
        quarterViewCamera.SetActive(true);

        this.gameObject.SetActive(false);

        InteractionUI3D.SetActive(true);

        Schedules.gameObject.SetActive(true);
        PhoneListOpenBtn.gameObject.SetActive(true);
        sectionIsThis = false;

        PlayerInputController.CanMove = true;
        
    }

    #endregion
}
