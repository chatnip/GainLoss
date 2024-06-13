//Refactoring v1.0
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Header("=== Other")]
    [SerializeField] public string currentChapter = "1";
    [SerializeField] public MainInfo mainInfo = new MainInfo(0, 0, 0, 0, 0);

    // Other Value
    public bool canInput = false;
    public bool canSkipTalking = false;
    public bool canInteractObject = true;
    public e_currentActPart currentActPart = e_currentActPart.UseActivity;

    #endregion

    #region Enum

    public enum e_currentActPart
    {
        UseActivity, VisitPlace, StreamingTime, EndDay, ReasoningDay
    }

    #endregion

    #region Framework & Base Set


    protected override void Awake()
    {
        base.Awake();

    }
    private void OnEnable()
    {
        currentChapter = "1";
        mainInfo = new MainInfo(
            DataManager.Instance.Get_ChapterStartDay(currentChapter),
            DataManager.Instance.Get_GiveActivity(currentChapter),
            DataManager.Instance.Get_Obs(currentChapter), 
            DataManager.Instance.Get_Soc(currentChapter),
            DataManager.Instance.Get_Men(currentChapter));

        Alloffset();
    }

    private void Alloffset()
    {
        LanguageManager.Instance.Offset();
        LoadingManager.Instance.Offset();
        GameSystem.Instance.Offset();
        PlaceManager.Instance.Offset();
        ReasoningManager.Instance.Offset();
        PhoneOptionManager.Instance.Offset();

        ActivityController.Instance.Offset();

        PhoneSoftware.Instance.Offset();
        PhoneHardware.Instance.Offset();

        PlayerInputController.Instance.Offset();
        DesktopController.Instance.Offset();
        StreamController.Instance.Offset();

        InteractObjectBtnController.Instance.Offset();
        ObjectPooling.Instance.Offset();

    }

    #endregion


}

[Serializable]
public class MainInfo
{
    // Flow
    public bool NewGame = true;
    public int Day = 1;

    // Activity
    public int CurrentActivity = 0;
    public int MaxActivity = 0;

    // Ability
    public int observation = 0;
    public int sociability = 0;
    public int mentality = 0;

    // Place & Streaming
    public int PositiveAndNegative = 0;

    public MainInfo() { }
    public MainInfo(int day, int _maxActivity, int d_Obse, int d_Pers, int d_Ment)
    {
        Day = day;
        MaxActivity = _maxActivity;
        observation = d_Obse;
        sociability = d_Pers;
        mentality = d_Ment;
    }
    public bool IsEnoughAbility(int d_Obse, int d_Pers, int d_Ment)
    {
        if (observation >= d_Obse &&
            sociability >= d_Pers &&
            mentality >= d_Ment)
        { return true; }
        else
        { return false; }
    }

    public void IncAbility(ActivityController.e_HomeInteractType HI_Type, int incAbilityValue, int DecActivityValue)
    {
        CurrentActivity -= DecActivityValue;
        switch (HI_Type)
        {
            case ActivityController.e_HomeInteractType.Observational:
                observation += incAbilityValue;
                break;

            case ActivityController.e_HomeInteractType.Persuasive:
                sociability += incAbilityValue;
                break;

            case ActivityController.e_HomeInteractType.MentalStrength:
                mentality += incAbilityValue;
                break;


            default:
                break;
        }
    }


    public static Dictionary<string, List<string>> abilityTypeLanguage = new Dictionary<string, List<string>>
    {
        { "Activity", new List<string> { "Activity", "행동력" } },
        { "observation", new List<string> { "observation", "관찰력" } },
        { "sociability", new List<string> { "sociability", "설득력" } },
        { "mentality", new List<string> { "mentality", "정신력" } }
    };


    // Deco
    public string TodayOfTheWeek = "Monday";

}


