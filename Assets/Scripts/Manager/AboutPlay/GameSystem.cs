//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main Info")]
    public bool canInput = false;
    public bool canSkipTalking = false;
    public bool canInteractObject = true;
    public e_currentActPart currentActPart = e_currentActPart.StartDay;

    [Header("=== Main UI")]
    [SerializeField] Button pauseBtn;
    [SerializeField] List<TMP_Text> abilityTxts;

    [Header("=== Sprites")]
    [SerializeField] public List<SpriteModule> characterSprites;
    [SerializeField] public List<SpriteModule> objectSprites;
    [SerializeField] public List<SpriteModule> screenSprites;
    Dictionary<string, List<SpriteModule>> typeBySpriteDict = new Dictionary<string, List<SpriteModule>>();

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    [Header("== Epilogue")]
    [SerializeField] CanvasGroup epilogueCG;
    [SerializeField] RectTransform epilogueTxtRT;
    [SerializeField] TMP_Text epilogueTxt;
    [SerializeField] Button skipBtn;
    Sequence endSeq;

    [SerializeField] public MainInfo mainInfo = new MainInfo(0, 0, 0, 0, 0);

    #endregion

    #region Enum

    public enum e_currentActPart
    {
        StartDay, UseActivity, VisitPlace, StreamingTime, EndDay, ReasoningDay, EndChapter
    }

    public void SeteCurrentActPart(e_currentActPart eCurrentActPart)
    {
        currentActPart = eCurrentActPart;

        // 튜토리얼 조건 충족 시 키기
        GuideManager.Instance.PlayTutorial_WhenHad();

        // 가이드 화살표 세팅
        GuideManager.Instance.SetGuideArrow();

        // 각 상태에 오브젝트들 On/Off
        GuideManager.Instance.SetActiveOnOffGOs();
    }

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Player
        playerPos.position = new Vector3(0f, 0f, 0f);
        playerPos.rotation = Quaternion.identity;

        // Text
        SetAbilityUI();
        
        pauseBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameSystem.Instance.canInput) { return; }

                StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.option));
            });

        skipBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameSystem.Instance.canSkipTalking) { return; }
                DOTween.Kill(endSeq);
                SceneManager.LoadScene("Title");
            });



        // Sprites
        string Path = "SpriteModule/";
        foreach (SpriteModule SM in characterSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
        foreach (SpriteModule SM in objectSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
        foreach (SpriteModule SM in screenSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }

        // Dict
        typeBySpriteDict = new Dictionary<string, List<SpriteModule>>
        {
            { "Character", characterSprites },
            { "Object", objectSprites },
            { "Screen", screenSprites }
        };


        Debug.Log("TestVer Epilogue");
        epilogueCG.alpha = 0f;
        epilogueCG.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void OnEnable()
    {
        mainInfo = new MainInfo(
        DataManager.Instance.Get_ChapterStartDay(GameManager.Instance.currentChapter),
        DataManager.Instance.Get_GiveActivity(GameManager.Instance.currentChapter),
        DataManager.Instance.Get_Obs(GameManager.Instance.currentChapter),
        DataManager.Instance.Get_Soc(GameManager.Instance.currentChapter),
        DataManager.Instance.Get_Men(GameManager.Instance.currentChapter));

        Alloffset();
    }

    private void Alloffset()
    {
        Offset();
        
        // Manager
        LanguageManager.Instance.Offset();
        LoadingManager.Instance.Offset();
        PlaceManager.Instance.Offset();
        DialogManager.Instance.Offset();
        ReasoningManager.Instance.Offset();
        PhoneOptionManager.Instance.Offset();
        GuideManager.Instance.Offset();

        // 3D Controller
        ActivityController.Instance.Offset();

        // Phone
        PhoneSoftware.Instance.Offset();
        PhoneHardware.Instance.Offset();

        // Input
        PlayerInputController.Instance.Offset();

        // 2D Controller
        DesktopController.Instance.Offset();
        StreamController.Instance.Offset();

        // Other
        InteractObjectBtnController.Instance.Offset();
        ObjectPooling.Instance.Offset();

    }



    #endregion

    #region SetAbilityUI

    public void SetAbilityUI()
    {
        abilityTxts[0].text = mainInfo.observation.ToString();
        abilityTxts[1].text = mainInfo.sociability.ToString();
        abilityTxts[2].text = mainInfo.mentality.ToString();
    }

    #endregion

    #region Sprite

    private List<Sprite> GetAll_Illust(string type)
    {
        List<Sprite> allSprite = new List<Sprite>();
        foreach(SpriteModule SM in typeBySpriteDict[type])
        {
            foreach(Sprite sprite in SM.sprites)
            {
                allSprite.Add(sprite);
            }
        }
        return allSprite;
    }
    
    public Sprite Get_IllustToID(string type, string IllustID)
    {
        if (type == "" || IllustID == "") { return null; }

        List<Sprite> allIllust = GetAll_Illust(type);
        foreach(Sprite sprite in allIllust)
        {
            if(sprite.name == IllustID)
            {
                return sprite;
            }
        }
        return null;
    }



    #endregion

    #region Animator

    public void PlayAnimationOnce(Animator At, AnimationClip AC)
    {
        RuntimeAnimatorController myController = At.runtimeAnimatorController;
        AnimatorOverrideController myOverrideController = new AnimatorOverrideController();
        myOverrideController.runtimeAnimatorController = myController;
        myOverrideController["NullAnimation"] = AC;
        At.runtimeAnimatorController = myOverrideController;
        At.SetTrigger("Interact");
    }

    public void EndAnimationOnce(Animator At, RuntimeAnimatorController AC)
    {
        At.runtimeAnimatorController = AC;
        At.SetTrigger("Return");
    }

    #endregion

    #region T E S T ver

    public void ShowEpilogue()
    {
        canInput = false;
        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.ResetAnime();
        canInteractObject = false;

        endSeq = DOTween.Sequence();

        epilogueCG.gameObject.SetActive(true);

        endSeq.Append(epilogueCG.DOFade(1f, 3f)
            .OnComplete(() =>
            {
                GameSystem.Instance.canInput = true;
            }));
        endSeq.Append(epilogueTxtRT.DOAnchorPosY(4120, 20f).SetEase(Ease.InOutSine));
        endSeq.Append(epilogueTxt.DOFade(0f, 3f));
        endSeq
            .OnComplete(() =>
            {
                SceneManager.LoadScene("Title");
            });
    }

    #endregion

}

[System.Serializable]
public class SpriteModule
{
    public string nameID;
    public Texture2D texture;
    public Sprite[] sprites;
}


[Serializable]
public class MainInfo
{
    // Flow
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
