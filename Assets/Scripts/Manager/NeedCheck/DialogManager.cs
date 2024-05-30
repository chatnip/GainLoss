using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;
using TMPro;
using Spine.Unity;
using System;
using System.Linq;

public class DialogManager : Singleton<DialogManager>, IInteract
{
    #region Value

    [HideInInspector] public string currentStreamEventID;

    [HideInInspector] public List<Dictionary<string, object>> currentStreamEventDatas = new();

    [Header("*Dialog")]
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] public Button dialogNextBtn;
    [SerializeField] InputAction click;
    [HideInInspector] public bool turnOver = false;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();


    [Header("* Result")]
    [SerializeField] float showTime = 0.8f; //Dotween 시간값
    [SerializeField] GameObject resultWindow;
    [SerializeField] Button EndBtn;

    [Header("=== Viewer")]
    [SerializeField] TMP_Text viewerAmountTxt;

    [Header("=== Stream Reservation")]
    [SerializeField] public string streamReservationID;
    [SerializeField] public List<string> streamQuarterID = new List<string>();
    [SerializeField] List<string> completedStreamQuarterID = new List<string>();
    [SerializeField] string currentStreamModultID;

    [Header("=== Animation")]
    [SerializeField] SkeletonGraphic skeletonGraphic;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        ScenarioBase
            .Where(Base => Base != null)
            .Subscribe(texting => { StartCoroutine(DialogTexting(texting)); });
    }

    private void OnEnable()
    { click.Enable(); }

    private void OnDisable()
    { click.Disable(); }

    #endregion

    #region For Pad

    public void Interact()
    {
        if (PlayerInputController.Instance.SelectBtn == dialogNextBtn)
        { turnOver = true; return; }
        else if (PlayerInputController.Instance.SelectBtn == EndBtn)
        {
            PlayerInputController.Instance.SetSectionBtns(null, null);
            Desktop.Instance.streamWindow.SetActive(false);
            Desktop.Instance.resultWindow.SetActive(false);
        }

    }


    #endregion

    #region Stream

    public void SetstreamReservationID(string PlaceID)
    {
        List<string> ChpaterPlaceIDs = DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 6][GameManager.Instance.currentChapter].ToString().Split('/').ToList();
        List<string> ChapterStreamIDs = DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 8][GameManager.Instance.currentChapter].ToString().Split('/').ToList();
        int index = ChpaterPlaceIDs.IndexOf(PlaceID);
        streamReservationID = ChapterStreamIDs[index];
        Debug.Log(streamReservationID);
    }

    public void StartStreaming()
    {
        for(int i = 0; i < streamQuarterID.Count; i++)
        { completedStreamQuarterID.Add(streamReservationID + streamQuarterID[i]); }

        ChooseAndPlay_BaseStreaming();
    }

    private void ChooseAndPlay_BaseStreaming()
    {
        if (completedStreamQuarterID.Count == 0 || completedStreamQuarterID == null)
        { 
            ShowResult();
            return; 
        }
        else
        {
            // Local Data Set
            int rand = UnityEngine.Random.Range(0, completedStreamQuarterID.Count);
            currentStreamModultID = completedStreamQuarterID[rand];
            completedStreamQuarterID.Remove(completedStreamQuarterID[rand]);

            // Set EventID
            string BaseID = currentStreamModultID.Substring(0, 7) + "B";
            List<string> dialogTexts = 
                DataManager.Instance.StreamModuleCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][BaseID].ToString().Split("/").ToList();
            List<string> AnimeIds =
                DataManager.Instance.StreamModuleCSVDatas[LanguageManager.Instance.languageTypeAmount * 2][BaseID].ToString().Split('/').ToList();
            List<string> leftOrRightstring =
                DataManager.Instance.StreamModuleCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 1][BaseID].ToString().Split('/').ToList();

            // Set Scenario
            List<Fragment> fragments = new();
            for(int i = 0; i < dialogTexts.Count;i++)
            {
                fragments.Add(new Fragment(AnimeIds[i], dialogTexts[i], leftOrRightstring[i]));
            }
            ScenarioBase scenario = new(fragments);
            ScenarioBase.Value = scenario;
        }

    }
    private void ChoiceAndPlay_ChoiceStreaming()
    {

    }

    private void ShowResult()
    {

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

    #region Dialog

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        streamScriptText.text = null;

        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            Sequence sequence = DOTween.Sequence();
            streamScriptText.text = null;

            skeletonGraphic.AnimationState.SetEmptyAnimations(0);
            AnimationSetup((SpineAniState)System.Enum.Parse(typeof(SpineAniState), scenarioBase.Fragments[temp].animationID));

            Fragment newFragment = scenarioBase.Fragments[temp];
            sequence.Append(streamScriptText.DOText(newFragment.Script, newFragment.Script.Length / 10)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        if (click.triggered || turnOver)
                        {
                            sequence.Complete();
                            turnOver = false;
                        }
                    }));

            yield return new WaitUntil(() =>
            {
                if (streamScriptText.text == newFragment.Script)
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

        StartCoroutine(ResultWindowOn());
    }

    #endregion

    #region ResultWindow

    IEnumerator ResultWindowOn()
    {
        DOTween.Kill("UpdateSubscriberAmountText");

        PlayerInputController.Instance.SetSectionBtns(null, null);
        ClearGageAndText();

        CanvasGroup CG = resultWindow.GetComponent<CanvasGroup>();
        CG.alpha = 0.0f;
        CG.DOFade(1, showTime);
        yield return new WaitForSeconds(showTime);

        // 다음날로 가는 버튼 출력
        EndBtn.gameObject.SetActive(true);
        EndBtn.image.DOFade(1, showTime).SetEase(Ease.OutSine);
        PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { new List<Button> { EndBtn } }, this);

        void ClearGageAndText()
        {
            EndBtn.gameObject.SetActive(false);
            EndBtn.image.color = new Color(0, 0, 0, 0);
            resultWindow.SetActive(true); //결과창
        }
/*
        void EffectGage(Slider slider, int appliedGage, float time, Color color)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => slider.value, x => slider.value = x, (appliedGage * 0.01f), time)).SetEase(Ease.OutSine);
            Image img = slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            var sequence2 = DOTween.Sequence();
            img.color = baseColor;
            Color LastColor = Color.Lerp(baseColor, color, appliedGage * 0.01f);
            sequence2.Append(img.DOColor(LastColor, time)).SetEase(Ease.OutSine);
        }
        void IncOpacityText(TMP_Text text, int appliedGage, int incGage, float time)
        {
            string gageAmount = "";
            if (incGage >= 0) { gageAmount = appliedGage + " ( +" + incGage + " )"; }
            else { gageAmount = appliedGage + " ( " + incGage + " )"; }
            
            text.text = gageAmount;
            text.DOFade(1, time).SetEase(Ease.OutSine);
        }
*/
    }

    #endregion

    #region Subscriber

    public void SetUpdateSubscriberAmountText()
    {
        Sequence seq = DOTween.Sequence();
        int i = UnityEngine.Random.Range(4000, 5000 + 1);
        viewerAmountTxt.text = i.ToString();

        seq.Append(viewerAmountTxt.DOFade(1f, 1f)
            .OnComplete(() =>
            {
                int j = UnityEngine.Random.Range(-10, 10 + 1);
                int currentAmount = Convert.ToInt32(viewerAmountTxt.text); 
                Debug.Log(currentAmount);
                int result = currentAmount + j;

                if (result < 4000) { result = 4000; }
                else if (result > 5000) { result = 5000; }

                viewerAmountTxt.text = result.ToString();
            }));
        seq.AppendInterval(1f);

        seq.SetLoops(-1, LoopType.Restart)
            .SetId("UpdateSubscriberAmountText");
    }


    #endregion
}