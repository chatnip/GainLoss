using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyWindow_Extract : MonoBehaviour, IInteract
{
    #region Value
    [Header("*Property")]
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] Desktop Desktop;

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO_Extract SelectedPreliminarySurveySO;

    [Header("*Gage")]
    [SerializeField] Image gageImg;
    [SerializeField] TMP_Text percentTxt;
    [SerializeField] TMP_Text megaBiteTxt;
    [SerializeField] public int currentGage;

    [Header("*Block, Ball, Board")]
    [SerializeField] RectTransform blockParent;
    [Tooltip("Start: Lv1, Lv2 ~~ LvMax, Can'tBreakBlock")]
    [SerializeField] public List<Sprite> eachBlockSprite;
    [SerializeField] BallController BallController;
    [SerializeField] TMP_Text CountAndAnnoTxt;
    [SerializeField] Rigidbody2D boardRb;
    [SerializeField] public Vector3 boardMoveDir;
    [SerializeField] float boardMoveSpeed;

    [Header("*GOs")]
    [SerializeField] List<GameObject> Life_X_GO;

    [Header("*Result")]
    [SerializeField] public GameObject resultWindowParentGO;
    [SerializeField] RectTransform resultWindowRT;
    [SerializeField] GameObject OnlyFail;
    [SerializeField] GameObject OnlyComplete;
    [SerializeField] TMP_Text incomeData;
    [SerializeField] Button endBtn;

    [Header("Test")]
    [SerializeField] cutsceneSO TempCutsceneSO;

    #endregion

    #region Main

    private void Awake()
    {
        endBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Desktop.EndScheduleThis();
                PlayerInputController.SetSectionBtns(null, null);
                resultWindowParentGO.SetActive(false);
                this.gameObject.SetActive(false);
            });
    }

    private void OnEnable()
    {
        ft_setData();
        StartCoroutine(ft_firstStart());
    }

    private void FixedUpdate()
    {
        ft_moveBoard(boardMoveDir);
    }

    public void Interact()
    {
        if (PlayerInputController.SelectBtn == endBtn && endBtn.interactable)
        {
            Desktop.EndScheduleThis();
            PlayerInputController.SetSectionBtns(null, null);
            resultWindowParentGO.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Set 

    private void ft_setData()
    {
        PlayerInputController.SetSectionBtns(null, this);

        Debug.Log("세팅 -> 바꿔야함 임시임");
        //SelectedPreliminarySurveySO = PreliminarySurveyManager.ft_startPS_Extract();
        #region Temp

        SelectedPreliminarySurveySO = new PreliminarySurveySO_Extract();
        SelectedPreliminarySurveySO.array = new int[8, 12]
        { 
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
            { 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4},
            { 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            { 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            { 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4},
            { 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4},
            { 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4}
        };
        SelectedPreliminarySurveySO.GoalPoint = 230;
        SelectedPreliminarySurveySO.GetPoint_OnceTime = 20;
        SelectedPreliminarySurveySO.getID = "WA03";
        SelectedPreliminarySurveySO.cutsceneSO = TempCutsceneSO;
        #endregion

        #region Set Gage

        currentGage = 0;
        ft_setGage();

        #endregion

        #region Set Tile

        // 모든 타일 List
        List<Image> allTile = new List<Image>();
        foreach (Transform child in blockParent)
        {
            if (child.name == blockParent.name)
                return;
            child.TryGetComponent(out Image eachTileImg);
            allTile.Add(eachTileImg); 
        }

        // 타일별 Sprite 변경
        for(int i = 0; i < SelectedPreliminarySurveySO.array.GetLength(0); i++)
        {
            for(int j = 0; j < SelectedPreliminarySurveySO.array.GetLength(1); j++)
            {
                int type = SelectedPreliminarySurveySO.array[i, j];
                Image thisTile = allTile[(i * SelectedPreliminarySurveySO.array.GetLength(1)) + (j)];

                thisTile.TryGetComponent(out TileController TC);
                TC.tileHP = type;
                TC.ft_setSprite(eachBlockSprite);
            }
        }

        #endregion

        #region Set Life

        foreach(GameObject X in Life_X_GO)
        {
            X.SetActive(false);
        }

        #endregion

    }

    #endregion

    #region Gage

    public void ft_getGage()
    {
        currentGage += SelectedPreliminarySurveySO.GetPoint_OnceTime;
        ft_setGage();
    }
    private void ft_setGage()
    {
        megaBiteTxt.text = currentGage + "MB<size=75%> / " + SelectedPreliminarySurveySO.GoalPoint + "MB</size>";
        float percent = (float)currentGage / (float)SelectedPreliminarySurveySO.GoalPoint;
        string result = string.Format("{0:F1}", percent * 100);
        percentTxt.text = result + "%";
        gageImg.fillAmount = percent;
    }

    #endregion

    #region Start

    private IEnumerator ft_firstStart()
    {
        CountAndAnnoTxt.text = "";
        CountAndAnnoTxt.DOText("암호가 존재합니다!", 1);
        
        yield return new WaitForSeconds(2f);

        ft_readySetGo();
    }

    public void ft_readySetGo()
    {
        BallController.ft_resetPos();

        Sequence seq = DOTween.Sequence();

        seq.Append(CountAndAnnoTxt.DOText("3", 0));
        seq.AppendInterval(1f);
        seq.Append(CountAndAnnoTxt.DOText("2", 0));
        seq.AppendInterval(1f);
        seq.Append(CountAndAnnoTxt.DOText("1", 0));
        seq.AppendInterval(1f);
        
        seq.OnComplete(() =>
        {
            CountAndAnnoTxt.text = "";
            BallController.ft_shotBall();
        });
    }

    #endregion

    #region Move Board

    private void ft_moveBoard(Vector3 Dir)
    {
        if(Dir.x == 0)
        { boardRb.velocity = new Vector2(0, 0); }
        else if(Dir.x > 0)
        { boardRb.velocity = new Vector2(boardMoveSpeed, 0); }
        else
        { boardRb.velocity = new Vector2(-boardMoveSpeed, 0); }
    }

    #endregion

    #region Fail & Clear

    // 체력 감소
    public bool ft_minusTryAmount()
    {
        for (int i = 0; i < Life_X_GO.Count; i++)
        {
            if (!Life_X_GO[i].gameObject.activeSelf)
            {
                Life_X_GO[i].gameObject.SetActive(true);
                if (i == Life_X_GO.Count - 1)
                {
                    ft_resultFail();
                    return false;
                }
                return true;
            }
        }
        return true;

    }

    // Result: 실패
    private void ft_resultFail()
    {
        incomeData.text = "미획득";

        OnlyFail.gameObject.SetActive(true);
        OnlyComplete.gameObject.SetActive(false);

        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);
        resultWindowParentGO.SetActive(true);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }

    // Result: 성공

    public void ft_checkClear()
    {
        if(currentGage >= SelectedPreliminarySurveySO.GoalPoint)
        {
            ft_showCutscene();
            BallController.ft_resetPos();
        }
    }

    private void ft_showCutscene()
    {
        endBtn.interactable = false;
        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);

        if (cutsceneSO.currentCSSO != null) { return; }
        cutsceneSO.currentCSSO = SelectedPreliminarySurveySO.cutsceneSO;
        cutsceneSO.cutsceneSeq = cutsceneSO.makeCutscene(GameSystem.cutsceneImg, GameSystem.cutsceneTxt);
        cutsceneSO.cutsceneSeq.OnComplete(() =>
        {
            showResult();
            endBtn.interactable = true;
            GameSystem.cutsceneImg.color = new Color(0, 0, 0, 0);
            GameSystem.cutsceneImg.gameObject.SetActive(false);
            GameSystem.cutsceneTxt.text = "";
            cutsceneSO.currentCSSO = null;
        });
    }

    private void showResult()
    {
        endBtn.interactable = true;
        
        incomeData.text = GameSystem.ft_setTextGetData(SelectedPreliminarySurveySO.getID);

        OnlyFail.gameObject.SetActive(false);
        OnlyComplete.gameObject.SetActive(true);

        resultWindowParentGO.SetActive(true);
        Debug.Log(SelectedPreliminarySurveySO.name);
        PreliminarySurveyManager.PSSO_FindClue_ExceptionIDs.Add(SelectedPreliminarySurveySO.name);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }

    #endregion


}
