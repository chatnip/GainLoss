using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] GameSystem GameSystem;

    [Header("*Hardware")]
    [SerializeField] GameObject phoneScreen;
    [SerializeField] public GameObject phone2DCamera;
    [SerializeField] PhoneSoftware phoneSoftware;
    [SerializeField] GameObject phoneViewCamera;
    [SerializeField] GameObject quarterViewCamera;
    [SerializeField] GameObject lockScreen;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject Schedules;
    [SerializeField] GameObject ScheduleUIs;


    [Header("*Effectful Phone")]
    [SerializeField] RectTransform circleEffectRT;
    [SerializeField] RectTransform bellRT;
    [SerializeField] RectTransform waveRT;

    [Header("*On/Off Btns")]
    [SerializeField] public Button PhoneListOpenBtn;
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
        PlayerInputController.StopMove();

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
                PlayerInputController.OnOffPhone();
                //SetOnOffPhoneBtn();
            });


        PhoneOnBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                phoneEffectfulCor(false);

                /*SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();

                phoneSoftware.SetCurrentScheduleUI(false);*/
            });
        PhoneOnByScheduleBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                phoneEffectfulCor(true);

                /*SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();

                phoneSoftware.SetCurrentScheduleUI(true);*/
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
            phoneEffectfulCor(false);

            /*SchedulePrograss.ResetExlanation();
            ResetPhoneBtns();
            PhoneOn();

            phoneSoftware.SetCurrentScheduleUI(false);*/
        }
        else if (PlayerInputController.SelectBtn == PhoneOnByScheduleBtn) 
        {
            phoneEffectfulCor(true);

            /*SchedulePrograss.ResetExlanation();
            ResetPhoneBtns();
            PhoneOn();

            phoneSoftware.SetCurrentScheduleUI(true);*/
        }
    }

    #endregion

    #region Reset

    public void ResetPhoneBtns()
    {
        PhoneOnBtn.TryGetComponent(out RectTransform PhoneOnBtnRT);
        PhoneOnByScheduleBtn.TryGetComponent(out RectTransform PhoneOnByScheduleBtnRT);
        SetOff(PhoneOnBtn, new Vector2(0, PhoneOnBtnRT.anchoredPosition.y));
        SetOff(PhoneOnByScheduleBtn, new Vector2(0, PhoneOnByScheduleBtnRT.anchoredPosition.y));

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

    public void phoneEffectfulCor(bool setCurrentScheduleUI)
    {
        PlayerInputController.SetSectionBtns(null, null);

        Sequence seq = DOTween.Sequence();

        circleEffectRT.anchoredPosition = PhoneListOpenBtn.GetComponent<RectTransform>().anchoredPosition;
        circleEffectRT.sizeDelta = Vector2.zero;
        circleEffectRT.gameObject.SetActive(true); 
        circleEffectRT.TryGetComponent(out Image CEImg);
        waveRT.TryGetComponent(out Image WImg);

        CEImg.DOFade(1, 0);
        WImg.DOFade(1, 0);

        bellRT.sizeDelta = Vector2.zero;
        bellRT.gameObject.SetActive(true);

        seq.Append(circleEffectRT.DOSizeDelta(Vector2.one * 4000f, 0.8f));
        seq.Append(bellRT.DOSizeDelta(new Vector2(0.75f, 1f)* 100, 0.1f));
        for (int i = 0; i < 4; i++)
        {
            //bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, 360.0f * i), 20.0f, RotateMode.FastBeyond360);
            if (i % 2 == 0)
            { seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, 15.0f), 0.1f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack)
                    .OnUpdate(() =>
                    {
                        bellRT.rotation.SetLookRotation(Vector3.back);
                        //float z = bellRT.rotation.z;
                        //bellRT.rotation = Quaternion.Euler(new Vector3(0, 0, z));
                    })); }
            else
            { seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, -15.0f), 0.1f, RotateMode.FastBeyond360))
                    .SetEase(Ease.OutBack)
                    .OnUpdate(() =>
                    {
                        bellRT.rotation.SetLookRotation(Vector3.back);
                        //float z = bellRT.rotation.z;
                        //bellRT.rotation = Quaternion.Euler(new Vector3(0, 0, z));
                    }); 
            }
        }
        seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 0.1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBack));

        seq.AppendInterval(0.5f);
        seq.Append(bellRT.DOSizeDelta(Vector2.zero, 0.1f)
            .OnComplete(() =>
            {
                //SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();
                phoneSoftware.SetCurrentScheduleUI(setCurrentScheduleUI);

                waveRT.gameObject.SetActive(true);
                waveRT.sizeDelta = Vector2.zero;

                bellRT.gameObject.SetActive(false);
            }));
        
        seq.Append(waveRT.DOSizeDelta(Vector2.one * 200, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                waveRT.gameObject.SetActive(false);
            }));
        seq.Join(WImg.DOFade(0, 0.5f)).SetEase(Ease.OutCubic);



        seq.Append(CEImg.DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                circleEffectRT.gameObject.SetActive(false);
            }));
    }
    public void SetOnOffPhoneBtn()
    {
        DOTween.Kill(PhoneOnBtn);
        DOTween.Kill(PhoneOnByScheduleBtn);

        if(!sectionIsThis) // 휴대폰 버튼이 상호작용 버튼들로 적용
        {
            sectionIsThis = true;

            SchedulePrograss.SetByScheduleBtnOwnTxt();
            SetOn(PhoneOnBtn, new Vector2(PhoneOnBtn.GetComponent<RectTransform>().rect.width * 1.1f, PhoneOnBtn.GetComponent<RectTransform>().anchoredPosition.y));
            PlayerInputController.SetSectionBtns(SetPhoneOnButtons(), this);
            foreach (string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) 
                {
                    return; 
                }
            }
            if (DoNotNeedBtns_ExceptionSituation || ScheduleManager.currentPrograssScheduleComplete) { return; }

            SetOn(PhoneOnByScheduleBtn, new Vector2(PhoneOnByScheduleBtn.GetComponent<RectTransform>().rect.width * 1.1f, PhoneOnByScheduleBtn.GetComponent<RectTransform>().anchoredPosition.y));
            PlayerInputController.SetSectionBtns(SetPhoneOnButtons(), this);
            
        }
        else // 휴대폰 버튼이 상호작용 버튼들로 적용 해제
        {

            sectionIsThis = false;

            SetOff(PhoneOnBtn, new Vector2(0.0f, PhoneOnBtn.GetComponent<RectTransform>().anchoredPosition.y));
            PlayerInputController.SetSectionBtns(null, null);
            foreach (string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) 
                {
                    PlayerInputController.SetSectionBtns(null, null);
                    return; 
                }
            }
            if (DoNotNeedBtns_ExceptionSituation || ScheduleManager.currentPrograssScheduleComplete) { return; }
            SetOff(PhoneOnByScheduleBtn, new Vector2(0.0f, PhoneOnByScheduleBtn.GetComponent<RectTransform>().anchoredPosition.y));

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
    

    public List<List<Button>> SetPhoneOnButtons()
    {
        List<List<Button>> buttons = new List<List<Button>>();
        foreach(Button btn in PhoneOnButtons)
        {
            if (btn.gameObject.activeSelf) { buttons.Add(new List<Button> { btn }); }
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

        InteractionUI3D.SetActive(false);

        Schedules.gameObject.SetActive(false);

        PhoneListOpenBtn.gameObject.SetActive(false);

        PlayerInputController.StopMove();

        ScheduleManager.ResetDotweenGuide();
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

        ScheduleManager.SetDotweenGuide(); 
    }

    #endregion
}
