//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReasoningController : Singleton<ReasoningController>, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Value

    [Header("=== ID")]
    [SerializeField] public string reasoningID;

    [Header("=== Component")]
    [SerializeField] RectTransform thisRT;
    float zoomMin = 1f; float zoomMax = 2f; float currentZoom;
    float roundX = 0f; float roundY = 0f; Vector2 currentMousePos;
    [SerializeField] CanvasGroup thisCG;
    [SerializeField] Sprite idBtnSprite;

    [Header("=== Photo")]
    [SerializeField] Transform photoParentTF;
    [SerializeField] List<ReasoningPhoto> photos = new List<ReasoningPhoto>();

    [Header("=== Arrow")]
    [SerializeField] Transform arrowParentTF;
    [SerializeField] List<ReasoningArrow> arrows = new List<ReasoningArrow>();

    [Header("=== Answer")]
    [SerializeField] Transform answerParentTF;
    [SerializeField] List<ReasoningAnswer> answers = new List<ReasoningAnswer>();
    [SerializeField] public ReasoningAnswer selectedAnswer;

    [Header("=== UI")]
    [SerializeField] Button exitBtn;
    [SerializeField] Button decideBtn;
    [SerializeField] float zoomInOutValue;

    [Header("=== OP")]
    [SerializeField] List<IDBtn> sectionContentIDBtns = new List<IDBtn>();

    [Header("=== Get Chapter ID")]
    [SerializeField] List<GetChapterID> specialGetChapterID;
    [SerializeField] string defaultGetChapterID;

    // Other Value
    List<IDisposable> allIDBtnIDis = new List<IDisposable>();


    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        this.gameObject.SetActive(false);

        if (this.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.sizeDelta = ReasoningManager.Instance.thisCanvasScaler.referenceResolution;
            rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        }


        // Photo
        foreach (Transform photoTF in photoParentTF)
        {
            if (photoTF.TryGetComponent(out ReasoningPhoto RP))
            { RP.Offset(); photos.Add(RP); }
        }

        // Arrow
        foreach (Transform photoTF in arrowParentTF)
        {
            if (photoTF.TryGetComponent(out ReasoningArrow RA))
            { arrows.Add(RA); }
        }

        // Answer
        foreach (Transform answerTF in answerParentTF)
        {
            if (answerTF.TryGetComponent(out ReasoningAnswer RAW))
            { RAW.Offset(); answers.Add(RAW); }
        }

        //Btn
        exitBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }
                ActiveOff(0.5f);
            });
        decideBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningChooseContoller.Instance.ActiveOn_ConfirmPopup(0.2f);
            });

        this.gameObject.transform.SetAsFirstSibling();
        currentZoom = 1f;
        thisRT.localScale = Vector3.one * currentZoom;
        thisCG.alpha = 0f;
    }

    protected override void Awake()
    {
        base.Awake();
        Offset();
    }

    #endregion

    #region Active On/Off

    public void ActiveOn(float time)
    {
        GameManager.Instance.canInput = false;
        PlayerInputController.Instance.CanMove = false;

        this.gameObject.SetActive(true);

        thisCG.DOFade(1f, time)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
            });

        foreach (ReasoningPhoto RP in photos)
        { RP.CheckVisible(time); }

        foreach (ReasoningArrow RA in arrows)
        { RA.CheckVisible(time); }

        foreach (ReasoningAnswer RAW in answers)
        { RAW.CheckVisible(time); }

        if(GameManager.Instance.mainInfo.Day == DataManager.Instance.Get_ChapterEndDay(GameManager.Instance.currentChapter))
        { decideBtn.interactable = true; }
        else
        { decideBtn.interactable = false; }
    }

    public void ActiveOff(float time)
    {
        GameManager.Instance.canInput = false;
        PlayerInputController.Instance.CanMove = true;

        thisCG.DOFade(0f, time)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
                this.gameObject.SetActive(false);
            });

        ActivityController.Instance.QuestionWindow_ActiveOff(time);

        foreach (IDBtn sectionIDBtn in sectionContentIDBtns)
        {
            ObjectPooling.Instance.GetBackIDBtn(sectionIDBtn);
        }
        sectionContentIDBtns = new List<IDBtn>();

        ReasoningChooseContoller.Instance.ActiveOff_ChooseBtn(time);
    }

    #endregion

    #region Set Photo Visible

    public void SetPhotoVisible(string objectID)
    {
        foreach (ReasoningPhoto RP in photos)
        {
            if (RP.relationObejctID == objectID) { RP.isVisible = true; Debug.Log(RP.gameObject.name); }
        }
    }

    #endregion

    #region Set Button Interact

    public void SetReasoningBtn(List<string> iDs)
    {
        if (iDs.Count == 0 || iDs == null) { return; }

        foreach (IDBtn sectionIDBtn in sectionContentIDBtns)
        { ObjectPooling.Instance.GetBackIDBtn(sectionIDBtn); }
        sectionContentIDBtns = new List<IDBtn>();

        foreach (IDisposable idBtnIDis in allIDBtnIDis)
        { idBtnIDis.Dispose(); }
        allIDBtnIDis = new List<IDisposable>();


        float Y_UP_fix = ((iDs.Count - 1) * 130f) / 2f;

        for (int i = 0; i < iDs.Count; i++)
        {
            IDBtn idBtn = ObjectPooling.Instance.GetIDBtn();
            idBtn.buttonID = iDs[i];
            idBtn.transform.SetParent(ReasoningChooseContoller.Instance.chooseBtn_CG.gameObject.transform);
            idBtn.buttonType = ButtonType.ChoiceType_Reasoning2D;
            idBtn.inputBasicImage = idBtnSprite;
            idBtn.inputAnchorPos = new Vector3(0f, (i * -130f) + Y_UP_fix, 0f);
            idBtn.inputSizeDelta = new Vector2(225f, 100f);
            idBtn.gameObject.SetActive(true);
            IDisposable iDis = idBtn.button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    string id = idBtn.buttonID;
                    selectedAnswer.SetContent(id);
                });

            allIDBtnIDis.Add(iDis);
            sectionContentIDBtns.Add(idBtn);
        }
    }

    #endregion

    #region Zoom & Drag

    public void ZoomInOut(float axis)
    {
        if (!this.gameObject.activeSelf) { return; }

        ReasoningChooseContoller.Instance.ActiveOff_ChooseBtn(0.2f);

        // Set Zoom
        currentZoom += (axis * 0.001f);

        if (currentZoom > zoomMax)
        { currentZoom = zoomMax; }
        else if (currentZoom < zoomMin) 
        { currentZoom = zoomMin; }

        thisRT.localScale = Vector3.one * currentZoom;

        // Set Round Value
        roundX = ((1920 * thisRT.localScale.x) - 1920) / 2;
        roundY = ((1080 * thisRT.localScale.y) - 1080) / 2;

        CheckRound();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ReasoningChooseContoller.Instance.ActiveOff_ChooseBtn(0.2f);
        currentMousePos = (Vector2)Input.mousePosition;
        Debug.Log(currentMousePos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(currentMousePos != (Vector2)Input.mousePosition)
        {
            Vector2 dir = (Vector2)Input.mousePosition - currentMousePos;
            thisRT.anchoredPosition += dir;
            currentMousePos = (Vector2)Input.mousePosition;

            CheckRound();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentMousePos = Vector2.zero;
    }

    private void CheckRound()
    {
        if (thisRT.anchoredPosition.x > roundX) { thisRT.anchoredPosition = new Vector2(roundX, thisRT.anchoredPosition.y); }
        else if (thisRT.anchoredPosition.x < -roundX) { thisRT.anchoredPosition = new Vector2(-roundX, thisRT.anchoredPosition.y); }

        if (thisRT.anchoredPosition.y > roundY) { thisRT.anchoredPosition = new Vector2(thisRT.anchoredPosition.x, roundY); }
        else if (thisRT.anchoredPosition.y < -roundY) { thisRT.anchoredPosition = new Vector2(thisRT.anchoredPosition.x, -roundY); }
    }

    #endregion

    #region Deside

    public void GetChapter()
    {
        List<string> chosenAllID = new List<string>();
        for(int i = 0; i < answers.Count; i++)
        { chosenAllID.Add(answers[i].currentSelectedContentID); }

        foreach(GetChapterID GCID in specialGetChapterID)
        {
            List<string> isEqualsIDs = new List<string>();
            for (int i = 0; i < GCID.answerReasoningContentIDs.Count; i++)
            { isEqualsIDs.Add(GCID.answerReasoningContentIDs[i]); }

            if (chosenAllID.Except(isEqualsIDs).Count() == 0 )
            {
                Debug.Log(GCID.getChpaterID);
                return;
            }
        }

        Debug.Log(defaultGetChapterID);
    }

    #endregion

}

[System.Serializable]
public class GetChapterID
{
    public List<string> answerReasoningContentIDs;
    public string getChpaterID;
}
