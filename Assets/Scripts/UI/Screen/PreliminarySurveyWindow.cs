using System.Collections.Generic;
using System.Linq;
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

    [Header("*Btn")]
    [SerializeField] Button[] clueReductionBtns = new Button[8];
    [SerializeField] Button resetBtn;
    [SerializeField] Button tryToCombineBtn;

    [Header("*Img")]
    [SerializeField] Image[] selectedImgs = new Image[2];
    [SerializeField] Image clueImg;
    [SerializeField] Image combineUI;
    [SerializeField] Image targetPointer;

    float PointerSpeed = 1.0f;

    #endregion

    #region Main

    private void OnEnable()
    {
        ft_setData(); // Set Clue Reduction Img
        ft_setPadSection(clueReductionBtns.ToList());
    }

    private void LateUpdate()
    {
        ft_pointerMove();
    }

    public void Interact()
    {

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
    }

    public void ft_clearChooseClue()
    {
        chooseClueData = new ClueData[2];
        ft_setChooeseClueImg();
    }

    #endregion

    #region Pointer

    private void ft_pointerMove()
    {
        if(PlayerInputController.pointerMove != Vector2.zero)
        {
            targetPointer.TryGetComponent(out RectTransform RT);
            RT.anchoredPosition += PlayerInputController.pointerMove.normalized * PointerSpeed;
        }
    }

    #endregion

    #endregion

}
