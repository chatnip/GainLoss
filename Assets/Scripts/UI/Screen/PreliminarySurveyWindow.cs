using Cinemachine.Utility;
using DG.Tweening;
using System.Collections.Generic;
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

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO SelectedPreliminarySurveySO;
    [SerializeField] ClueData[] chooseClueData = new ClueData[2];
    [SerializeField] ClueData[] genClueData = new ClueData[8];
    [SerializeField] ClueData currentClueData;

    [Header("*Btn")]
    [SerializeField] Button[] clueReductionBtns = new Button[8];
    [SerializeField] Button resetBtn;
    [SerializeField] Button tryToCombineBtn;

    [Header("*Img")]
    [SerializeField] Image[] selectedImgs = new Image[2];
    [SerializeField] RectTransform clueImg;
    [SerializeField] Image combineRotateUI;
    [SerializeField] RectTransform targetPointer;

    [Header("*Txt")]
    [SerializeField] TMP_Text AnnouncementTxt;

    [Header("*GOs")]
    [SerializeField] List<GameObject> Life_X_GO;


    [Header("*other Component")]
    [SerializeField] DOTweenAnimation combineRotateUI_DTA;

    float PointerSpeed = 1.0f;

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
        ft_pointerMove();
    }
    

    #endregion

    #region Func

    #region Set
    private void ft_setData()
    {
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
        if (currentClueData != null)
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
            AnnouncementTxt.text = "※ 2가지 모두 선택해야합니다. ※";
            AnnouncementTxt.DOFade(0.0f, 0.7f);
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
                    Debug.Log(CD_1 + "의 ID를 통해 결과 실행하기");
                }
            }
        }
        if (!CanCombine)
        {
            ft_minusTryAmount();
        }
    }
    private void ft_minusTryAmount()
    {
        for(int i = 0; i < Life_X_GO.Count; i++)
        {
            if (!Life_X_GO[i].gameObject.activeSelf)
            {
                Life_X_GO[i].gameObject.SetActive(true);
                if(i == Life_X_GO.Count - 1)
                {
                    ft_noTryAmount();
                }
                return;
            }
        }
        
    }
    private void ft_noTryAmount()
    {
        Debug.Log("모두 소진함");
    }

    #endregion

    #region Pointer

    private void ft_pointerMove()
    {
        // Move
        if (PlayerInputController.pointerMove != Vector2.zero) 
        { targetPointer.anchoredPosition += PlayerInputController.pointerMove.normalized * PointerSpeed; }

        //Move Limit
        if (targetPointer.anchoredPosition.x > clueImg.rect.width / 2) { targetPointer.anchoredPosition = new Vector2(clueImg.rect.width / 2, targetPointer.anchoredPosition.y); }
        if (targetPointer.anchoredPosition.x < - clueImg.rect.width / 2) { targetPointer.anchoredPosition = new Vector2(- clueImg.rect.width / 2, targetPointer.anchoredPosition.y); }
        if (targetPointer.anchoredPosition.y > clueImg.rect.height / 2) { targetPointer.anchoredPosition = new Vector2(targetPointer.anchoredPosition.x, clueImg.rect.height / 2); }
        if (targetPointer.anchoredPosition.y < - clueImg.rect.height / 2) { targetPointer.anchoredPosition = new Vector2(targetPointer.anchoredPosition.x, - clueImg.rect.height / 2); }

    }

    #endregion

    #endregion

}
