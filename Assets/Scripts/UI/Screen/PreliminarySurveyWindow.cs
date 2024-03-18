using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyWindow : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] GameSystem GameSystem;
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] Desktop Desktop;

    [Header("*What Value is Changing")]
    [SerializeField] public PreliminarySurveySO SelectedPreliminarySurveySO;
    [SerializeField] ClueData[] genClueData = new ClueData[8];
    [SerializeField] ClueData currentClueData;

    [Header("*Btn")]
    [SerializeField] Button[] clueReductionBtns = new Button[8];
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
    [SerializeField] Button endBtn;


    float pointerSpeed = 1.0f;

    #endregion

    #region Main

    private void OnEnable()
    {
        ft_setData(); // Set Clue Reduction Img
        ft_setPadSection(new List<List<Button>> { 
            new List<Button> { clueReductionBtns[0], clueReductionBtns[1], clueReductionBtns[2], clueReductionBtns[3] },
            new List<Button> { clueReductionBtns[4], clueReductionBtns[5], clueReductionBtns[6], clueReductionBtns[7] }});
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
    private void ft_setPadSection(List<List<Button>> BtnList)
    {
        PlayerInputController.SetSectionBtns(BtnList , this);
    }
    #endregion

    #region Data

    // �ȳ� ����
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

    // �ڷḦ ����
    public void ft_setChooseClue(Button btn)
    {
        foreach(ClueData clueData in genClueData)
        {
            if(clueData.partnerBtn == btn)
            {
                if(ChooseClueNumTxts[ChooseClueNumTxts.Length - 1].text != null && ChooseClueNumTxts[ChooseClueNumTxts.Length - 1].text != "")
                {
                    ft_setAnnouncementText("�� �̻� ���� �ڷḦ �߰��� �� �����ϴ�.");
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
        foreach (TMP_Text Text in  ChooseClueNumTxts) // ���� ���� �ִ��� �Ǻ��ϰ�, ������ �߰� X
        {
            if(Text.text == trySetNum)
            {
                ft_setAnnouncementText("�̹� �߰� �߰��� �ڷ��Դϴ�.");
                return true;
            }
        }
        if (ChooseClueNumTxts[num].text == null || ChooseClueNumTxts[num].text == "") // �� ������ �ִ��� �Ǻ��Ͽ� ���� ���ʿ� ����
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

    // �ܼ� ã�� �õ�
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
            if (range < 0.5f) { currentClueData.ft_showInfo(); }
        }
    }


    // ���� �õ�
    public void ft_tryToCombine()
    {
        string tryNum = "";
        for(int i = 0; i < ChooseClueNumTxts.Length; i++)
        { if (ChooseClueNumTxts[i].text != null && ChooseClueNumTxts[i].text != "") { tryNum += ChooseClueNumTxts[i].text; } }

        if(SelectedPreliminarySurveySO.answerNum == tryNum)
        {
            endBtn.interactable = false;
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { endBtn } }, this);

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
        else if (tryNum.Length < 4)
        { ft_setAnnouncementText("��� �������� �ʾҽ��ϴ�."); }
        else
        { ft_minusTryAmount(); }
    }

    // ���� �� �õ� ���� Ƚ�� ����
    private void ft_minusTryAmount()
    {
        for (int i = 0; i < Life_X_GO.Count; i++)
        {
            if (!Life_X_GO[i].gameObject.activeSelf)
            {
                Life_X_GO[i].gameObject.SetActive(true);
                if (i == Life_X_GO.Count - 1)
                {
                    ft_resultFail();
                }
                return;
            }
        }

    }
    
    // ��� �����ֱ�
    private void showResult()
    {
        endBtn.interactable = true;
        char[] Nums = SelectedPreliminarySurveySO.answerNum.ToCharArray(0, 4);
        for (int i = 0; i < CompleteImgs.Length; i++)
        {
            int index = ((int)Nums[i] - 49);
            CompleteImgs[i].sprite = genClueData[index].mainSprite;
        }
        incomeData.text = ft_setTextGetData(SelectedPreliminarySurveySO.getID);

        OnlyFail.gameObject.SetActive(false);
        OnlyComplete.gameObject.SetActive(true);

        resultWindowParentGO.SetActive(true);
        Debug.Log(SelectedPreliminarySurveySO.name);
        PreliminarySurveyManager.ExceptionPSSO_IDs.Add(SelectedPreliminarySurveySO.name);
        EffectfulWindow.AppearEffectful(resultWindowRT, 0.2f, 0.7f, Ease.OutSine);
    }
    // ��� Text �ϼ� �� ȹ�� ID ����(����)
    private string ft_setTextGetData(string ID)
    {
        string name = "";
        if(ID.Substring(0, 2) == "WA")
        {
            name = ft_setEachTextGetData(DataManager.WordActionDatas[3], WordManager.currentWordActionIDList, ID, ".EXE");
        }
        else if(ID.Substring(0, 1) == "W")
        {
            name = ft_setEachTextGetData(DataManager.WordDatas[5], WordManager.currentWordIDList, ID, ".AIL");
        }
        else if(ID.Substring(0, 1) == "P")
        {
            name = ft_setEachTextGetData(DataManager.PlaceDatas[1], PlaceManager.currentPlaceID_Dict.Keys.ToList(), ID, "(Place)");
        }
        return name;

        string ft_setEachTextGetData(Dictionary<string, object> Data, List<string> haveThings, string id, string type)
        {
            string nameTemp = "";
            if (!haveThings.Contains(id))
            {
                nameTemp = Data[id].ToString() + ".EXE" + "\n�� ȹ�� ��";
                haveThings.Add(id);
            }
            else
            {
                nameTemp = "�� �̹� ������ �ִ� ��� ��";
            }
            return nameTemp;
        }
    }
    
    // ���� �� ( = ���� �õ� ���� Ƚ�� ��� ���� �� )
    private void ft_resultFail()
    {
        incomeData.text = "�ƹ��͵� �˾Ƴ��� ���߽��ϴ�.";

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
