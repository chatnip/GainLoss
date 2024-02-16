using Cinemachine.Utility;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyWindow : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] Desktop Desktop;

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO SelectedPreliminarySurveySO;
    [SerializeField] ClueData[] chooseClueData = new ClueData[2];
    [SerializeField] ClueData[] genClueData = new ClueData[8];
    [SerializeField] ClueData currentClueData;

    [Header("*Btn")]
    [SerializeField] Button[] clueReductionBtns = new Button[8];
    [SerializeField] Button resetBtn;
    [SerializeField] Button tryToCombineBtn;

    [Header("*RT")]
    [SerializeField] RectTransform clueImg;
    [SerializeField] RectTransform targetPointer;

    [Header("*Img")]
    [SerializeField] Image[] selectedImgs = new Image[2];
    [SerializeField] Image combineRotateUI;

    [Header("*Sprite")]
    [SerializeField] Sprite RotateSprite;
    [SerializeField] Sprite CompleteSprite;
    [SerializeField] Sprite FailSprite;

    [Header("*Txt")]
    [SerializeField] TMP_Text AnnouncementTxt;

    [Header("*GOs")]
    [SerializeField] List<GameObject> Life_X_GO;

    [Header("*Result")]
    [SerializeField] public GameObject resultWindowParentGO;
    [SerializeField] RectTransform resultWindowRT;
    [SerializeField] TMP_Text[] clueTxts = new TMP_Text[2];
    [SerializeField] TMP_Text incomeDataTxt;
    [SerializeField] Button endBtn;

    [Header("*other Component")]
    [SerializeField] DOTweenAnimation combineRotateUI_DTA;

    float pointerSpeed = 1.0f;

    #endregion

    #region Main

    private void OnEnable()
    {
        ft_setData(); // Set Clue Reduction Img
        ft_setPadSection(clueReductionBtns.ToList());
        ft_setChooeseClueImg();
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
        SelectedPreliminarySurveySO = null;
        chooseClueData = new ClueData[2];
        genClueData = new ClueData[8];
        currentClueData = null;
        AnnouncementTxt.color = new UnityEngine.Color(0, 0, 0, 0);
        foreach(var X in Life_X_GO)
        { X.gameObject.SetActive(false); }
        combineRotateUI.sprite = RotateSprite;

        SelectedPreliminarySurveySO = PreliminarySurveyManager.ft_startPS(); // Set Selected PSSO

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
    private void ft_setPadSection(List<Button> BtnList)
    {
        PlayerInputController.SetSectionBtns(BtnList , this);
    }
    #endregion

    #region Data

    // 자료를 선택
    public void ft_setChooseClue(Button btn)
    {
        foreach (ClueData clue in genClueData)
        {
            if (clue.partnerBtn == btn)
            {
                if (chooseClueData[0] == null)
                {
                    chooseClueData[0] = clue;
                }
                else if (chooseClueData[1] == null)
                {
                    chooseClueData[1] = clue;
                }
            }
        }
        ft_setChooeseClueImg();
    }
    private void ft_setChooeseClueImg()
    {
        for (int i = 0; i < chooseClueData.Length; i++)
        {
            if (chooseClueData[i] != null)
            {
                selectedImgs[i].sprite = chooseClueData[i].mainSprite;
            }
            else
            {
                selectedImgs[i].sprite = null;
            }
        }
        if (chooseClueData[0] != null &&
            chooseClueData[1] != null)
        { 
            combineRotateUI.gameObject.SetActive(true);
            combineRotateUI_DTA.DOPlay(); 
        }
        else
        {
            combineRotateUI.gameObject.SetActive(false);
            combineRotateUI_DTA.DOPause(); 
        }

    }
    public void ft_clearChooseClue()
    {
        chooseClueData = new ClueData[2];
        ft_setChooeseClueImg();
    }

    // 단서 찾기 시도
    public void Interact()
    {
        if (PlayerInputController.SelectBtn == endBtn)
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
            foreach (IDBtn idBtn in currentClueData.ClueSpotBtns)
            {
                float range = Vector3.Distance(idBtn.transform.position, targetPointer.transform.position);
                if (range < 0.5f) { currentClueData.ft_getID(idBtn); }
            }
        }
    }


    // 결합 시도
    public void ft_tryToCombine()
    {
        if (chooseClueData[0] == null || chooseClueData[1] == null)
        {
            DOTween.Kill(AnnouncementTxt);
            AnnouncementTxt.color = new UnityEngine.Color(0, 0, 0, 1);
            AnnouncementTxt.text = "※ 2가지 모두 선택해야합니다. ※";
            AnnouncementTxt.DOFade(0.0f, 1.0f);
            return;
        }
        
        bool CanCombine = false;
        foreach(string CD_1 in chooseClueData[0].CanCombineIDList)
        {
            foreach(string CD_2 in chooseClueData[1].CanCombineIDList)
            {
                if( CD_1 == CD_2)
                {
                    CanCombine = true;
                    // ID에 따른 성공 결과 출력 및 데이터 적용
                    ft_resultComplete(chooseClueData, CD_1);
                }
            }
        }
        if (!CanCombine)
        {
            ft_minusTryAmount();
        }
    }

    // 성공 시
    private void ft_resultComplete(ClueData[] fromClueDatas, string incomeID)
    {
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
        combineRotateUI_DTA.DOPause();
        combineRotateUI.transform.rotation = Quaternion.identity;
        combineRotateUI.sprite = CompleteSprite;

        for (int i = 0; i < clueTxts.Length; i++)
        { 
            clueTxts[i].text = fromClueDatas[i].clueName;
            foreach(IDBtn CD_idBtn in fromClueDatas[i].ClueSpotBtns)
            {
                if(CD_idBtn.buttonValue.ID == incomeID) { clueTxts[i].text += "\n<size=30>" +  CD_idBtn.buttonValue.Name + "</size>"; }
            }
        }
        incomeDataTxt.text = incomeID;

        PlayerInputController.SetSectionBtns(new List<Button> { endBtn }, this);
        Debug.Log("획득한 소득 데이터로 실제 삽입 구간");

        resultWindowParentGO.SetActive(true);
    }

    // 실패 시 시도 가능 횟수 감소
    private void ft_minusTryAmount()
    {
        for(int i = 0; i < Life_X_GO.Count; i++)
        {
            if (!Life_X_GO[i].gameObject.activeSelf)
            {
                Life_X_GO[i].gameObject.SetActive(true);
                if(i == Life_X_GO.Count - 1)
                {
                    ft_resultFail();
                }
                return;
            }
        }
        
    }
    // 실패 시 ( = 모두 소진 시 )
    private void ft_resultFail()
    {
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
        combineRotateUI_DTA.DOPause(); 
        combineRotateUI.transform.rotation = Quaternion.identity;
        combineRotateUI.sprite = FailSprite;

        for (int i = 0; i < clueTxts.Length; i++)
        { clueTxts[i].text = "증거 미확인"; }
        incomeDataTxt.text = "미획득";

        PlayerInputController.SetSectionBtns(new List<Button> { endBtn }, this);

        resultWindowParentGO.SetActive(true);
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
