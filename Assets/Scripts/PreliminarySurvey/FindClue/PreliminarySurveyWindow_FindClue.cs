using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyWindow_FindClue : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] GameSystem GameSystem;
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] Desktop Desktop;

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO_FindClue SelectedPreliminarySurveySO;
    [SerializeField] ClueData[] genClueData = new ClueData[8];
    [SerializeField] ClueData currentClueData;

    [Header("*Btn")]
    [SerializeField] RectTransform clueReduction;
    [SerializeField] Button[] clueReductionBtns = new Button[8];
    [SerializeField] Button initNumBtn;
    [SerializeField] Button resetBtn;
    [SerializeField] Button tryToCombineBtn;

    [Header("*RT")]
    [SerializeField] RectTransform clueImg;
    [SerializeField] RectTransform targetPointer;

    [Header("*Sprite")]
    [SerializeField] Sprite RotateSprite;
    [SerializeField] Sprite CompleteSprite;
    [SerializeField] Sprite FailSprite;

    [Header("*Txt")]
    [SerializeField] TMP_Text AnnouncementTxt;
    [SerializeField] TMP_Text[] ChooseClueNumTxts = new TMP_Text[4];

    [Header("*GOs")]
    [SerializeField] List<GameObject> Life_X_GO;

    [Header("*Result")]
    [SerializeField] public GameObject resultWindowParentGO;
    [SerializeField] RectTransform resultWindowRT;
    [SerializeField] GameObject OnlyFail;
    [SerializeField] GameObject OnlyComplete;
    [SerializeField] Image[] CompleteImgs = new Image[4];
    [SerializeField] TMP_Text incomeData;
    [SerializeField] TMP_Text incomeDataAnno;
    [SerializeField] Button endBtn;


    float pointerSpeed = 1.0f;

    #endregion

    #region Main

    private void Awake()
    {
        initNumBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ft_setChooseClue(PlayerInputController.SelectBtn);
            });

        tryToCombineBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ft_tryToCombine();
            });

        resetBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ft_clearChooseClue();
            });

        foreach(Button btn in clueReductionBtns)
        {
            btn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayerInputController.SelectBtn = btn;
                    PlayerInputController.OnOffSelectedBtn(PlayerInputController.SelectBtn);
                    ft_setClueImg(PlayerInputController.SelectBtn);
                });
        }

        /*cutsceneBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (GameSystem.cutsceneImg.gameObject.activeSelf)
                { cutsceneSO.skipOrCompleteSeq(GameSystem.cutsceneImg); return; }
            });*/

        endBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Desktop.EndScheduleThis();
                PlayerInputController.SetSectionBtns(null, null);
                resultWindowParentGO.SetActive(false);
                foreach (Transform child in clueImg)
                { Destroy(child.gameObject); }
                this.gameObject.SetActive(false);
            });

    }

    private void OnEnable()
    {
        ft_setData(); // Set Clue Reduction Img

        ft_setPadSection(new List<List<Button>> {
            new List<Button> { clueReductionBtns[0] },
            new List<Button> { clueReductionBtns[1] },
            new List<Button> { clueReductionBtns[2] },
            new List<Button> { clueReductionBtns[3] },
            new List<Button> { clueReductionBtns[4] },
            new List<Button> { clueReductionBtns[5] },
            new List<Button> { clueReductionBtns[6] },
            new List<Button> { clueReductionBtns[7] }});

        clueReduction.anchoredPosition = new Vector2(clueReduction.anchoredPosition.x, clueReduction.rect.height * -1);
        clueReduction.DOAnchorPos(new Vector2(clueReduction.anchoredPosition.x, 0), 0.5f); // 필름 나오게하는 연출
    }

    private void LateUpdate()
    {
        if (!resultWindowParentGO.activeSelf)
        { ft_pointerMove(); }
    }
    

    #endregion

    #region Func

    #region Set
    private void ft_setData()
    {
        resultWindowParentGO.SetActive(false);
        SelectedPreliminarySurveySO = null;
        genClueData = new ClueData[8];
        currentClueData = genClueData[0];
        AnnouncementTxt.color = new UnityEngine.Color(0, 0, 0, 0);
        foreach(GameObject X in Life_X_GO)
        { X.gameObject.SetActive(false); }
        foreach(TMP_Text num in ChooseClueNumTxts)
        { num.text = ""; }

        SelectedPreliminarySurveySO = PreliminarySurveyManager.ft_startPS_FindClue(); // Set Selected PSSO

        for (int i = 0; i < clueReductionBtns.Length; i++) // Set Reduction Img
        {
            SelectedPreliminarySurveySO.clues[i].TryGetComponent(out ClueData bringCD);
            ClueData genCD = Instantiate(SelectedPreliminarySurveySO.clues[i], clueImg.gameObject.transform).GetComponent<ClueData>();
            genClueData[i] = genCD;
            genCD.gameObject.SetActive(false);   

            if (clueReductionBtns[i].TryGetComponent(out Image img))
            {
                img.sprite = bringCD.mainSprite;
                genCD.partnerBtn = clueReductionBtns[i];
            }
        }
        ft_setClueImg(clueReductionBtns[0]);
    }

    #endregion

    #region Set Img

    public void ft_setClueImg(Button btn)
    {
        foreach(ClueData CD in genClueData)
        {
            if(CD.partnerBtn == btn)
            {
                CD.gameObject.SetActive(true);
                currentClueData = CD;
            }
            else
            {
                CD.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Pad
    private void ft_setPadSection(List<List<Button>> BtnList)
    {
        PlayerInputController.SetSectionBtns(BtnList , this);
    }
    #endregion

    #region Data

    // 안내 문구
    private void ft_setAnnouncementText(string Announcement)
    {
        DOTween.Kill(AnnouncementTxt);
        AnnouncementTxt.text = Announcement;
        AnnouncementTxt.DOFade(1, 0)
            .OnComplete(() =>
            {
                AnnouncementTxt.DOFade(0, 2);
            });
    }

    // 자료를 선택
    public void ft_setChooseClue(Button btn)
    {
        foreach(ClueData clueData in genClueData)
        {
            if(clueData.partnerBtn == btn)
            {
                if(ChooseClueNumTxts[ChooseClueNumTxts.Length - 1].text != null && ChooseClueNumTxts[ChooseClueNumTxts.Length - 1].text != "")
                {
                    ft_setAnnouncementText("더 이상 선택 자료를 추가할 수 없습니다.");
                    return;
                }
                for(int i = 0; i < ChooseClueNumTxts.Length; i++)
                {
                    if (ft_inputChooseNum(i, clueData))
                    { return; }
                }
            }
        }
    }
    private bool ft_inputChooseNum(int num, ClueData chooseCD)
    {
        string trySetNum = (Array.IndexOf(genClueData, chooseCD) + 1).ToString();
        foreach (TMP_Text Text in  ChooseClueNumTxts) // 같은 수가 있는지 판별하고, 있으면 추가 X
        {
            if(Text.text == trySetNum)
            {
                ft_setAnnouncementText("이미 추가 추가된 자료입니다.");
                return true;
            }
        }
        if (ChooseClueNumTxts[num].text == null || ChooseClueNumTxts[num].text == "") // 빈 공간이 있는지 판별하여 가장 왼쪽에 삽입
        {
            ChooseClueNumTxts[num].text = trySetNum;
            return true;
        }
        return false;
    }

    // Reset
    public void ft_clearChooseClue()
    {
        foreach(TMP_Text txt in ChooseClueNumTxts)
        {
            txt.text = "";
        }
        
    }

    // 단서 찾기 시도
    public void Interact()
    {
        if (PlayerInputController.SelectBtn == endBtn && endBtn.interactable)
        {
            Desktop.EndScheduleThis();
            PlayerInputController.SetSectionBtns(null, null);
            resultWindowParentGO.SetActive(false);
            foreach (Transform child in clueImg)
            { Destroy(child.gameObject); }
            this.gameObject.SetActive(false);

        }
        else if (currentClueData != null)
        {
            float range = Vector3.Distance(currentClueData.ClueSpotBtn.transform.position, targetPointer.transform.position);
            if (range < 2.0f) { currentClueData.ft_showInfo(); }
        }
    }


    // 결합 시도
    public void ft_tryToCombine()
    {
        string tryNum = "";
        for(int i = 0; i < ChooseClueNumTxts.Length; i++)
        { if (ChooseClueNumTxts[i].text != null && ChooseClueNumTxts[i].text != "") { tryNum += ChooseClueNumTxts[i].text; } }

        if(SelectedPreliminarySurveySO.answerNum == tryNum)
        {
            if (SelectedPreliminarySurveySO.cutsceneSO.cutsceneSprites.Count <= 0)
            {
                showResult();
                endBtn.interactable = true;
                return;
            }

            endBtn.interactable = false;
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);

            if(cutsceneSO.currentCSSO != null) { return; }
            
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
            return;

        }
        else if (tryNum.Length < 4)
        { ft_setAnnouncementText("모두 선택하지 않았습니다."); }
        else
        { ft_minusTryAmount(); }
    }

    // 실패 시 시도 가능 횟수 감소
    private void ft_minusTryAmount()
    {
        for (int i = 0; i < Life_X_GO.Count; i++)
        {
            if (!Life_X_GO[i].gameObject.activeSelf)
            {
                XEffect(Life_X_GO[i]);
                if (i == Life_X_GO.Count - 1)
                {
                    ft_resultFail();
                }
                return;
            }
        }
    }
    // X 표시 효과
    private void XEffect(GameObject XGO)
    {
        Sequence seq = DOTween.Sequence();

        XGO.gameObject.SetActive (true);
        XGO.TryGetComponent(out RectTransform RT);
        seq.Append(RT.DOScale(1.5f, 0.15f).SetEase(Ease.OutBack));
        seq.Append(RT.DOScale(1.0f, 0.35f).SetEase(Ease.OutBounce));
    }
    
    // 결과 보여주기
    private void showResult()
    {
        endBtn.interactable = true;
        char[] Nums = SelectedPreliminarySurveySO.answerNum.ToCharArray(0, 4);
        for (int i = 0; i < CompleteImgs.Length; i++)
        {
            int index = ((int)Nums[i] - 49);
            CompleteImgs[i].sprite = genClueData[index].mainSprite;
        }
        incomeData.text = GameSystem.ft_setTextGetData(SelectedPreliminarySurveySO.getID);
        incomeDataAnno.text = SelectedPreliminarySurveySO.getID_Anno;

        OnlyFail.gameObject.SetActive(false);
        OnlyComplete.gameObject.SetActive(true);

        resultWindowParentGO.SetActive(true);
        Debug.Log(SelectedPreliminarySurveySO.name);
        PreliminarySurveyManager.PSSO_FindClue_ExceptionIDs.Add(SelectedPreliminarySurveySO.name);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }
    
    // 실패 시 ( = 결합 시도 가능 횟수 모두 소진 시 )
    private void ft_resultFail()
    {
        incomeData.text = "미획득";
        incomeDataAnno.text = "";
        OnlyFail.gameObject.SetActive(true);
        OnlyComplete.gameObject.SetActive(false);

        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);
        resultWindowParentGO.SetActive(true);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }

    #endregion

    #region Pointer

    private void ft_pointerMove()
    {
        // Move
        if (PlayerInputController.pointerMove != Vector2.zero) 
        { targetPointer.anchoredPosition += PlayerInputController.pointerMove.normalized * pointerSpeed; }

        //Move Limit
        if (targetPointer.anchoredPosition.x > clueImg.rect.width / 2) { targetPointer.anchoredPosition = new Vector2(clueImg.rect.width / 2, targetPointer.anchoredPosition.y); }
        if (targetPointer.anchoredPosition.x < - clueImg.rect.width / 2) { targetPointer.anchoredPosition = new Vector2(- clueImg.rect.width / 2, targetPointer.anchoredPosition.y); }
        if (targetPointer.anchoredPosition.y > clueImg.rect.height / 2) { targetPointer.anchoredPosition = new Vector2(targetPointer.anchoredPosition.x, clueImg.rect.height / 2); }
        if (targetPointer.anchoredPosition.y < - clueImg.rect.height / 2) { targetPointer.anchoredPosition = new Vector2(targetPointer.anchoredPosition.x, - clueImg.rect.height / 2); }


    }

    #endregion

    #endregion

}
