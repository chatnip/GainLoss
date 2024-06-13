//Refactoring v1.0
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : Singleton<PhoneHardware>
{
    #region Value

    [Header("=== UICanvas")]
    [SerializeField] List<GameObject> identicalGOs;
    [SerializeField] List<GameObject> oppositeGOs;

    [Header("=== UI")]
    [SerializeField] Button pauseBtn;

    [Header("=== Effectful")]
    [SerializeField] RectTransform circleEffectRT;
    [SerializeField] RectTransform bellRT;
    [SerializeField] RectTransform waveRT;

    // Other Value
    [HideInInspector] public Dictionary<e_phoneStateExtra, GameObject> turnOnExtraGODict;
    [HideInInspector] public e_phoneStateExtra PhoneStateExtra;
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
        // Dict
        turnOnExtraGODict = new Dictionary<e_phoneStateExtra, GameObject>
        {
            { e_phoneStateExtra.visitPlace,  PhoneSoftware.Instance.visitPlaceScreen },
            { e_phoneStateExtra.option, PhoneSoftware.Instance.optionScreen }
        };
        this.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
    }
    
    #endregion

    #region Effectful

    public IEnumerator Start_PhoneOn(e_phoneStateExtra pse)
    {
        if(!GameManager.Instance.canInput) { yield return null; }
        GameManager.Instance.canInput = false;

        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.ResetAnime();

        PhoneSoftware.Instance.ResetMapIcons();

        Sequence seq = DOTween.Sequence();

        PhoneStateExtra = pse;
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

                // 상황에 맞는 앱 켜지게
                foreach (KeyValuePair<e_phoneStateExtra, GameObject> keyValuePair in turnOnExtraGODict)
                {
                    if (keyValuePair.Value == turnOnExtraGODict[PhoneStateExtra])
                    {
                        keyValuePair.Value.gameObject.SetActive(true); 
                    }
                    else
                    { 
                        keyValuePair.Value.gameObject.SetActive(false); 
                    }
                }

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

        seq.Append(CEImg.DOFade(0, 0.1f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                circleEffectRT.gameObject.SetActive(false); 
                
                GameManager.Instance.canInput = true;

                if (PhoneSoftware.Instance.visitPlaceScreen.gameObject.activeSelf)
                { PhoneSoftware.Instance.OpenMap(); }
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
