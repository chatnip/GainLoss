using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : MonoBehaviour
{
    #region Value

    [Header("Property")]
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ScheduleManager ScheduleManager;

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

    List<string> DoNotNeedBtns;
    [HideInInspector] public bool DoNotNeedBtns_ExceptionSituation;

    #endregion

    #region Main

    public void Awake()
    {
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
            });
        PhoneOnByScheduleBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SchedulePrograss.ResetExlanation();
                ResetPhoneBtns();
                PhoneOn();

                phoneSoftware.SetCurrentScheduleUI();
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

    private void SetOnOffPhoneBtn()
    {
        DOTween.Kill(PhoneOnBtn);
        DOTween.Kill(PhoneOnByScheduleBtn);
        if(!PhoneOnBtn.gameObject.activeSelf && !PhoneOnByScheduleBtn.gameObject.activeSelf)
        {
            SchedulePrograss.SetByScheduleBtnOwnTxt();
            SetOn(PhoneOnBtn, new Vector2(0, 0));
            foreach(string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) { return; }
            }
            if (DoNotNeedBtns_ExceptionSituation) { return; }
            SetOn(PhoneOnByScheduleBtn, new Vector2(0, -50));
        }
        else
        {
            SetOff(PhoneOnBtn, new Vector2(0, 75));
            foreach (string DoNotNeedBtn in DoNotNeedBtns)
            {
                if (DoNotNeedBtn == ScheduleManager.currentPrograssScheduleID) { return; }
            }
            if (DoNotNeedBtns_ExceptionSituation) { return; }
            SetOff(PhoneOnByScheduleBtn, new Vector2(0, 75));
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
    }

    #endregion
}
