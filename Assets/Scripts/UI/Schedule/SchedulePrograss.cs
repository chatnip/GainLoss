using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SchedulePrograss : MonoBehaviour
{
    #region Value
    
    [Header("*Manger")]
    [SerializeField] ScheduleManager ScheduleManager;

    [Header("Txts")]
    [SerializeField] CanvasGroup currentSchedule;
    [SerializeField] TMP_Text ScheduleAM;
    [SerializeField] TMP_Text SchedulePM;
    [SerializeField] TMP_Text ExplanationTxt;
    [SerializeField] TMP_Text ByScheduleBtnOwnTxt;

    [Header("*Btns")]
    [SerializeField] Button ExplanationBtn;
    [SerializeField] Button ByScheduleBtn;
    [HideInInspector] public bool OnExplanation = false;

    [Header("Imgs")]
    [SerializeField] Image StartImg;
    [SerializeField] Image AMImg;
    [SerializeField] Image PMImg;
    [SerializeField] Image EndImg;
    [SerializeField] Color CurrentPrograssColor;
    [SerializeField] CanvasGroup padControllerCG;

    #endregion


    #region Main

    private void Awake()
    {
        ScheduleManager.PassNextSchedule();
        //Set_InStartScheduleUI();

        ExplanationBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                OnOffVisibleSchedule();
            });
    }

    #endregion

    #region Progress UI

    public void OnOffVisibleSchedule()
    {
        if (OnExplanation) // 표 끄기
        {
            DOTween.Kill(ExplanationTxt);
            DOTween.Kill(currentSchedule);

            // 텍스트 보이기
            ExplanationTxt.gameObject.SetActive(true);
            ExplanationTxt.DOFade(1, 0.5f);

            // 스케쥴 표 올리기
            currentSchedule.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 0.5f);
            currentSchedule.DOFade(0, 0.5f);

            // 패드 키 안내 명도 조절
            DOTween.Kill(padControllerCG);
            padControllerCG.DOFade(0.3f, 0.5f);

            OnExplanation = false;
        }
        else // 표 키기
        {
            if (ScheduleManager.currentPrograssScheduleID != null)
            {
                SetExplanation(ScheduleManager.currentPrograssScheduleID);
            }

            DOTween.Kill(ExplanationTxt);
            DOTween.Kill(currentSchedule);

            // 텍스트 안보이게 하기
            ExplanationTxt.DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    ExplanationTxt.gameObject.SetActive(false);
                });

            // 스케쥴 표 내리기
            currentSchedule.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);
            currentSchedule.DOFade(1, 0.5f);

            // 패드 키 안내 명도 조절
            DOTween.Kill(padControllerCG);
            padControllerCG.DOFade(1, 0.5f);

            OnExplanation = true;
        }
    }

    public void ResetExlanation()
    {
        DOTween.Kill(ExplanationTxt);
        DOTween.Kill(currentSchedule);

        currentSchedule.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
        ExplanationTxt.gameObject.SetActive(true);
        ExplanationTxt.alpha = 1.0f;
        OnExplanation = false;
    }

    public void Set_InStartScheduleUI()
    {
        ScheduleAM.text = "오전 (계획 필요)";
        SchedulePM.text = "오후 (계획 필요)";

        SetProgressingUI(StartImg);
        SetNotProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

    }

    public void Set_InAMScheduleUI()
    {
        ScheduleAM.text = (string)DataManager.ScheduleDatas[3][ScheduleManager.currentSelectedScheduleID[0]];
        SchedulePM.text = (string)DataManager.ScheduleDatas[3][ScheduleManager.currentSelectedScheduleID[1]];

        SetComplatePrograssing(StartImg);
        SetProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);
    }

    public void Set_InPMScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);
    }

    public void Set_InEndScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetComplatePrograssing(PMImg);
        SetProgressingUI(EndImg);
    }

    

    private void SetNotProgressingUI(Image img)
    {
        DOTween.Kill(img.GetComponent<CanvasGroup>());
        img.gameObject.GetComponent<Image>().color = Color.white;
        img.gameObject.GetComponent<CanvasGroup>().alpha = 0.3f;
    }
    private void SetProgressingUI(Image img)
    {
        img.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        img.gameObject.GetComponent<Image>().color = CurrentPrograssColor;
        img.gameObject.GetComponent<CanvasGroup>().DOFade(0.3f, 1.25f).SetLoops(-1, LoopType.Yoyo);
    }
    private void SetComplatePrograssing(Image img)
    {
        DOTween.Kill(img.GetComponent<CanvasGroup>());
        img.gameObject.GetComponent<Image>().color = Color.white;
        img.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }


    public void SetExplanation(string id)
    {
        ExplanationTxt.text = (string)DataManager.ScheduleDatas[4][id];
    }
    public void SetByScheduleBtnOwnTxt()
    {
        ByScheduleBtnOwnTxt.text = (string)DataManager.ScheduleDatas[3][ScheduleManager.currentPrograssScheduleID];
    }

    #endregion



}
