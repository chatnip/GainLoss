using UnityEngine;
using System;
using DG.Tweening;

[Serializable]
public class GameManager : MonoBehaviour
{
    [HideInInspector] public MainInfo currentMainInfo = new MainInfo();
    [SerializeField] public cutsceneSO TestCutsceneSO;
    [SerializeField] GameSystem GameSystem;
    public static bool CanInput = true;

    private void Awake()
    {
        if(currentMainInfo.NewGame == true)
        {
            currentMainInfo.NewGame = false;
        }
    }

    public Sequence playMainCutscene(cutsceneSO playCutsceneSO)
    {

        if (cutsceneSO.currentCSSO != null) { return null; }
        else
        {
            cutsceneSO.currentCSSO = playCutsceneSO;
            return cutsceneSO.cutsceneSeq = cutsceneSO.makeCutscene(GameSystem.cutsceneImg, GameSystem.cutsceneTxt);
        }
    }
}

[Serializable]
public class MainInfo
{
    public bool NewGame = true;
    public int day = 1;

    public int maxActivity = 4;
    public int currentActivity = 2;

    #region Need to determine if necessary

    public string TodayOfTheWeek = "Monday";
    public int money = 100;

    public int stressGage = 0;
    public int angerGage = 0;
    public int riskGage = 0;
    public int overloadGage = 0;

    #endregion
}


