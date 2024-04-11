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

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [HideInInspector] public List<string> currentSelectedScheduleID = new List<string>();

    [HideInInspector] public string currentPrograssScheduleID;

    [Header("*Btn")]
    [SerializeField] public Button EndDayBtn;

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
    }

    #endregion

    #region Schedule

    public void PassNextSchedule()
    {
        if (currentPrograssScheduleID == "" || currentPrograssScheduleID == null || currentPrograssScheduleID == "S00")
        {
            // ������ ¥��
            currentPrograssScheduleID = "S00";
            if (currentSelectedScheduleID.Count == 0)
            {
                SchedulePrograss.Set_InStartScheduleUI();
                SchedulePrograss.SetExplanation(currentPrograssScheduleID);
            }
        }
        else
        {
            // ù ��° -> �� ��°��
            if (currentPrograssScheduleID == currentSelectedScheduleID[0])
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
        }

        SetDotweenGuide();
    }

    public void SetDotweenGuide()
    {
        DOTween.Kill("ScheduleGuide_Dotween");
        computerArrowRT.gameObject.SetActive(false);
        computerArrowRT.anchoredPosition = new Vector2(0, 9);

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
        currentSelectedScheduleID.Clear();
        currentPrograssScheduleID = "S00";
        EndDayBtn.gameObject.SetActive(false);
        PassNextSchedule();
    }

    #endregion

}
