//Refactoring v1.0
using System.Collections.Generic;
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

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        foreach(GameObject allReasoningCorkBoard in allReasoningCorkBoards)
        { 
            if (allReasoningCorkBoard.TryGetComponent(out ReasoningController reasoningController) &&
                reasoningController.reasoningID == DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 9][GameManager.Instance.currentChapter].ToString())
            {
                GameObject genReasoningCorkBoard = Instantiate(allReasoningCorkBoard, Vector3.zero, Quaternion.identity, reasoningCorkBoardParentTF);
                currentReasoningCorkBoard = genReasoningCorkBoard;
            }
        }

        currentReasoningCorkBoard.gameObject.SetActive(false);
    }
    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

}
