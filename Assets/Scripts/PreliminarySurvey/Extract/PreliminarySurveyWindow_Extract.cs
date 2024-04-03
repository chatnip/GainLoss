using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyWindow_Extract : MonoBehaviour, IInteract
{
    #region Value
    [Header("*Property")]
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] PlayerInputController PlayerInputController;

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

    #endregion

    #region Main

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
        SelectedPreliminarySurveySO.GetPoint_OnceTime = 4;

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


    #region Life



    #endregion


}
