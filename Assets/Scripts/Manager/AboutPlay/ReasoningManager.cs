//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningManager : Singleton<ReasoningManager>
{
    #region Value

    [Header("=== Contents")]
    [SerializeField] public List<string> reasoningContentIDs = new List<string>();

    [Header("=== Cork Board")]
    [SerializeField] List<GameObject> allReasoningCorkBoards;
    [SerializeField] GameObject currentReasoningCorkBoard;
    [SerializeField] Transform reasoningCorkBoardParentTF;
    [SerializeField] public CanvasScaler thisCanvasScaler;

    [Header("=== Confirm Popup")]
    [SerializeField] CanvasGroup confirmPopup_CG;
    [SerializeField] Button confirmYesBtn;
    [SerializeField] Button confirmNoBtn;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        foreach(GameObject allReasoningCorkBoard in allReasoningCorkBoards)
        { 
            if (allReasoningCorkBoard.TryGetComponent(out ReasoningController reasoningController) &&
                reasoningController.reasoningID == GameManager.Instance.currentChapter)
            {
                GameObject genReasoningCorkBoard = Instantiate(allReasoningCorkBoard, Vector3.zero, Quaternion.identity, reasoningCorkBoardParentTF);
                currentReasoningCorkBoard = genReasoningCorkBoard;
            }
        }

        currentReasoningCorkBoard.gameObject.SetActive(false);

        // Set Component
        confirmPopup_CG.gameObject.SetActive(false);
        confirmPopup_CG.alpha = 0f;

        // Btn
        confirmYesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningController.Instance.GetChapter();
            });
        confirmNoBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ActiveOff_ConfirmPopup(0.2f);
            });
    }
    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Panel Confirm Popup

    public void ActiveOn_ConfirmPopup(float time)
    {
        confirmPopup_CG.gameObject.SetActive(true);
        confirmPopup_CG.DOFade(1f, time);
    }
    public void ActiveOff_ConfirmPopup(float time)
    {
        confirmPopup_CG.DOFade(0f, time)
            .OnComplete(() => { confirmPopup_CG.gameObject.SetActive(false); });
    }

    #endregion
}
