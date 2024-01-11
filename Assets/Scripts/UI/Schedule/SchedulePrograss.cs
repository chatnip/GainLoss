using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
        ScheduleAM.text = "���� ��ȹ (�ʿ�)";
        SchedulePM.text = "���� ��ȹ (�ʿ�)";

        SetProgressingUI(StartImg);
        SetNotProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation("SetSchedule");
    }

    public void Set_InAMScheduleUI()
    {
        SetStartEachScheduleUI(scheduleManager.currentSelectedSchedule[0], ScheduleAM);
        SetStartEachScheduleUI(scheduleManager.currentSelectedSchedule[1], SchedulePM);

        SetComplatePrograssing(StartImg);
        SetProgressingUI(AMImg);
        SetNotProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation(scheduleManager.currentSelectedSchedule[0]);
    }

    public void Set_InPMScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetProgressingUI(PMImg);
        SetNotProgressingUI(EndImg);

        SetExplanation(scheduleManager.currentSelectedSchedule[1]);
    }

    public void Set_InEndScheduleUI()
    {
        SetComplatePrograssing(StartImg);
        SetComplatePrograssing(AMImg);
        SetComplatePrograssing(PMImg);
        SetProgressingUI(EndImg);

        SetExplanation("EndToday");
    }

    private void SetStartEachScheduleUI(string txt, TMP_Text inputTmp)
    {
        if (txt == "SiteSurvey") { inputTmp.text = "��� �湮�ϱ�"; }
        else if (txt == "WatchingTheStreaming") { inputTmp.text = "��� ��û�ϱ�"; }
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

    private void SetExplanation(string name)
    {
        if (name == "SetSchedule")
        {
            ExplanationTxt.text = "�޴����� Ű��,\n�Ϸ� �ϰ��� ��ȹ�ϼ���!";
        }
        else if (name == "SiteSurvey")
        {
            ExplanationTxt.text = "�޴������� ��Ҹ� ���� �Ŀ�,\n�� ����� ������ �޼��ϼ���!";
        }
        else if (name == "WatchingTheStreaming")
        {
            ExplanationTxt.text = "��ǻ�ͷ�\n����� ��û�ϼ���!";
        }
        else if (name == "EndToday")
        {
            ExplanationTxt.text = "�Ϸ縦 �����ϰ�,\n���ο� �Ϸ縦 �����ϼ���!";
        }
    }
    #endregion
}
