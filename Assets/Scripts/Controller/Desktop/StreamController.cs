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
    [SerializeField] float sb_IDBtnsFirstY = 25f;
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
    [SerializeField] Image getThingIcon;

    [Header("=== Viewer")]
    [SerializeField] TMP_Text viewerAmountTxt;

    [Header("=== Stream Reservation")]
    [SerializeField] public string startSDialogID;
    [SerializeField] public string endSDialogID;
    [SerializeField] public List<string> playSDialogIDs = new List<string>();
    [SerializeField] string currentStartStreamID;

    [Header("=== Animation")]
    [SerializeField] SkeletonGraphic skeletonGraphic;

    [Header("=== Choice")]
    [SerializeField] public float chooseLimitTime = 5f;
    [SerializeField] public int goodOrEvilGage = 0;
    [SerializeField] public string haveChoiceDialogID = "";

    // Other Value
    IDBtn iDBtn = null;
    Sequence tween_SubscriberAmountTxt;
    Sequence StreamSeq;

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
                if (!GameSystem.Instance.canInput) { return; }

                if (panelResultDescTxt.gameObject.activeSelf)
                {
                    ShowGetReasoningContent();
                }
                else if (panelResultGetTxt.gameObject.activeSelf)
                {
                    ActiveOff();
                    ActivityController.Instance.OnEndDayBtn();
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

        playSDialogIDs = Shuffle(playSDialogIDs);

        playSDialogIDs.Insert(0, startSDialogID);
        playSDialogIDs.Add(endSDialogID);

        ChooseAndPlay_BaseStreaming();
    }

    // Shuffle
    public static List<string> Shuffle(List<string> values)
    {
        System.Random rand = new System.Random();
        var shuffled = values.OrderBy(_ => rand.Next()).ToList();

        return shuffled;
    }

    // Start Base
    private void ChooseAndPlay_BaseStreaming()
    {
        if (playSDialogIDs.Count == 0 || playSDialogIDs == null)
        {
            Debug.Log("방송 종료");

            playSDialogIDs = new List<string>();
            startSDialogID = null;
            endSDialogID = null;

            ShowResult();
            return;
        }
        else
        {
            currentStartStreamID = playSDialogIDs[0];
            playSDialogIDs.Remove(playSDialogIDs[0]);

            SetScenarioBase(currentStartStreamID);
            return;
        }

    }

    // Set Scenario
    public void SetScenarioBase(string SDialogID)
    {
        // Set Scenario
        List<StreamingFragment> fragments = GetSDialogs(SDialogID);
        ScenarioBase scenario = new(fragments);
        ScenarioBase.Value = scenario;
    }

    public void SetScenarioBaseWithChoice(string SDialogID, StreamingFragment sf)
    {
        // Set Scenario
        List<StreamingFragment> fragments = GetSDialogs(SDialogID);
        fragments.Insert(0, sf);
        ScenarioBase scenario = new(fragments);
        ScenarioBase.Value = scenario;
    }

    public List<StreamingFragment> GetSDialogs(string startDialogID)
    {
        List<StreamingFragment> fragments = new();

        string name = DataManager.Instance.Get_SDialogName(startDialogID);
        string dialog = DataManager.Instance.Get_SDialogText(startDialogID);
        string dialogAnim = DataManager.Instance.Get_SDialogAnim(startDialogID);
        fragments.Add(new StreamingFragment(name, dialog, dialogAnim, false));

        if (DataManager.Instance.Get_SDialogHasChoice(startDialogID))
        { this.haveChoiceDialogID = startDialogID; }

        string nextSDialogID = DataManager.Instance.Get_NextSDialogID(startDialogID);

        
        if (nextSDialogID == null || nextSDialogID == "")
        { return fragments; }

        int i = 0;
        while (true)
        {
            name = DataManager.Instance.Get_SDialogName(nextSDialogID);
            dialog = DataManager.Instance.Get_SDialogText(nextSDialogID);
            dialogAnim = DataManager.Instance.Get_SDialogAnim(nextSDialogID);
            fragments.Add(new StreamingFragment(name, dialog, dialogAnim, false));

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

    private void ShowResult()
    {
        DOTween.Kill(tween_SubscriberAmountTxt);

        // Get ID
        Debug.Log(goodOrEvilGage + " -> 지금은 하드코딩 되어있음"); 
        string GetMaterialID = "";
        if (goodOrEvilGage >= 0)
        { GetMaterialID = "114"; }
        else
        { GetMaterialID = "113"; }
        
        if (GetMaterialID != "")
        {
            ReasoningManager.Instance.reasoningMaterialIDs.Add(GetMaterialID);

            resultTxt.text = "후... 오늘도 열심히 방송했다!!"; // 결과 마린 이야기
            resultIcon.sprite = GameSystem.Instance.Get_IllustToID("", ""); // 결과 마린 이미지

            getThingTxt.text = DataManager.Instance.Get_MaterialName(GetMaterialID); // 추리소재 이름
            getThingIcon.sprite = GameSystem.Instance.Get_IllustToID("", ""); // 추리소재 이미지
        }
        else
        {
            resultTxt.text = "None"; // 결과 마린 이야기
            resultIcon.sprite = null; // 결과 마린 이미지

            getThingTxt.text = "None"; // 추리소재 이름
            getThingIcon.sprite = null; // 추리소재 이미
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
                GameSystem.Instance.canInput = false;
            })
            .OnComplete(() =>
            {
                GameSystem.Instance.canInput = true;
            });
    }

    private void ShowJournal()
    {
        EndTxt.text = "Next";
        panelResultDescTxt.gameObject.SetActive(true);
        panelResultGetTxt.gameObject.SetActive(false);
    }
    private void ShowGetReasoningContent()
    {
        EndTxt.text = "Exit";
        panelResultDescTxt.gameObject.SetActive(false);
        panelResultGetTxt.gameObject.SetActive(true);
    }

    #endregion

    #region Dialog

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            StreamSeq = DOTween.Sequence();


            if (scenarioBase.Fragments[temp].animationID != "")
            { 
                skeletonGraphic.AnimationState.SetEmptyAnimations(0); 
                AnimationSetup((SpineAniState)Enum.Parse(typeof(SpineAniState), scenarioBase.Fragments[temp].animationID)); 
            }

            StreamingFragment newFragment = scenarioBase.Fragments[temp];

            // Set Speech Bubble
            IDBtn idBtn = ObjectPooling.Instance.GetIDBtn();
            GenSpeechBubble(idBtn, newFragment.script, newFragment.name, newFragment.isRight);

            StreamSeq
                   .SetEase(Ease.Linear)
                   .OnUpdate(() =>
                   {
                       if (click.triggered || turnOver)
                       {
                           StreamSeq.Complete();
                           turnOver = false;
                       }
                   });


            yield return new WaitUntil(() =>
            {
                if (iDBtn.rect.anchoredPosition3D == new Vector3(0, sb_IDBtnsFirstY, 0))
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

        if(haveChoiceDialogID == "")
        {
            ChooseAndPlay_BaseStreaming();
        }
        else
        {
            DialogManager.Instance.ShowChioceWindow_Stream2D(haveChoiceDialogID, 0.25f);
        }
        
    }

    public void GenSpeechBubble(IDBtn Choice_IDBtn, string script, string name, bool isRight)
    {
        if(StreamSeq != null)
        {
            StreamSeq = DOTween.Sequence();
        }

        iDBtn = Choice_IDBtn;
        iDBtn.rect.localScale = Vector3.zero;
        iDBtn.buttonType = ButtonType.SpeechBubble_Stream2D;
        iDBtn.transform.SetParent(chattingNextBtn.transform);
        iDBtn.inputBasicImage = speechBubbleSprite;
        iDBtn.inputText = script;
        iDBtn.inputExtraText = name;
        iDBtn.inputIsRight = isRight;

        if ((script.Length * 25) + 50 < 500f)
        { iDBtn.inputSizeDelta = new Vector2((script.Length * 25) + 50, 110f); }
        else
        { iDBtn.inputSizeDelta = new Vector2(500f, 130f); }

        iDBtn.gameObject.SetActive(true);
        sb_IDBtns.Insert(0, iDBtn);

        // Remove
        List<IDBtn> removeIDBtns = new List<IDBtn>();
        if (sb_IDBtns != null && sb_IDBtns.Count > 0)
        {
            foreach (IDBtn idBtn in sb_IDBtns)
            {
                int index = sb_IDBtns.IndexOf(idBtn);

                if (index > 6)
                { removeIDBtns.Add(idBtn); break; }
            }
        }
        foreach (IDBtn removeIDBtn in removeIDBtns)
        {
            sb_IDBtns.Remove(removeIDBtn);
            ObjectPooling.Instance.GetBackIDBtn(removeIDBtn);
        }

        // Move
        float plusY = 0;
        if (sb_IDBtns != null && sb_IDBtns.Count > 0)
        {
            for (int i = 0; i < sb_IDBtns.Count; i++)
            {
                StreamSeq.Join(sb_IDBtns[i].rect.DOAnchorPos3D(new Vector3(0, sb_IDBtnsFirstY + (sb_IDBtnsFirstY * i) + plusY, 0), 0.15f));
                plusY += sb_IDBtns[i].rect.sizeDelta.y;
            }
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
        if (GameSystem.Instance == null) { return; }
        else if (GameSystem.Instance.currentActPart != GameSystem.e_currentActPart.StreamingTime) { return; }

        click.Enable();
        this.gameObject.SetActive(true);
        resultWindowCG.gameObject.SetActive(false);
        resultWindowCG.alpha = 0f;
        DialogManager.Instance.streamChioceCG.gameObject.SetActive(false);
        DialogManager.Instance.streamChioceCG.alpha = 0f;

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

        GameSystem.Instance.SeteCurrentActPart(GameSystem.e_currentActPart.EndDay);
    }
    #endregion
}
