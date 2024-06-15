//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReasoningController : Singleton<ReasoningController>, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Value

    [Header("=== ID")]
    [SerializeField] public string chapterID;

    [Header("=== Component")]
    [SerializeField] RectTransform thisRT;
    [SerializeField] CanvasGroup thisCG;

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
    [SerializeField] Button decideBtn;

    //Other Value
    float zoomMin = 1f;
    float zoomMax = 2f;
    float currentZoom;
    float roundX = 0f;
    float roundY = 0f;
    Vector2 currentMousePos;

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

        // All setting TMP_text
        foreach (ReasoningModule RM in AllRM())
        { RM.SetThisTmp(); }

        // Btn
        exitBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                ActiveOff(0.5f);
            });
        decideBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                ReasoningManager.Instance.ActiveOn_ConfirmPopup(0.2f);
            });

        // Other Set
        this.gameObject.transform.SetAsFirstSibling();
        currentZoom = 1f;
        thisRT.localScale = Vector3.one * currentZoom;
        thisCG.alpha = 0f;
        this.gameObject.SetActive(false);

        //Debug.Log("Test");
        //foreach(ReasoningModule RM in AllRM())
        //{ RM.isActive = true; }
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

        // 각각의 class 세팅
        List<ReasoningModule> RMs = AllRM();
        foreach (ReasoningModule RM in RMs)
        { RM.SetEachTime(time); }
        foreach (ReasoningAnswer RA in answers)
        { RA.SetDropDownOption(ReasoningManager.Instance.reasoningMaterialIDs); }

        // 추리 결정을 할 수 있는가
        if(GameManager.Instance.currentActPart == GameManager.e_currentActPart.ReasoningDay)
        { decideBtn.interactable = true; }
        else
        { decideBtn.interactable = false; }
    }

    public void ActiveOff(float time)
    {
        GameManager.Instance.canInput = false;

        thisCG.DOFade(0f, time)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;

                PlayerInputController.Instance.CanMove = true;
                this.gameObject.SetActive(false);

                GameManager.Instance.canInteractObject = true;
            });
    }

    #endregion

    #region Set Visible Reasoning

    public void SetVisibleReasoning(string reasoningID)
    {
        foreach(ReasoningModule RM in AllRM())
        {
            if(RM.thisID == reasoningID)
            { RM.isActive = true; }
        }
    }

    #endregion

    #region Zoom

    // 줌 인/아웃
    public void ZoomInOut(float axis)
    {
        if (!this.gameObject.activeSelf) { return; }

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

    // 영역을 빠져나가지 않게
    private void CheckRound()
    {
        if (thisRT.anchoredPosition.x > roundX) { thisRT.anchoredPosition = new Vector2(roundX, thisRT.anchoredPosition.y); }
        else if (thisRT.anchoredPosition.x < -roundX) { thisRT.anchoredPosition = new Vector2(-roundX, thisRT.anchoredPosition.y); }

        if (thisRT.anchoredPosition.y > roundY) { thisRT.anchoredPosition = new Vector2(thisRT.anchoredPosition.x, roundY); }
        else if (thisRT.anchoredPosition.y < -roundY) { thisRT.anchoredPosition = new Vector2(thisRT.anchoredPosition.x, -roundY); }
    }

    #endregion

    #region Drag

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        currentMousePos = (Vector2)Input.mousePosition;
        Debug.Log(currentMousePos);
    }
    
    // 드래그 중
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

    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        currentMousePos = Vector2.zero;
    }

    #endregion

    #region Deside

    public void Deside()
    {
        // 선택된 모든 소재 ID 가져오기
        List<string> choosenAllIDs = new List<string>();
        foreach (ReasoningAnswer RA in answers)
        {
            if (RA.thisSelectedMaterialID != "")
            {
                Debug.Log(RA.thisSelectedMaterialID);
                choosenAllIDs.Add(RA.thisSelectedMaterialID);
            }
        }

        // 선택된 소재들의 각각의 루트와 점수를 통해 딕셔너리 값 가져오기
        Dictionary<string, int> rootAndPointDict = new Dictionary<string, int>();
        foreach(string choosenID in choosenAllIDs)
        {
            string root = DataManager.Instance.Get_RootType(choosenID);
            int rootPoint = DataManager.Instance.Get_RootTypePoint(choosenID);

            if (rootAndPointDict.ContainsKey(root))
            {
                rootAndPointDict[root] += rootPoint;
            }
            else 
            {
                rootAndPointDict.Add(root, rootPoint);
            }
        }

        string getChapter = "";
        int getChapterCount = -1;
        foreach(KeyValuePair<string, int> keyValuePair in rootAndPointDict)
        {
            if(keyValuePair.Value > getChapterCount)
            {
                getChapterCount = keyValuePair.Value;
                getChapter = keyValuePair.Key;
            }
            Debug.Log(keyValuePair.Key + "의 점수: " + keyValuePair.Value);
        }
        Debug.Log("획득 Chapter ID : " + getChapter);

        ReasoningManager.Instance.SetResult(getChapter);
    }

    #endregion

    #region Other

    private List<ReasoningModule> AllRM()
    {
        List<ReasoningModule> allRMs = new List<ReasoningModule>();
        allRMs.AddRange(photos);
        allRMs.AddRange(arrows);
        allRMs.AddRange(answers);
        return allRMs;
    }

    #endregion

}

[System.Serializable]
public class GetChapterID
{
    public List<string> answerReasoningContentIDs;
    public string getChpaterID;
}
