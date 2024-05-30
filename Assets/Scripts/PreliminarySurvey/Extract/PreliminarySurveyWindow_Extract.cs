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
    [SerializeField] DesktopController Desktop;

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO_Extract SelectedPreliminarySurveySO;

    [Header("*Gage")]
    [SerializeField] Image gageImg;
    [SerializeField] RectTransform gageEffectfulRT;
    [SerializeField] TMP_Text percentTxt;
    [SerializeField] TMP_Text megaBiteTxt;
    [SerializeField] public int currentGage;

    [Header("*Block")]
    [SerializeField] RectTransform blockParent;
    [Tooltip("Start: Lv1, Lv2 ~~ LvMax, Can'tBreakBlock")]
    [SerializeField] public List<Sprite> eachBlockSprite;
    [SerializeField] public List<Sprite> eachBlockEffectfulSprite;
    [SerializeField] public Vector2 blockSizeDelta;

    [Header("*Ball")]
    [SerializeField] BallController BallController;

    [Header("*Board")]
    [SerializeField] Rigidbody2D boardRb;
    [SerializeField] public Vector3 boardMoveDir;
    [SerializeField] float boardMoveSpeed;

    [Header("Other")]
    [SerializeField] TMP_Text CountAndAnnoTxt;

    [Header("*GOs")]
    [SerializeField] List<GameObject> Life_X_GO;

    [Header("*Result")]
    [SerializeField] public GameObject resultWindowParentGO;
    [SerializeField] RectTransform resultWindowRT;
    [SerializeField] GameObject OnlyFail;
    [SerializeField] GameObject OnlyComplete;
    [SerializeField] TMP_Text PrograssBarDataTxt;
    [SerializeField] TMP_Text incomeData;
    [SerializeField] TMP_Text incomeDataAnno;
    [SerializeField] Button endBtn;

    #endregion

    #region Main

    private void Awake()
    {
        endBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
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
        SelectedPreliminarySurveySO = PreliminarySurveyManager.ft_startPS_Extract();
       
        #region Set Gage

        // 게이지 초기화
        currentGage = 0;
        ft_setGage();

        // 게이지 효과 시작
        gageImg.TryGetComponent(out RectTransform RT);
        gageEffectfulRT.anchoredPosition = new Vector2(-RT.rect.width, 0);
        gageEffectfulRT.DOAnchorPos(new Vector2(RT.rect.width, 0), 3f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);


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
        for(int i = 0; i < SelectedPreliminarySurveySO.tileArray.Length; i++)
        {
            for(int j = 0; j < SelectedPreliminarySurveySO.tileArray[i].LineIndex.Length; j++)
            {
                int type = SelectedPreliminarySurveySO.tileArray[i].LineIndex[j];
                Image thisTile = allTile[(i * SelectedPreliminarySurveySO.tileArray[i].LineIndex.Length) + (j)];

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
        if(currentGage >= SelectedPreliminarySurveySO.GoalPoint)
        {
            currentGage = SelectedPreliminarySurveySO.GoalPoint;
        }
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
        GameManager.Instance.canInput = false;

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
            GameManager.Instance.canInput = true;
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
                XEffect(Life_X_GO[i]); ;
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
    // X 표시 효과
    private void XEffect(GameObject XGO)
    {
        Sequence seq = DOTween.Sequence();

        XGO.gameObject.SetActive(true);
        XGO.TryGetComponent(out RectTransform RT);
        seq.Append(RT.DOScale(1.5f, 0.15f).SetEase(Ease.OutBack));
        seq.Append(RT.DOScale(1.0f, 0.35f).SetEase(Ease.OutBounce));
    }
    // Result: 실패
    private void ft_resultFail()
    {
        incomeData.text = "미획득";
        incomeDataAnno.text = "";

        BallController.ft_resetPos();
        PrograssBarDataTxt.text = currentGage + "MB / " + SelectedPreliminarySurveySO.GoalPoint + "MB";
        OnlyFail.gameObject.SetActive(true);
        OnlyComplete.gameObject.SetActive(false);

        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);
        resultWindowParentGO.SetActive(true);

        DOTween.Kill(gageEffectfulRT);
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
        if (SelectedPreliminarySurveySO.cutsceneSO.cutsceneSprites.Count <= 0)
        {
            showResult();
            endBtn.interactable = true;
            return;
        }

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
        
        //incomeData.text = GameSystem.ft_setTextGetData(SelectedPreliminarySurveySO.getID);
        incomeDataAnno.text = SelectedPreliminarySurveySO.getID_Anno;

        PrograssBarDataTxt.text = currentGage + "MB / " + SelectedPreliminarySurveySO.GoalPoint + "MB";

        OnlyFail.gameObject.SetActive(false);
        OnlyComplete.gameObject.SetActive(true);

        resultWindowParentGO.SetActive(true);
        Debug.Log(SelectedPreliminarySurveySO.name);
        PreliminarySurveyManager.PSSO_FindClue_ExceptionIDs.Add(SelectedPreliminarySurveySO.name);

        DOTween.Kill(gageEffectfulRT);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }

    #endregion


}
