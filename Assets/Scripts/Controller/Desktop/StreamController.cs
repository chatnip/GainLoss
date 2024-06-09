//Refactoring v1.0
using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class StreamController : Singleton<StreamController>
{
    #region Value

    [Header("=== Loading Screen")]
    [SerializeField] CanvasGroup loadingScreenCG;
    [SerializeField] RectTransform rotateRT;

    [Header("=== Input")]
    [SerializeField] InputAction click;

    [Header("=== Chatting")]
    [SerializeField] public Button chattingNextBtn;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();
    [SerializeField] Sprite speechBubbleSprite;
    [SerializeField] public List<float> sb_IDBtns_Y = new List<float>();
    [SerializeField] List<IDBtn> sb_IDBtns = new List<IDBtn>(); 
    [HideInInspector] public bool turnOver = false;

    [Header("=== Result")]
    [SerializeField] CanvasGroup resultWindowCG;
    [SerializeField] Button EndBtn;
    [SerializeField] TMP_Text EndTxt;
    [Header("-- Result 00")]
    [SerializeField] GameObject panelResultDescTxt;
    [SerializeField] TMP_Text resultTxt;
    [SerializeField] Image resultIcon;
    [Header("-- Result 01")]
    [SerializeField] GameObject panelResultGetTxt;
    [SerializeField] TMP_Text getThingTxt;

    [Header("=== Viewer")]
    [SerializeField] TMP_Text viewerAmountTxt;

    [Header("=== Stream Reservation")]
    [SerializeField] public List<string> streamQuarterID = new List<string>();
    [SerializeField] string currentStartStreamID;

    [Header("=== Animation")]
    [SerializeField] SkeletonGraphic skeletonGraphic;

    [Header("=== Choice")]
    [SerializeField] public float chooseLimitTime = 5f;
    [SerializeField] public int goodOrEvilGage = 0;
    [SerializeField] public string currentChooseID;
    string haveChoiceDialogID = "";
    bool isChoiceTime = false;

    // Other Value
    Sequence tween_SubscriberAmountTxt;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Streaming Data
        ScenarioBase
            .Where(Base => Base != null)
            .Subscribe(texting => { StartCoroutine(DialogTexting(texting)); });

        // Set Btn
        EndBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                if (panelResultDescTxt.gameObject.activeSelf)
                {
                    ShowGetReasoningContent();
                }
                else if (panelResultGetTxt.gameObject.activeSelf)
                {
                    ActiveOff();
                }
            });


        // Set Base
        this.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        ActiveOn();
    }

    private void OnDisable()
    { 
        click.Disable(); 
    }

    #endregion

    #region Stream

    // Start
    public void StartStreaming()
    {
        goodOrEvilGage = 0;

        tween_SubscriberAmountTxt = SetUpdateSubscriberAmountText();
        ChooseAndPlay_BaseStreaming();
    }

    // Start Base
    private void ChooseAndPlay_BaseStreaming()
    {
        if (streamQuarterID.Count == 0 || streamQuarterID == null)
        {
            //ShowResult();
            return;
        }
        else
        {
            isChoiceTime = true;

            Debug.Log("랜덤 섞기");
            // Local Data Set
            int rand = UnityEngine.Random.Range(0, streamQuarterID.Count);
            currentStartStreamID = streamQuarterID[rand];
            streamQuarterID.Remove(streamQuarterID[rand]);

            SetScenarioBase(currentStartStreamID);
            return;
        }

    }

    // Set Scenario
    public void SetScenarioBase(string SDialogID)
    {
        // Set Scenario
        List<Fragment> fragments = GetSDialogs(SDialogID);
        ScenarioBase scenario = new(fragments);
        ScenarioBase.Value = scenario;
    }

    public List<Fragment> GetSDialogs(string startDialogID)
    {
        List<Fragment> fragments = new();

        string name = DataManager.Instance.Get_SDialogName(startDialogID);
        string dialog = DataManager.Instance.Get_SDialogText(startDialogID);
        string dialogAnim = DataManager.Instance.Get_SDialogAnim(startDialogID);
        string nextSDialogID = DataManager.Instance.Get_NextSDialogID(startDialogID);
        Debug.Log(name + "/" + dialog + "/" + dialogAnim);
        fragments.Add(new Fragment(name, dialog, dialogAnim));
        if (nextSDialogID == null || nextSDialogID == "")
        { return fragments; }

        int i = 0;
        while (true)
        {
            name = DataManager.Instance.Get_SDialogName(nextSDialogID);
            dialog = DataManager.Instance.Get_SDialogText(nextSDialogID);
            dialogAnim = DataManager.Instance.Get_SDialogAnim(nextSDialogID);

            fragments.Add(new Fragment(name, dialog, dialogAnim));
            Debug.Log(name + "/" + dialog + "/" + dialogAnim);

            if (DataManager.Instance.Get_SDialogHasChoice(nextSDialogID))
            { this.haveChoiceDialogID = nextSDialogID; }

            nextSDialogID = DataManager.Instance.Get_NextSDialogID(nextSDialogID);
            i++;
            if (nextSDialogID == null || nextSDialogID == "" || i > 100)
            { return fragments; }
        }
    }

    #endregion

    #region Result
    /*
        private void ShowResult()
        {
            DOTween.Kill(tween_SubscriberAmountTxt);

            // Get ID
            int typeKindAmount = Convert.ToInt32(DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 1][streamQuarterID]);
            Debug.Log(typeKindAmount);
            for (int i = 0;  i < typeKindAmount; i++)
            {
                List<string> Data = 
                    DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + i + 2][streamQuarterID].ToString().Split('/').ToList();
                int min = Convert.ToInt32(Data[0]); Debug.Log(min);
                int max = Convert.ToInt32(Data[1]); Debug.Log(max);

                if (min <= goodOrEvilGage && goodOrEvilGage <= max)
                {
                    ReasoningManager.Instance.reasoningContentIDs.Add(Data[2]);
                    resultTxt.text = 
                        DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][streamQuarterID]
                        .ToString().Split('/').ToList()[i];
                    resultIcon.sprite =
                        GameSystem.Instance.GetCollectSprites(DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageTypeAmount * 2][streamQuarterID].ToString().Split('/').ToList())[i];

                    Debug.Log("추리 소재 CSV 필요");
                    getThingTxt.text = Data[2];

                    break; 
                }
            }

            Sequence seq = DOTween.Sequence();

            // On GameObject
            resultWindowCG.gameObject.SetActive(true);
            resultWindowCG.alpha = 0f;
            ShowJournal();

            seq.Append(resultWindowCG.DOFade(1f, 0.2f));


            seq
                .OnStart(() => 
                {
                    GameManager.Instance.canInput = false; 
                })
                .OnComplete(() =>
                {
                    GameManager.Instance.canInput = true;
                });
        }
    */
    private void ShowJournal()
    {
        EndTxt.text = DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageNum]["ID"].ToString().Split('/')[0];
        panelResultDescTxt.gameObject.SetActive(true);
        panelResultGetTxt.gameObject.SetActive(false);
    }
    private void ShowGetReasoningContent()
    {
        EndTxt.text = DataManager.Instance.StreamCSVDatas[LanguageManager.Instance.languageNum]["ID"].ToString().Split('/')[1];
        panelResultDescTxt.gameObject.SetActive(false);
        panelResultGetTxt.gameObject.SetActive(true);
    }

    #endregion

    #region Dialog

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        Debug.Log("Play 수정");
        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            Sequence sequence = DOTween.Sequence();



            if (scenarioBase.Fragments[temp].animationID != "")
            { 
                skeletonGraphic.AnimationState.SetEmptyAnimations(0); 
                AnimationSetup((SpineAniState)System.Enum.Parse(typeof(SpineAniState), scenarioBase.Fragments[temp].animationID)); 
            }

            Fragment newFragment = scenarioBase.Fragments[temp];

            // Set Speech Bubble
            IDBtn Choice_IDBtn = ObjectPooling.Instance.GetIDBtn();
            Choice_IDBtn.rect.localScale = Vector3.zero;
            Choice_IDBtn.buttonType = ButtonType.SpeechBubble_Stream2D;
            Choice_IDBtn.transform.SetParent(chattingNextBtn.transform);
            Choice_IDBtn.inputBasicImage = speechBubbleSprite;
            Choice_IDBtn.inputText = newFragment.script;
            Choice_IDBtn.inputIsRight = false;
            if (newFragment.script.Length * 40 < 450f)
            { Choice_IDBtn.inputSizeDelta = new Vector2(newFragment.script.Length * 40, 100f); }
            else
            { Choice_IDBtn.inputSizeDelta = new Vector2(450, 100f); }

            Choice_IDBtn.gameObject.SetActive(true);
            sb_IDBtns.Insert(0, Choice_IDBtn);

            int removeIndex = -1;
            if(sb_IDBtns != null && sb_IDBtns.Count > 0)
            {
                foreach (IDBtn idBtn in sb_IDBtns)
                {
                    int index = sb_IDBtns.IndexOf(idBtn);

                    if(index >= sb_IDBtns_Y.Count - 1)
                    { removeIndex = index; break; }

                    if (removeIndex == -1)
                    { sequence.Join(sb_IDBtns[index].rect.DOAnchorPos3D(new Vector3(0, sb_IDBtns_Y[1 + index], 0), 0.15f)); }
                }
            }
            if(removeIndex != -1)
            {
                ObjectPooling.Instance.GetBackIDBtn(sb_IDBtns[removeIndex]);
                sb_IDBtns.RemoveAt(removeIndex); 
            }

            sequence
                   .SetEase(Ease.Linear)
                   .OnUpdate(() =>
                   {
                       if (click.triggered || turnOver)
                       {
                           sequence.Complete();
                           turnOver = false;
                       }
                   });


            yield return new WaitUntil(() =>
            {
                if (Choice_IDBtn.rect.anchoredPosition3D == new Vector3(0, sb_IDBtns_Y[1], 0))
                {
                    return true;
                }
                return false;
            });

            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() =>
            {
                if (click.triggered || turnOver)
                {
                    turnOver = false;
                    return true;
                }
                return false;
            });
            turnOver = false;
            continue;
        }

        // 행동 있는지 확인
        // 행동 있으면 검은화면 껐다 켜지고 행동지문 출력
        // 행동 없으면 검은화면 껐다 켜지고 결과값 보여주기

        // dialog.SetActive(false);
        // GameSystem.StartGame();
        // 다이얼로그가 끝나면 게이지 결과값 보여주고 다음 날 시작

        turnOver = false;

        if (isChoiceTime)
        {
            isChoiceTime = false;
            GameSystem.Instance.ShowChioceWindow_Stream2D(currentStartStreamID, 0.25f);
        }
        else
        {
            ChooseAndPlay_BaseStreaming();
        }
    }

    #endregion

    #region Set Animation

    private void AnimationSetup(SpineAniState state)
    {
        // 애니메이션 출력
        switch (state)
        {
            case SpineAniState.A00:
                break;

            case SpineAniState.A01: // 인사 후, 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Konnichiwa", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "idle", loop: true);
                break;
            case SpineAniState.A02: // 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "idle", loop: true);
                break;
            case SpineAniState.A03: // 화난 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "angry look", loop: true);
                break;
            case SpineAniState.A04: // 당황한 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "bewildered look", loop: true);
                break;
            case SpineAniState.A05: // 차분한 화남 A
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "calm anger_A", loop: true);
                break;
            case SpineAniState.A06: // 차분한 화남 B
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "calm anger_B", loop: true);
                break;
            case SpineAniState.A07: // 눈 감기
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "closed-eyed", loop: true);
                break;
            case SpineAniState.A08: // 교활(?)
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "crafty", loop: true);
                break;
            case SpineAniState.A09: // 들뜬 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "excited look", loop: true);
                break;
            case SpineAniState.A10: // 자신감 넘치는
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "full of confidence", loop: true);
                break;
            case SpineAniState.A11: // 행복한 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "happy look", loop: true);
                break;
            case SpineAniState.A12: // 홤봑 움음
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "laugh", loop: true);
                break;
            case SpineAniState.A13: // 콧수염 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Mustache idle", loop: true);
                break;
            case SpineAniState.A14: // 콧수염 시작(뿅하면서 나타남)
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Mustache start", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "Mustache idle", loop: true);
                break;

        }


    }

    public enum SpineAniState
    {
        A00, A01, A02, A03, A04, A05, A06, A07, A08, A09, A10,
        A11, A12, A13, A14
    }

    #endregion

    #region Subscriber

    public Sequence SetUpdateSubscriberAmountText()
    {
        Sequence seq = DOTween.Sequence();
        int i = UnityEngine.Random.Range(4000, 5000 + 1);
        viewerAmountTxt.text = i.ToString();

        seq.Append(viewerAmountTxt.DOFade(1f, 1f)
            .OnComplete(() =>
            {
                int j = UnityEngine.Random.Range(-10, 10 + 1);
                int currentAmount = Convert.ToInt32(viewerAmountTxt.text);
                int result = currentAmount + j;

                if (result < 4000) { result = 4000; }
                else if (result > 5000) { result = 5000; }

                viewerAmountTxt.text = result.ToString();
            }));
        seq.AppendInterval(1f);

        seq.SetLoops(-1, LoopType.Restart)
            .SetId("UpdateSubscriberAmountText");

        return seq;
    }


    #endregion

    #region Active On/Off

    public void ActiveOn()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentActPart != GameManager.e_currentActPart.StreamingTime) { return; }

        click.Enable();
        this.gameObject.SetActive(true);
        resultWindowCG.gameObject.SetActive(false);

        // Loading Screen
        loadingScreenCG.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(rotateRT.DORotate(new Vector3(0f, 0f, -360f), 0.2f, RotateMode.FastBeyond360)
            .SetLoops(5, LoopType.Restart)
            .OnComplete(() =>
            {
                Debug.Log("Start Streaming");
                StartStreaming();
            }));
        seq.Append(loadingScreenCG.DOFade(0f, 0.2f)
            .OnComplete(() =>
            {
                loadingScreenCG.gameObject.SetActive(false);
            }));
    }

    private void ActiveOff()
    {
        DesktopController.Instance.TurnOff();
        panelResultDescTxt.gameObject.SetActive(false);
        panelResultGetTxt.gameObject.SetActive(false);

        foreach (IDBtn idBtn in sb_IDBtns) { ObjectPooling.Instance.GetBackIDBtn(idBtn); }
        sb_IDBtns.Clear();

        GameManager.Instance.currentActPart = GameManager.e_currentActPart.EndDay;
    }
    #endregion
}
