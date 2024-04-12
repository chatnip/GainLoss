using DG.Tweening;
using System.Collections;
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

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [SerializeField] public List<string> currentSelectedScheduleID = new List<string>();
    [SerializeField] public bool currentPrograssScheduleComplete = false;

    [HideInInspector] public string currentPrograssScheduleID;

    [Header("*Btn")]
    [SerializeField] public Button EndDayBtn;
    [SerializeField] public Button PassNextScheduleBtn;
    [SerializeField] public TMP_Text PassNextScheduleBtnText;

    [Header("*Guide")]
    [SerializeField] public RectTransform phoneOnOpenBtnRT;
    [SerializeField] public RectTransform computerArrowRT;
    [SerializeField] public RectTransform partTimeJobBtnRT;

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
        // �� ��

        // �ð���
        int prograssing = currentSelectedScheduleID.IndexOf(currentPrograssScheduleID);

        string Schedule = "";
        string Current = "";
        if(currentPrograssScheduleID == "S00")
        {
            // ���� ���� �����ֱ�
            Current = "����"; 
            Schedule = DataManager.ScheduleDatas[3][currentSelectedScheduleID[0]].ToString(); }
        else
        {
            // ���� ���� �����ֱ�
            if (prograssing == 0) 
            { 
                Current = "����"; 
                Schedule = DataManager.ScheduleDatas[3][currentSelectedScheduleID[1]].ToString(); 
            }
            // �߰� �Ϸ� ���� �����ֱ�
            else if (prograssing == 1) 
            { 
                Current = "�߰�"; 
                Schedule = DataManager.ScheduleDatas[3]["S99"].ToString(); 
            }
        }
        PassNextScheduleBtnText.text =
            "<size=150%><b><#161616>[" + Current + "]\r\n<size=120%><#7F0000>[" + Schedule + "]</b><size=100%><#000000>\r\n(��)�� �Ѿ��";
    }

    public void PassBtnOn()
    {
        ButtonSet();
        PassNextScheduleBtn.gameObject.SetActive(true);
        currentPrograssScheduleComplete = true;
        ResetDotweenGuide();
    }
    public void PassBtnOff()
    {
        PassNextScheduleBtn.gameObject.SetActive(false);
        currentPrograssScheduleComplete = false;
    }

    public void PassNextSchedule()
    {
        PassBtnOff();

        // ������ ¥�� -> ù ��°
        if (currentPrograssScheduleID == "S00")
        {
            currentPrograssScheduleID = currentSelectedScheduleID[0];
            SchedulePrograss.Set_InAMScheduleUI();
            PartTimeJobManager.distinctionPartTimeJob();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        }
        // ù ��° -> �� ��°��
        else if (currentPrograssScheduleID == currentSelectedScheduleID[0])
        {
            currentPrograssScheduleID = currentSelectedScheduleID[1];
            SchedulePrograss.Set_InPMScheduleUI();
            PartTimeJobManager.distinctionPartTimeJob();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        }
        // �� ��° -> �Ϸ� �����ϱ��
        else if (currentPrograssScheduleID == currentSelectedScheduleID[1])
        {
            currentPrograssScheduleID = "S99";
            SchedulePrograss.Set_InEndScheduleUI();
            SchedulePrograss.SetExplanation(currentPrograssScheduleID);
            EndDayBtn.gameObject.SetActive(true);
        }

        TutorialManager.OpenTutorialWindow(currentPrograssScheduleID);
        SetDotweenGuide();
    }

    public void ResetDotweenGuide()
    {
        DOTween.Kill("ScheduleGuide_Dotween");
        computerArrowRT.gameObject.SetActive(false);
        computerArrowRT.anchoredPosition = new Vector2(0, 9);
    }
    public void SetDotweenGuide()
    {
        ResetDotweenGuide();
        Debug.Log(currentPrograssScheduleID);

        Sequence seq = DOTween.Sequence();

        switch (currentPrograssScheduleID)
        {
            case "S00": // ������ ¥��
                seq.Append(phoneOnOpenBtnRT.DOScale(Vector3.one * 1.1f, 0.25f)
                    .SetLoops(4, LoopType.Yoyo));
                seq.AppendInterval(2.5f);
                break;

            case "S01": // ���� ����
                computerArrowRT.gameObject.SetActive(true);
                seq.Append(computerArrowRT.DOAnchorPos(computerArrowRT.anchoredPosition + new Vector2(0f, 1f), 0.5f)
                    .SetLoops(2, LoopType.Yoyo));
                break;

            case "S02": // ��� �湮
                seq.Append(phoneOnOpenBtnRT.DOScale(Vector3.one * 1.1f, 0.25f)
                    .SetLoops(4, LoopType.Yoyo));
                seq.AppendInterval(2.5f);
                break;

            case "S03": // ��� ��û
                computerArrowRT.gameObject.SetActive(true);
                seq.Append(computerArrowRT.DOAnchorPos(computerArrowRT.anchoredPosition + new Vector2(0f, 1f), 0.5f)
                    .SetLoops(2, LoopType.Yoyo));
                break;

            case "S04": // �Ƹ�����Ʈ
                seq.Append(partTimeJobBtnRT.DOScale(Vector3.one * 1.1f, 0.25f)
                   .SetLoops(4, LoopType.Yoyo));
                seq.AppendInterval(2.5f);
                break;

            case "S99": // �Ϸ� ����
                EndDayBtn.TryGetComponent(out RectTransform EndDayBtnRT);
                seq.Append(EndDayBtnRT.DOScale(Vector3.one * 1.1f, 0.25f)
                   .SetLoops(4, LoopType.Yoyo));
                seq.AppendInterval(2.5f);
                break;

            default: break;
        }

        seq.SetLoops(-1, LoopType.Restart);
        seq.SetId("ScheduleGuide_Dotween");
    }

    #endregion

    #region Reset

    public void ResetDay()
    {
        PassBtnOff();
        currentPrograssScheduleComplete = false;
        currentPrograssScheduleID = "S00";
        TutorialManager.OpenTutorialWindow(currentPrograssScheduleID);
        SchedulePrograss.Set_InStartScheduleUI();
        SchedulePrograss.SetExplanation(currentPrograssScheduleID);
        EndDayBtn.gameObject.SetActive(false);
        SetDotweenGuide();
    }

    #endregion

}
