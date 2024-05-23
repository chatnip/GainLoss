//Refactoring v1.0
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Header("=== Other")]
    [SerializeField] public MainInfo MainInfo = new MainInfo();
    [SerializeField] public cutsceneSO TestCutsceneSO;

    // Other Value
    public bool CanInput = true;
    public bool CanInteractObject = true;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        LoadingManager.Instance.Offset();
        ActivityController.Instance.Offset();

        PhoneSoftware.Instance.Offset();
        PhoneHardware.Instance.Offset();
    }

    #endregion
}

[Serializable]
public class MainInfo
{
    // Flow
    public bool NewGame = true;
    public int day = 1;

    // Activity
    public int currentActivity = 3;
    public int maxActivity = 4;

    // Ability
    public int ObservationalAbility = 0;
    public int PersuasiveAbility = 0;
    public int MentalStrengthAbility = 0;
    public void IncAbility(ActivityController.e_HomeInteractType HI_Type, int incAbilityValue, int DecActivityValue)
    {
        currentActivity -= DecActivityValue;
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


