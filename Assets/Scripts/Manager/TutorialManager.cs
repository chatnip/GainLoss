using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>, IInteract
{
    #region Value
    [Header("*Property")]
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Data")]
    [SerializeField] public TutorialInfo currentTutorialInfo;

    [Header("*UI")]
    [SerializeField] public CanvasGroup tutorial_ScreenCG;
    [SerializeField] Button closeTutorialBtn;
    [SerializeField] TMP_Text TutorialNameTxt;

    [SerializeField] public GameObject tutotial_MakeSchedule;
    [SerializeField] public GameObject tutotial_PS;
    [SerializeField] public GameObject tutotial_VisitStream;
    [SerializeField] public GameObject tutorial_WatchStream;
    [SerializeField] public GameObject turotial_PartTimeJob;
    [SerializeField] public GameObject turotial_endDay;

    Dictionary<string, GameObject> tutorialsDict;

    #endregion

    #region Main


    protected override void Awake()
    {
        tutorialsDict = new Dictionary<string, GameObject>()
        { 
            { "S00", tutotial_MakeSchedule },
            { "S01", tutotial_PS },
            { "S02", tutotial_VisitStream },
            { "S03", tutorial_WatchStream },
            { "S04", turotial_PartTimeJob },
            { "S99", turotial_endDay },
        };
        //OffAllTutorial();

        closeTutorialBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                OffAllTutorial();
            });

    }

    #endregion

    #region Reset

    private void OffAllTutorial()
    {
        GameManager.Instance.canInput = false;
        DOTween.Complete(tutorialsDict.Values);
        foreach(GameObject tutorial in tutorialsDict.Values) { DOTween.Complete(tutorial); }

        tutorial_ScreenCG.DOFade(0, 1f)
            .OnComplete(() =>
            {
                PlayerInputController.CanMove = true;
                foreach (GameObject tutorial in tutorialsDict.Values) { tutorial.gameObject.SetActive(false); }
                tutorial_ScreenCG.gameObject.SetActive(false);
                closeTutorialBtn.interactable = false;
                GameManager.Instance.canInput = true;
            });
    }
    #endregion

    #region Open

    public bool OpenTutorialWindow(string scheduleID)
    {
        Dictionary<string, bool> Dict = currentTutorialInfo.ID_Bool_Dict();
        if (Dict[scheduleID]) 
        { return false; }
        else 
        { return true; }
    }

    public void OpenTutorialWindow_On(string scheduleID)
    {
        GameManager.Instance.canInput = false;
        GameObject tutorialWindow_type = tutorialsDict[scheduleID];

        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { closeTutorialBtn } }, this);

        //TutorialNameTxt.text = "<size=80%>" + DataManager.ScheduleDatas[3][scheduleID].ToString() + "</size> <#323232>TUTORIAL</color>";
        closeTutorialBtn.interactable = false;
        tutorialWindow_type.SetActive(true);
        tutorial_ScreenCG.gameObject.SetActive(true);
        tutorial_ScreenCG.alpha = 0f;

        tutorial_ScreenCG.DOFade(1f, 2f)
            .OnStart(() =>
            {
                PlayerInputController.StopMove();
            })
            .OnComplete(() =>
            {
                closeTutorialBtn.interactable = true;
                string ID = tutorialsDict.FirstOrDefault(x => x.Value == tutorialWindow_type).Key;
                ShowRecordDataInit(ID);
                GameManager.Instance.canInput = true;
            });
    }

    private void ShowRecordDataInit(string ID)
    {
        if (ID == "S00") { currentTutorialInfo.HasDone_MakeSchedule = true; }
        else if (ID == "S01") { currentTutorialInfo.HasDone_PS = true; }
        else if (ID == "S02") { currentTutorialInfo.HasDone_VisitPlace = true; }
        else if (ID == "S03") { currentTutorialInfo.HasDone_WatchStream = true; }
        else if (ID == "S04") { currentTutorialInfo.HasDone_PartTimeJob = true; }
        else if (ID == "S99") { currentTutorialInfo.HasDone_EndDay = true; }
    }

    public void Interact()
    {
        if(PlayerInputController.SelectBtn == closeTutorialBtn &&
            closeTutorialBtn.interactable)
        {
            OffAllTutorial();
        }
    }


    #endregion
}


[Serializable]
public class TutorialInfo
{
    public bool HasDone_MakeSchedule = false;
    public bool HasDone_PS = false;
    public bool HasDone_VisitPlace = false;
    public bool HasDone_WatchStream = false;
    public bool HasDone_PartTimeJob = false;
    public bool HasDone_EndDay = false;

    public Dictionary<string, bool> ID_Bool_Dict()
    {
        return new Dictionary<string, bool>()
        {
            { "S00", HasDone_MakeSchedule},
            { "S01", HasDone_PS},
            { "S02", HasDone_VisitPlace},
            { "S03", HasDone_WatchStream},
            { "S04", HasDone_PartTimeJob},
            { "S99", HasDone_EndDay},
        };

    }

}
