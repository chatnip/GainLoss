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

    [Header("=== Photo")]
    [SerializeField] Transform photoParentTF;
    [SerializeField] List<ReasoningPhoto> photos = new List<ReasoningPhoto>();

    [Header("=== Arrow")]
    [SerializeField] Transform arrowParentTF;
    [SerializeField] List<ReasoningArrow> arrows = new List<ReasoningArrow>();

    [Header("=== Answer")]
    [SerializeField] Transform answerParentTF;
    [SerializeField] List<ReasoningAnswer> answers = new List<ReasoningAnswer>();
    [SerializeField] Image answerBG_Img;

    [Header("=== UI")]
    [SerializeField] Button exitBtn;
    

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
                ActiveOff();
            });
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
        this.gameObject.SetActive(true);
        ActivityController.Instance.QuestionWindow_ActiveOff(0.25f);

        foreach (ReasoningPhoto RP in photos)
        { RP.CheckVisible(time); }

        foreach (ReasoningArrow RA in arrows)
        { RA.CheckVisible(time); }

        foreach (ReasoningAnswer RA in answers)
        { RA.CheckVisible(time); }
    }

    public void ActiveOff()
    {
        this.gameObject.SetActive(false);

        GameManager.Instance.canInput = true;
        PlayerInputController.Instance.CanMove = true;
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

        answerBG_Img.gameObject.SetActive(true);
        answerBG_Img.DOFade(0.5f, 0.25f);
    }

    #endregion

}
