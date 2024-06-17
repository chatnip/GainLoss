//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningManager : Singleton<ReasoningManager>
{
    #region Value

    [Header("=== Contents")]
    [SerializeField] public List<string> reasoningMaterialIDs = new List<string>();

    [Header("=== Cork Board")]
    [SerializeField] List<GameObject> allReasoningCorkBoards;
    [SerializeField] GameObject currentReasoningCorkBoard;
    [SerializeField] Transform reasoningCorkBoardParentTF;
    [SerializeField] public CanvasScaler thisCanvasScaler;

    [Header("=== Confirm Popup")]
    [SerializeField] CanvasGroup confirmPopup_CG;
    [Header("-- sure?")]
    [SerializeField] public GameObject sureReconfirmGO;
    [SerializeField] Button confirmYesBtn;
    [SerializeField] Button confirmNoBtn;
    [Header("-- result")]
    [SerializeField] public GameObject resultGO;
    [SerializeField] TMP_Text resultTxt;
    [SerializeField] Button okBtn;
    [SerializeField] public string gottenChapterIdx;

    [Header("=== Btn")]
    [SerializeField] Button EndChapterBtn;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        foreach(GameObject allReasoningCorkBoard in allReasoningCorkBoards)
        { 
            if (allReasoningCorkBoard.TryGetComponent(out ReasoningController reasoningController) &&
                reasoningController.chapterID == GameManager.Instance.currentChapter)
            {
                GameObject genReasoningCorkBoard = Instantiate(allReasoningCorkBoard, Vector3.zero, Quaternion.identity, reasoningCorkBoardParentTF);
                currentReasoningCorkBoard = genReasoningCorkBoard;
            }
        }


        // Set Component
        confirmPopup_CG.gameObject.SetActive(false);
        confirmPopup_CG.alpha = 0f;

        sureReconfirmGO.gameObject.SetActive(false);
        resultGO.gameObject.SetActive(false);

        EndChapterBtn.TryGetComponent(out RectTransform btnRT);
        btnRT.anchoredPosition = new Vector2(-300f, 0f);

        // Btn
        confirmYesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningController.Instance.Deside();
            });
        confirmNoBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ActiveOff_ConfirmPopup(0.2f);
            });

        okBtn.OnClickAsObservable()
            .Subscribe(_ => 
            {
                ReasoningController.Instance.ActiveOff(0.2f);
                ActiveOff_ConfirmPopup(0.2f);

                GameManager.Instance.SeteCurrentActPart(GameManager.e_currentActPart.EndChapter);
                GameSystem.Instance.ObjDescOn(null, DataManager.Instance.Get_GetChapterDialog(gottenChapterIdx));
            });

        EndChapterBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                EndChapterBtn.TryGetComponent(out RectTransform btnRT);
                btnRT.DOAnchorPos(new Vector2(-300f, 0f), 1f).SetEase(Ease.OutCubic);

                GameSystem.Instance.ShowEpilogue();

                Debug.Log("Ã©ÅÍ Á¾·á");
            });

        // Ã©ÅÍ¿¡¼­ ±âº»À¸·Î ¾ò´Â ¼ÒÀç È¹µæ
        List<string> getBaseIDs = DataManager.Instance.Get_MaterialIDsByChapter(GameManager.Instance.currentChapter);
        reasoningMaterialIDs.AddRange(getBaseIDs);
        //reasoningMaterialIDs.AddRange(new List<string> { "108", "109", "110", "111", "112", "113", "114" });


    }
    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Panel Confirm Popup

    public void ActiveOn_ConfirmPopup(float time)
    {
        sureReconfirmGO.gameObject.SetActive(true);
        resultGO.gameObject.SetActive(false);

        confirmPopup_CG.gameObject.SetActive(true);
        confirmPopup_CG.DOFade(1f, time);
    }
    public void ActiveOff_ConfirmPopup(float time)
    {
        confirmPopup_CG.DOFade(0f, time)
            .OnComplete(() => { confirmPopup_CG.gameObject.SetActive(false); });
    }

    #endregion

    #region Get Chapter

    public void SetResult(string getChapterID)
    {
        gottenChapterIdx = getChapterID;
        resultTxt.text = DataManager.Instance.Get_ChapterName(gottenChapterIdx);
        sureReconfirmGO.SetActive(false);
        resultGO.SetActive(true);
    }

    public void SetEndChapterBtn()
    {
        EndChapterBtn.TryGetComponent(out RectTransform btnRT);
        btnRT.DOAnchorPos(new Vector2(0f, 0f), 1f).SetEase(Ease.OutCubic);
    }

    #endregion
}
