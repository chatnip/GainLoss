using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : Singleton<PhoneHardware>, IInteract
{
    #region Value

    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ActivityController ActivityController;

    [Header("*Hardware")]
    [SerializeField] GameObject phoneScreen;
    [SerializeField] public GameObject phone2DCamera;
    [SerializeField] PhoneSoftware phoneSoftware;
    [SerializeField] GameObject phoneViewCamera;
    [SerializeField] GameObject quarterViewCamera;
    [SerializeField] GameObject lockScreen;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject ScheduleUIs;
    [SerializeField] GameObject IconCollectionGO;


    [Header("*Effectful Phone")]
    [SerializeField] RectTransform circleEffectRT;
    [SerializeField] RectTransform bellRT;
    [SerializeField] RectTransform waveRT;

    List<string> DoNotNeedBtns;
    [HideInInspector] public bool DoNotNeedBtns_ExceptionSituation;
    [HideInInspector] public bool sectionIsThis = false;

    #endregion

    #region Main

    protected override void Awake()
    {
        base.Awake();
        this.gameObject.SetActive(false);
        PlayerInputController.StopMove();

        DoNotNeedBtns = new List<string>()
        {
            "S01", "S03", "S04", "S99"
        };
        DoNotNeedBtns_ExceptionSituation = false;
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
        if (!GameManager.Instance.CanInput) { return; }
        
    }

    #endregion

    #region Reset

    public void ResetPhoneBtns()
    {
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
        if(!GameManager.Instance.CanInput) { return; }
        GameManager.Instance.CanInput = false;

        PlayerInputController.SetSectionBtns(null, null);

        Sequence seq = DOTween.Sequence();

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
                GameManager.Instance.CanInput = true;
            }));
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



        PlayerInputController.StopMove();

        ActivityController.gameObject.SetActive(false);
        IconCollectionGO.gameObject.SetActive(false);

    }

    public void PhoneOff()
    {
        phone2DCamera.SetActive(false);
        phoneScreen.SetActive(false);
        phoneViewCamera.SetActive(false);
        quarterViewCamera.SetActive(true);

        this.gameObject.SetActive(false);

        InteractionUI3D.SetActive(true);

        sectionIsThis = false;

        ActivityController.gameObject.SetActive(true);
        IconCollectionGO.gameObject.SetActive(true);

        PlayerInputController.CanMove = true;

    }

    #endregion
}
