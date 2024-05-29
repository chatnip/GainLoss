//Refactoring v1.0
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Header("=== Other")]
    [SerializeField] public string currentChapter = "A";
    [SerializeField] public MainInfo mainInfo = new MainInfo();

    // Other Value
    public bool canInput = true;
    public bool canSkipTalking = false;
    public bool canInteractObject = true;

    #endregion

    #region Framework & Base Set

    

    protected override void Awake()
    {
        base.Awake();

        //temp
        currentChapter = "A";

        Alloffset();
    }

    private void Offset()
    {
        mainInfo = new MainInfo(
            Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 1][currentChapter]),
            Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 0][currentChapter]),
            Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 3][currentChapter]),
            Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 4][currentChapter]),
            Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 5][currentChapter])
            );
    }

    private void Alloffset()
    {
        this.Offset();

        LanguageManager.Instance.Offset();
        GameSystem.Instance.Offset();
        LoadingManager.Instance.Offset();
        PlaceManager.Instance.Offset();

        ActivityController.Instance.Offset();

        Desktop.Instance.Offset();

        PhoneSoftware.Instance.Offset();
        PhoneHardware.Instance.Offset();

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

    // Reasoning
    public List<string> ReasoningContentsID = new List<string>();

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


