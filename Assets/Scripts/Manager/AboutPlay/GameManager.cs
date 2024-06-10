//Refactoring v1.0
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Header("=== Other")]
    [SerializeField] public string currentChapter = "1";
    [SerializeField] public MainInfo mainInfo = new MainInfo(0, 2, 0, 0, 0);

    // Other Value
    public bool canInput = false;
    public bool canSkipTalking = false;
    public bool canInteractObject = true;
    public e_currentActPart currentActPart = e_currentActPart.UseActivity;

    #endregion

    #region Enum

    public enum e_currentActPart
    {
        UseActivity, VisitPlace, StreamingTime, EndDay
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
        //MainOptionManager.Instance.Offset();

        ActivityController.Instance.Offset();

        PhoneSoftware.Instance.Offset();
        PhoneHardware.Instance.Offset();

        PlayerInputController.Instance.Offset();
        DesktopController.Instance.Offset();
        StreamController.Instance.Offset();

        SetInteractionObjects.Instance.Offset();
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
    public int CurrentActivity = 3;
    public int MaxActivity = 4;

    // Ability
    public int ObservationalAbility = 0;
    public int PersuasiveAbility = 0;
    public int MentalStrengthAbility = 0;

    // Place & Streaming
    public int PositiveAndNegative = 0;

    public MainInfo() { }
    public MainInfo(int day, int currentActivity, int d_Obse, int d_Pers, int d_Ment)
    {
        Day = day;
        CurrentActivity = currentActivity;
        ObservationalAbility = d_Obse;
        PersuasiveAbility = d_Pers;
        MentalStrengthAbility = d_Ment;
    }
    public bool IsEnoughAbility(int d_Obse, int d_Pers, int d_Ment)
    {
        if (ObservationalAbility >= d_Obse &&
            PersuasiveAbility >= d_Pers &&
            MentalStrengthAbility >= d_Ment)
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
                ObservationalAbility += incAbilityValue;
                break;

            case ActivityController.e_HomeInteractType.Persuasive:
                PersuasiveAbility += incAbilityValue;
                break;

            case ActivityController.e_HomeInteractType.MentalStrength:
                MentalStrengthAbility += incAbilityValue;
                break;


            default:
                break;
        }
    }

    // Deco
    public string TodayOfTheWeek = "Monday";

}


