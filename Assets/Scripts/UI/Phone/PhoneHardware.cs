//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : Singleton<PhoneHardware>, IInteract
{
    #region Value

    [Header("=== UICanvas")]
    [SerializeField] List<GameObject> identicalGOs;
    [SerializeField] List<GameObject> oppositeGOs;

    [Header("=== Effectful")]
    [SerializeField] RectTransform circleEffectRT;
    [SerializeField] RectTransform bellRT;
    [SerializeField] RectTransform waveRT;

    // Other Value
    public Dictionary<e_phoneStateExtra, GameObject> turnOnExtraGODict;

    #endregion

    #region Enum

    public enum e_phoneStateExtra
    {
        visitPlace, option
    }

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        turnOnExtraGODict = new Dictionary<e_phoneStateExtra, GameObject>
        {
            { e_phoneStateExtra.visitPlace,  PhoneSoftware.Instance.visitPlaceScreen },
            { e_phoneStateExtra.option, PhoneSoftware.Instance.optionScreen }
        };
    }

    protected override void Awake()
    {
        base.Awake();
        this.gameObject.SetActive(false);
    }
    
    #endregion

    #region For Pad

    public void Interact()
    {
        if (!GameManager.Instance.CanInput) { return; }
        
    }

    #endregion

    #region Effectful

    public void Start_PhoneOn(e_phoneStateExtra pse)
    {
        if(!GameManager.Instance.CanInput) { return; }
        GameManager.Instance.CanInput = false;

        PlayerInputController.Instance.StopMove();
        PlayerInputController.Instance.SetSectionBtns(null, null);

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
            if (i % 2 == 0)
            { seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, 15.0f), 0.1f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack)
                    .OnUpdate(() =>
                    {
                        bellRT.rotation.SetLookRotation(Vector3.back);
                    })); }
            else
            { seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, -15.0f), 0.1f, RotateMode.FastBeyond360))
                    .SetEase(Ease.OutBack)
                    .OnUpdate(() =>
                    {
                        bellRT.rotation.SetLookRotation(Vector3.back);
                    }); 
            }
        }
        seq.Append(bellRT.DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 0.1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBack));

        seq.AppendInterval(0.5f);
        seq.Append(bellRT.DOSizeDelta(Vector2.zero, 0.1f)
            .OnComplete(() =>
            {
                PhoneOn();
                turnOnExtraGODict[pse].gameObject.SetActive(true);

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
        foreach(GameObject identicalGO in identicalGOs) { identicalGO.SetActive(true); }
        foreach(GameObject oppositeGO in oppositeGOs) { oppositeGO.SetActive(false); }

        PhoneSoftware.Instance.SetBaseInfo();
    }

    public void PhoneOff()
    {
        this.gameObject.SetActive(false);
        foreach (GameObject identicalGO in identicalGOs) { identicalGO.SetActive(false); }
        foreach (GameObject oppositeGO in oppositeGOs) { oppositeGO.SetActive(true); }
    }

    #endregion
}
