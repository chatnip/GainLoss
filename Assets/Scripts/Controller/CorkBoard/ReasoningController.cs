//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningController : Singleton<ReasoningController>
{
    #region Value

    [Header("=== ID")]
    [SerializeField] public string reasoningID;

    [Header("=== Component")]
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

    [Header("=== UI")]
    [SerializeField] Button exitBtn;

    [Header("=== OP")]
    [SerializeField] List<IDBtn> sectionContentIDBtns = new List<IDBtn>();
    

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        this.gameObject.SetActive(false);

        if(this.TryGetComponent(out RectTransform rectTransform))
        { 
            rectTransform.sizeDelta = ReasoningManager.Instance.thisCanvasScaler.referenceResolution;
            rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        }


        // Photo
        foreach (Transform photoTF in photoParentTF)
        {
            if (photoTF.TryGetComponent(out ReasoningPhoto RP))
            { photos.Add(RP); }
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
            { answers.Add(RAW); }
        }

        //Btn
        exitBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }
                ActiveOff(1f);
            });

        this.gameObject.transform.SetAsFirstSibling();
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

        ReasoningChooseContoller.Instance.ActiveOn(time);

        foreach (ReasoningPhoto RP in photos)
        { RP.CheckVisible(time); }

        foreach (ReasoningArrow RA in arrows)
        { RA.CheckVisible(time); }

        foreach (ReasoningAnswer RAW in answers)
        { RAW.CheckVisible(time); }
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

        ReasoningChooseContoller.Instance.ActiveOff(time);
        ActivityController.Instance.QuestionWindow_ActiveOff(0f);

        GameManager.Instance.canInput = true;

        foreach (IDBtn sectionIDBtn in sectionContentIDBtns)
        {
            ObjectPooling.Instance.GetBackIDBtn(sectionIDBtn);
        }
        sectionContentIDBtns = new List<IDBtn>();
    }

    #endregion

    #region Set Photo Visible

    public void SetPhotoVisible(string objectID)
    {
        foreach(ReasoningPhoto RP in photos)
        {
            if (RP.relationObejctID == objectID) { RP.isVisible = true; Debug.Log(RP.gameObject.name); }
        }
    }

    #endregion

    #region Set Button Interact

    public void SetReasoningBtn(List<string> iDs)
    {
        if (iDs.Count == 0 || iDs == null) { return; }

        foreach(IDBtn sectionIDBtn in sectionContentIDBtns)
        {
            ObjectPooling.Instance.GetBackIDBtn(sectionIDBtn);
        }
        sectionContentIDBtns = new List<IDBtn>();

        float Y_UP_fix = ((iDs.Count - 1) * 130f) / 2f;

        for (int i = 0; i < iDs.Count; i++)
        {
            IDBtn idBtn = ObjectPooling.Instance.GetIDBtn();
            idBtn.buttonID = iDs[i];
            idBtn.transform.SetParent(ReasoningChooseContoller.Instance.gameObject.transform);
            idBtn.buttonType = ButtonType.ChoiceType_Reasoning2D;
            idBtn.inputBasicImage = idBtnSprite;
            idBtn.inputAnchorPos = new Vector3(0f, (i * -130f) + Y_UP_fix, 0f);
            idBtn.inputSizeDelta = new Vector2(225f, 100f);
            idBtn.gameObject.SetActive(true);

            sectionContentIDBtns.Add(idBtn);
        }
    }

    #endregion

}
