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
    [SerializeField] ScheduleManager scheduleManager;

    [Header("Txts")]
    [SerializeField] TMP_Text ScheduleAM;
    [SerializeField] TMP_Text SchedulePM;
    [SerializeField] TMP_Text ExplanationTxt;

    [Header("*Btns")]
    [SerializeField] Button ExplanationBtn;
    bool OnExplanation = false;

    [Header("Imgs")]
    [SerializeField] Image StartImg;
    [SerializeField] Image AMImg;
    [SerializeField] Image PMImg;
    [SerializeField] Image EndImg;

    #endregion


    #region Main

    private void Awake()
    {
        Set_InStartScheduleUI();

        ExplanationBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if(OnExplanation)
                {
                    DOTween.Kill(ExplanationTxt);
                    DOTween.Kill(ExplanationBtn);

                    ExplanationTxt.DOFade(1, 0);
                    ExplanationTxt.DOFade(0, 0.5f)
                        .OnComplete(() => 
                        {
                            ExplanationTxt.gameObject.SetActive(false);
                        });
                    ExplanationBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 100), 0.5f);

                    OnExplanation = false;
                }
                else
                {
                    DOTween.Kill(ExplanationTxt);
                    DOTween.Kill(ExplanationBtn);

                    ExplanationTxt.DOFade(0, 0);
                    ExplanationTxt.gameObject.SetActive(true);
                    ExplanationTxt.DOFade(1, 0.5f);
                    ExplanationBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 25), 0.5f);

                    OnExplanation = true;
                }
            });
    }
    
    #endregion

    #region UI

    public void Set_InStartScheduleUI()
    {
        ScheduleAM.text = "오전 계획 (필요)";
        SchedulePM.text = "오후 계획 (필요)";

        SetProgressingUI(StartImg);
        SetNotProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation("0");
    }

    public void Set_InAMScheduleUI()
    {
        SetStartEachScheduleUI(scheduleManager.currentSelectedScheduleID[0], ScheduleAM);
        SetStartEachScheduleUI(scheduleManager.currentSelectedScheduleID[1], SchedulePM);

        SetComplatePrograssing(StartImg);
        SetProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation(scheduleManager.currentPrograssScheduleID);
    }

    public void Set_InPMScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation(scheduleManager.currentPrograssScheduleID);
    }

    public void Set_InEndScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetComplatePrograssing(PMImg);
        SetProgressingUI(EndImg);

        SetExplanation("1");
    }

    private void SetStartEachScheduleUI(string Id, TMP_Text inputTmp)
    {
        inputTmp.text = (string)DataManager.ScheduleDatas[3][Id];
    }

    private void SetNotProgressingUI(Image img)
    {
        DOTween.Kill(img.GetComponent<CanvasGroup>());
        img.gameObject.GetComponent<CanvasGroup>().alpha = 0.3f;
    }
    private void SetProgressingUI(Image img)
    {
        img.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        img.gameObject.GetComponent<CanvasGroup>().DOFade(0.3f, 1.75f).SetLoops(-1, LoopType.Yoyo);
    }
    private void SetComplatePrograssing(Image img)
    {
        DOTween.Kill(img.GetComponent<CanvasGroup>());
        img.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    private void SetExplanation(string id)
    {
        if (id == "0")
        {
            ExplanationTxt.text = "휴대폰을 키고,\n하루 일과를 계획하세요!";
        }
        else if (id == "1")
        {
            ExplanationTxt.text = "하루를 종료하고,\n새로운 하루를 맞이하세요!";
        }
        else
        {
            ExplanationTxt.text = (string)DataManager.ScheduleDatas[4][id];
        }
    }
    #endregion
}
