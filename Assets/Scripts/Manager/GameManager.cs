//Refactoring v1.0
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Value

    [Header("=== Other")]
    [SerializeField] public MainInfo MainInfo = new MainInfo();
    [SerializeField] public cutsceneSO TestCutsceneSO;

    // Other
    public bool CanInput = true;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        ActionEventManager.Instance.Offset();
        ActivityController.Instance.Offset();
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

    // Deco
    public string TodayOfTheWeek = "Monday";

}


