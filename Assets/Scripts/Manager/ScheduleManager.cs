using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : Manager<ScheduleManager>
{
    #region Value
    [Header("*Property")]
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PartTimeJobManager PartTimeJobManager;
    [SerializeField] TutorialManager TutorialManager;
    [SerializeField] PhoneHardware PhoneHardware;

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [SerializeField] public List<string> currentSelectedScheduleID = new List<string>();
    [SerializeField] public bool currentPrograssScheduleComplete = false;

    [HideInInspector] public string currentPrograssScheduleID;

    [Header("*Btn")]
    [SerializeField] public Button EndDayBtn;
    [SerializeField] public Button PassNextScheduleBtn;
    [SerializeField] public TMP_Text PassNextScheduleBtnText;
    [SerializeField] public RectTransform TerminatedPart;

    [Header("*Guide")]
    [SerializeField] RectTransform computerArrowRT;
    [SerializeField] public RectTransform inUIEffectRT;

    #endregion

    #region Main

    protected override void Awake()
    {
        EndDayBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ActionEventManager.TurnOnLoading();
            });

        PassNextScheduleBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                PassNextSchedule();
            });
    }

    #endregion

    #region Schedule

    private void ButtonSet()
    {
        // 할 일

        // 시간대
        int prograssing = currentSelectedScheduleID.IndexOf(currentPrograssScheduleID);

        string Schedule = "";
        string Current = "처음";
        if(currentPrograssScheduleID == "S00")
        {
            // 오전 업무 보여주기
            Current = "오전";
            Schedule = DataManager.ScheduleDatas[3][currentSelectedScheduleID[0]].ToString(); }
        else
        {
            // 오후 업무 보여주기
            if (prograssing == 0) 
            { 
                Current = "오후";
                Schedule = DataManager.ScheduleDatas[3][currentSelectedScheduleID[1]].ToString(); 
            }
            // 야간 하루 종료 보여주기
            else if (prograssing == 1) 
            { 
                Current = "야간";
                Schedule = DataManager.ScheduleDatas[3]["S99"].ToString(); 
            }
        }
        PassNextScheduleBtnText.text =
            "<size=140%><b><#161616>[" + Current + "]\r\n<size=120%><#7F0000>[" + Schedule + "]</b><size=100%><#000000>\r\n(으)로 넘어가기";
    
    }

    public void PassBtnOn()
    {
        ButtonSet();
        PassNextScheduleBtn.gameObject.SetActive(true);
        currentPrograssScheduleComplete = true;
    }
    public void PassBtnOff()
    {
        PassNextScheduleBtn.gameObject.SetActive(false);
        currentPrograssScheduleComplete = false;
    }

    public void PassNextSchedule()
    {
        PassBtnOff();

        // 스케쥴 짜기 -> 첫 번째
        if (currentPrograssScheduleID == "S00")
        {
            currentPrograssScheduleID = currentSelectedScheduleID[0];
            SchedulePrograss.Set_InAMScheduleUI();
            PartTimeJobManager.distinctionPartTimeJob();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        }
        // 첫 번째 -> 두 번째로
        else if (currentPrograssScheduleID == currentSelectedScheduleID[0])
        {
            currentPrograssScheduleID = currentSelectedScheduleID[1];
            SchedulePrograss.Set_InPMScheduleUI();
            PartTimeJobManager.distinctionPartTimeJob();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        }
        // 두 번째 -> 하루 종료하기로
        else if (currentPrograssScheduleID == currentSelectedScheduleID[1])
        {
            currentPrograssScheduleID = "S99";
            SchedulePrograss.Set_InEndScheduleUI();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
            EndDayBtn.gameObject.SetActive(true);
        }

        // 튜토리얼 보여주기
        if (TutorialManager.OpenTutorialWindow(currentPrograssScheduleID))
        {
            TutorialManager.OpenTutorialWindow_On(currentPrograssScheduleID);
        }

        SetDotweenGuide();
        
    }

    public void ResetDotweenGuide()
    {
        if(DOTween.IsTweening("ScheduleGuide_Dotween"))
        DOTween.Kill("ScheduleGuide_Dotween");

        computerArrowRT.gameObject.SetActive(false);
        computerArrowRT.anchoredPosition = new Vector2(0, 9);

        inUIEffectRT.gameObject.SetActive(false);
        inUIEffectRT.localScale = Vector2.one;

        inUIEffectRT.TryGetComponent(out Image img);
        img.color = Color.white;

    }
    public void SetDotweenGuide()
    {
        ResetDotweenGuide();

        inUIEffectRT.TryGetComponent(out Image img);

        Sequence seq = DOTween.Sequence();

        

        if (PassNextScheduleBtn.gameObject.activeSelf)
        { seq.Append(InCanvasUI(PassNextScheduleBtn.gameObject, 1f)); }
        else
        {
            // S00: 스케쥴 짜기
            if (currentPrograssScheduleID == "S00") 
            {
                seq.Append(InCanvasUI(PhoneHardware.PhoneListOpenBtn.gameObject, 1f)); 
            }

            // S01: 사전 조사하기
            else if (currentPrograssScheduleID == "S01") 
            {
                computerArrowRT.gameObject.SetActive(true);
                seq.Append(computerArrowRT.DOAnchorPos(computerArrowRT.anchoredPosition + new Vector2(0f, 1f), 0.5f)
                    .SetLoops(2, LoopType.Yoyo));
            }

            // S02: 장소 방문하기
            else if (currentPrograssScheduleID == "S02") 
            {
                if (PlaceManager.currentPlace.ID == "P00")
                { seq.Append(InCanvasUI(PhoneHardware.PhoneListOpenBtn.gameObject, 1f)); }
                else
                { seq.Append(InCanvasUI(TerminatedPart.gameObject, 1f)); }
            }

            // S03: 방송 시청하기
            else if (currentPrograssScheduleID == "S03") 
            {
                computerArrowRT.gameObject.SetActive(true);
                seq.Append(computerArrowRT.DOAnchorPos(computerArrowRT.anchoredPosition + new Vector2(0f, 1f), 0.5f)
                    .SetLoops(2, LoopType.Yoyo));
            }

            // S04: 알바 하기
            else if (currentPrograssScheduleID == "S04") 
            { 
                seq.Append(InCanvasUI(PartTimeJobManager.partTimeJob_StartBtn.gameObject, 1f)); 
            }

            // S99: 하루 종료하기
            else if (currentPrograssScheduleID == "S99") 
            { 
                seq.Append(InCanvasUI(EndDayBtn.gameObject, 1f));
            }
        }

        seq.SetLoops(-1, LoopType.Restart);
        seq.SetId("ScheduleGuide_Dotween");
        return;

        Sequence InCanvasUI(GameObject targetGO, float time)
        {
            Sequence seq2 = DOTween.Sequence();
            targetGO.TryGetComponent(out RectTransform targetRT);
            targetGO.TryGetComponent(out Image targetImg);

            inUIEffectRT.gameObject.SetActive(true);
            inUIEffectRT.anchoredPosition = targetRT.anchoredPosition;
            inUIEffectRT.sizeDelta = targetRT.sizeDelta;
            img.sprite = targetImg.sprite;
            seq2.Append(inUIEffectRT.DOScale(Vector2.one * 1.5f, time));
            seq2.Join(img.DOFade(0f, time));
            seq2.AppendInterval(time);
            seq2.OnStart(() =>
            {
                inUIEffectRT.localScale = Vector2.one;
                img.color = Color.white;
            });


            return seq2;
        }
    }

    #endregion

    #region Reset

    public void ResetDay()
    {
        PassBtnOff();
        currentPrograssScheduleComplete = false;
        currentPrograssScheduleID = "S00";

        // 튜토리얼 보여주기
        if (TutorialManager.OpenTutorialWindow(currentPrograssScheduleID))
        {
            TutorialManager.OpenTutorialWindow_On(currentPrograssScheduleID);
        }

        SchedulePrograss.Set_InStartScheduleUI();
        SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        EndDayBtn.gameObject.SetActive(false);
        SetDotweenGuide();
    }

    #endregion

}
