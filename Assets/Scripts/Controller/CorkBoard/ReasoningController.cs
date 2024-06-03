//Refactoring v1.0
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

    public void ActiveOn()
    {
        this.gameObject.SetActive(true);
        
        foreach(ReasoningPhoto RP in photos)
        { RP.CheckVisible(); }

        foreach (ReasoningArrow RA in arrows)
        { RA.CheckVisible(); }
    }

    public void ActiveOff()
    {
        this.gameObject.SetActive(false);
        ActivityController.Instance.QuestionWindow_ActiveOff(0.25f);

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

}
