//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using System;
using UniRx.Triggers;
using System.Linq;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main UI")]
    [SerializeField] Button pauseBtn;
    [SerializeField] List<TMP_Text> abilityTxts;

    [Header("=== Obj Panel")]
    [SerializeField] public Button objPanelBtn;
    [SerializeField] TMP_Text objName;
    [SerializeField] TMP_Text objText;
    [SerializeField] Image objImg;
    [SerializeField] Animator anotherAnimator;
    [SerializeField] float textingSpeed = 20f;

    [Header("=== Stream UI")]
    [SerializeField] Image timeAttackFillImg;

    [Header("=== Dialog")]
    [SerializeField] List<string> objWriteTexts = new List<string>();
    [SerializeField] List<string> objNameTexts = new List<string>();
    [SerializeField] List<Sprite> objSprites = new List<Sprite>();
    [SerializeField] List<string> objAnimNames = new List<string>();

    InteractObject currentIO;
    Tween objTextingTween;
    int objWriteTexts_currentOrder = 0;

    List<IDisposable> iDisposables = new List<IDisposable>();

    [Header("=== Character Sprite")]
    [SerializeField] public List<SpriteModule> spriteModules;

    [Header("=== Choice UI")]
    [Header("-- 3D Object")]
    [SerializeField] CanvasGroup objectChioceCG;
    [SerializeField] RectTransform objectBtnParentRT;
    [SerializeField] TMP_Text needAbilityTxt;
    [Header("-- 2D Stream")]
    [SerializeField] public CanvasGroup streamChioceCG;
    [SerializeField] RectTransform streamBtnParentRT;

    [SerializeField] List<IDBtn> choiceBtnList = new List<IDBtn>();
    [SerializeField] Sprite btnSprite;

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    // Other Value
    string haveChoiceDialogID;
    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Player
        playerPos.position = new Vector3(0f, 0f, 0f);
        playerPos.rotation = Quaternion.identity;

        // Text
        SetAbilityUI();
        if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
        { CG.alpha = 0f; }
        List<TMP_Text> systemTmpT = new List<TMP_Text> { objName, objText, needAbilityTxt, cutsceneTxt };


        

        // Btns
        objPanelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (!GameManager.Instance.canSkipTalking) { return; }

                ObjDescSkip();
            });
        pauseBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.option));
            });

        // Desc Panel
        objPanelBtn.gameObject.SetActive(false);

        // Choice
        objectChioceCG.gameObject.SetActive(false);
        objectChioceCG.alpha = 0f;

        // Sprites
        string Path = "SpriteModule/";
        foreach (SpriteModule SM in spriteModules)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && cutsceneImg.gameObject.activeSelf)
        {
            cutsceneSO.skipOrCompleteSeq(cutsceneImg, cutsceneTxt);
            return;
        }
    }


    #endregion

    #region Obj Panel

    public void GetDialogs(string startDialogID)
    {
        objWriteTexts = new List<string>();
        objNameTexts = new List<string>();
        objSprites = new List<Sprite>();
        objAnimNames = new List<string>();
        haveChoiceDialogID = null;

        string dialog = DataManager.Instance.Get_DialogText(startDialogID);
        objWriteTexts.Add(dialog);
        string dialogSpeaker = DataManager.Instance.Get_DialogSpeaker(startDialogID);
        objNameTexts.Add(dialogSpeaker);
        string dialogAnim = DataManager.Instance.Get_DialogAnim(startDialogID);
        objAnimNames.Add(dialogAnim);
        string dialogSprite = DataManager.Instance.Get_DialogIllust(startDialogID);
        objSprites.Add(GetCharacterSprite(dialogSprite));

        if (DataManager.Instance.Get_DialogHasChoice(startDialogID))
        { this.haveChoiceDialogID = startDialogID; }

        currentIO.endDialogID = startDialogID;
        string nextDialogID = DataManager.Instance.Get_NextDialogID(startDialogID);
        if (nextDialogID == null || nextDialogID == "")
        { return; }

        int i = 0;
        while (true)
        {
            dialog = DataManager.Instance.Get_DialogText(nextDialogID);
            objWriteTexts.Add(dialog);
            dialogSpeaker = DataManager.Instance.Get_DialogSpeaker(nextDialogID);
            objNameTexts.Add(dialogSpeaker);
            dialogAnim = DataManager.Instance.Get_DialogAnim(nextDialogID);
            objAnimNames.Add(dialogAnim);
            dialogSprite = DataManager.Instance.Get_DialogIllust(startDialogID);
            objSprites.Add(GetCharacterSprite(dialogSprite));

            if (DataManager.Instance.Get_DialogHasChoice(nextDialogID))
            { this.haveChoiceDialogID = nextDialogID; }

            currentIO.endDialogID = nextDialogID;
            nextDialogID = DataManager.Instance.Get_NextDialogID(nextDialogID);
            i++;
            if(nextDialogID == null || nextDialogID == "" || i > 100)
            { break; }
        }
    }
    
    private Tween SetWrite(int i)
    {
        return objText.DOText(objWriteTexts[i], objWriteTexts[i].Length / textingSpeed)
                     .SetEase(Ease.Linear)
                     .OnStart(() =>
                     {
                         Vector2 panelSizeDelta = new Vector2(0, 0);
                         if (objNameTexts[i] != "" && objNameTexts[i] != null)
                         {
                             objName.text = objNameTexts[i];
                             objName.gameObject.SetActive(true);
                             panelSizeDelta.y = 180f;
                         }
                         else
                         {
                             objName.gameObject.SetActive(false);
                             panelSizeDelta.y = 250f;
                         }

                         if (objSprites[i] != null) 
                         { 
                             objImg.sprite = objSprites[i];
                             objImg.gameObject.SetActive(true);
                             panelSizeDelta.x = 1100f;
                         }
                         else
                         {
                             objImg.gameObject.SetActive(false);
                             panelSizeDelta.x = 1400f;
                         }

                         if (objAnimNames[i] != "" && objAnimNames[i] != null)
                         {
                             if (objAnimNames[i].Substring(0, 3) == "KAA")
                             {
                                 Debug.Log("playerAnim");
                                 PlayerController.Instance._animator.Play(objAnimNames[i]);
                             }
                             else
                             {
                                 Debug.Log("anotherAnim");
                                 PlayerController.Instance.resetAnime();
                                 anotherAnimator.Play(objAnimNames[i]);
                             }
                         }
                         else
                         {
                             PlayerController.Instance.resetAnime();
                         }


                         if (objText.TryGetComponent(out RectTransform rt)) { rt.sizeDelta = panelSizeDelta; }
                     })
                     .OnComplete(() =>
                     {
                         objWriteTexts_currentOrder++;
                     });
    }

    public void ObjDescOn(InteractObject IO, string startDialogID)
    {
        // Set Base Data
        if(IO != null && IO.TryGetComponent(out Animator animator)) 
        { anotherAnimator = animator; }

        currentIO = IO;
        GameManager.Instance.canInput = false;
        GameManager.Instance.canInteractObject = false;
        GameManager.Instance.canSkipTalking = true;
        PlayerInputController.Instance.MoveStop(); 
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);

        // Set Text Data
        objText.text = "";
        objName.text = "";
        objWriteTexts_currentOrder = 0;
        GetDialogs(startDialogID);

        // Set Writting
        objPanelBtn.gameObject.SetActive(true);
        objTextingTween = SetWrite(objWriteTexts_currentOrder);
    }

    public void ObjDescSkip()
    {
        // 글인 라이팅 중이면, 바로 완료
        if (DOTween.IsTweening(objText))
        {
            Debug.Log("라이팅 중");
            DOTween.Complete(objText);
        }
        else
        {
            // 마지막 라이팅까지 완료가 안되어있다면, 새로운 라이팅 시작
            if (objWriteTexts.Count > objWriteTexts_currentOrder)
            {
                Debug.Log("넘기기");
                objText.text = "";
                SetWrite(objWriteTexts_currentOrder);
            }
            // 마지막 라이팅까지 완료가 되어있다면
            else
            {
                // 선택지를 주는 라이팅 이었다면
                if (haveChoiceDialogID != null)
                {
                    Debug.Log("선택지");
                    ShowChioceWindow_Object3D(0.25f);
                }
                else // 아니라면(기본 라이팅)
                {
                    Debug.Log("종료");
                    ObjDescOff();
                }
            }
        }
    }

    public void ObjDescOff()
    {
        if (currentIO != null)
        {
            if (PlaceManager.Instance.needInteractIOs.Contains(currentIO))
            { PlaceManager.Instance.needInteractIOs.Remove(currentIO); } 
            currentIO.IsInteracted = true;
            currentIO = null; 
        }

        GameManager.Instance.canInput = true;
        GameManager.Instance.canInteractObject = true;
        PlayerInputController.Instance.CanMove = true;
        PlayerController.Instance._animator.SetTrigger("Return");
        PlayerController.Instance.resetAnime();
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);

        DOTween.Kill(objTextingTween);
        objPanelBtn.gameObject.SetActive(false);
        objWriteTexts_currentOrder = 0;

        if(GameManager.Instance.currentActPart == GameManager.e_currentActPart.VisitPlace)
        { PlaceManager.Instance.CheckCanGoHome(); }
    }

    #endregion

    #region Choice

    public void ShowChioceWindow_Object3D(float time)
    {
        // time Pause
        Time.timeScale = 0f;

        // Set On
        objectChioceCG.gameObject.SetActive(true);
        objectChioceCG.DOFade(1f, time)
            .OnStart(() => { GameManager.Instance.canSkipTalking = false; })
            .OnComplete(() => { GameManager.Instance.canSkipTalking = true; })
            .SetUpdate(true);

        // Set Btn By ID
        List<string> choiceIDs = DataManager.Instance.Get_ChoiceIDs(haveChoiceDialogID);

        // Set UI
        for (int i = 0; i < choiceIDs.Count; i++)
        {
            IDBtn Choice_IDBtn = ObjectPooling.Instance.GetIDBtn();
            Choice_IDBtn.buttonType = ButtonType.ChoiceType_Object3D;
            Choice_IDBtn.transform.SetParent(objectBtnParentRT);
            Choice_IDBtn.buttonID = choiceIDs[i];
            Choice_IDBtn.inputBasicImage = btnSprite;
            Choice_IDBtn.inputSizeDelta = new Vector2(1000f, 100f);

            Choice_IDBtn.inputAnchorPos = new Vector3(0f, i * -120f, 0f);
            if (choiceIDs.Count > 1) { Choice_IDBtn.inputAnchorPos += Vector3.up * 60f * (choiceIDs.Count - 1); }

            Choice_IDBtn.gameObject.SetActive(true);
            choiceBtnList.Add(Choice_IDBtn);
        }

        // Set Btn Subscribe
        needAbilityTxt.text = null;
        if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
        {
            DOTween.Kill(CG);
            CG.alpha = 0f;
            needAbilityTxt.transform.parent.gameObject.SetActive(false);
        }

        foreach (IDBtn idBtn in choiceBtnList)
        {
            // Pointer Enter
            IDisposable iDisEnter = idBtn.button.OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    string _id = idBtn.buttonID;
                    if (DataManager.Instance.Get_ChoiceNeedAbility(_id) == "") 
                    { return; }

                    string type = DataManager.Instance.abilityTypeLanguage[DataManager.Instance.Get_ChoiceNeedAbility(_id)][Convert.ToInt32(LanguageManager.Instance.languageID)];
                    int amount = DataManager.Instance.Get_ChoiceNeedAbilityAmount(_id);

                    needAbilityTxt.transform.parent.gameObject.SetActive(true);
                    if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(1f, 0.25f).SetUpdate(true);
                    }
                    
                    needAbilityTxt.text = type + " : " + amount;
                    needAbilityTxt.TryGetComponent(out RectTransform rt);
                    if (DOTween.IsTweening(rt)) { DOTween.Kill(rt); }
                    if (DOTween.IsTweening(needAbilityTxt)) { DOTween.Kill(needAbilityTxt); }
                    rt.localScale = Vector3.one;
                    needAbilityTxt.color = Color.white;
                });
            // Pointer Exit
            IDisposable iDisExit = idBtn.button.OnPointerExitAsObservable()
                .Subscribe(_ =>
                {
                    if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(0f, 0.25f)
                            .OnComplete(() => { needAbilityTxt.transform.parent.gameObject.SetActive(false); })
                            .SetUpdate(true);
                    }
                });
            //Pointer Click 
            IDisposable iDisClick = idBtn.button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    string _id = idBtn.buttonID;

                    // 능력치 충분한지 체크
                    int need_obs = 0;
                    int need_soc = 0;
                    int need_men = 0;

                    if (DataManager.Instance.Get_ChoiceNeedAbility(_id) != "")
                    {
                        int need_ = DataManager.Instance.Get_ChoiceNeedAbilityAmount(_id);
                        switch (DataManager.Instance.Get_ChoiceNeedAbility(_id))
                        {
                            case "observation":
                                need_obs = need_;
                                break;
                            case "sociability":
                                need_obs = need_;
                                break;
                            case "mentality":
                                need_obs = need_;
                                break;
                        }
                    }

                    if (GameManager.Instance.mainInfo.IsEnoughAbility(need_obs, need_soc, need_men))
                    {
                        currentIO.IsInteracted = true;
                        ChoiceTab_Object3D(_id);
                    }
                    else
                    {
                        Debug.Log("불충분");
                        needAbilityTxt.TryGetComponent(out RectTransform rt);
                        rt.DOScale(1.2f, 0.2f).SetUpdate(true);
                        needAbilityTxt.DOColor(Color.red, 0.2f).SetUpdate(true);
                    }
                });
            iDisposables.Add(iDisEnter); iDisposables.Add(iDisExit); iDisposables.Add(iDisClick);
        }

    }
    public void ShowChioceWindow_Stream2D(string currentSChoiceID, float time)
    {
        // Set On
        streamChioceCG.gameObject.SetActive(true);
        streamChioceCG.DOFade(1f, time)
            .OnStart(() => { GameManager.Instance.canSkipTalking = false; })
            .OnComplete(() => { GameManager.Instance.canSkipTalking = true; })
            .SetUpdate(true);

        // Set Btn By ID
        List<string> SChoiceIDs = DataManager.Instance.Get_SChoiceIDs(currentSChoiceID);

        // Set UI
        for (int i = 0; i < SChoiceIDs.Count; i++)
        {
            IDBtn Choice_IDBtn = ObjectPooling.Instance.GetIDBtn();
            Choice_IDBtn.buttonType = ButtonType.ChoiceType_Stream2D;
            Choice_IDBtn.transform.SetParent(streamBtnParentRT);
            Choice_IDBtn.buttonID = SChoiceIDs[i];
            Choice_IDBtn.inputBasicImage = btnSprite;
            Choice_IDBtn.inputSizeDelta = new Vector2(1000f, 100f);

            Choice_IDBtn.inputAnchorPos = new Vector3(0f, i * -120f, 0f);
            if (SChoiceIDs.Count > 1) { Choice_IDBtn.inputAnchorPos += Vector3.up * 60f * (SChoiceIDs.Count - 1); }

            Choice_IDBtn.gameObject.SetActive(true);
            choiceBtnList.Add(Choice_IDBtn);
        }

        // Set Time Attack
        timeAttackFillImg.DOFillAmount(0f, StreamController.Instance.chooseLimitTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                ChoiceTab_Stream2D(choiceBtnList[choiceBtnList.Count - 1].buttonID);
            });

        // Set Btn Subscribe
        foreach (IDBtn idBtn in choiceBtnList)
        {
            //Pointer Click 
            IDisposable iDisClick = idBtn.button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    string id = idBtn.buttonID;
                    ChoiceTab_Stream2D(id);
                });
            iDisposables.Add(iDisClick);
        }

        
    }

    private void ChoiceTab_Object3D(string _id)
    {
        objectChioceCG.DOFade(0f, 0.2f)
            .OnStart(() =>
            {
                GameManager.Instance.canSkipTalking = false;
                Time.timeScale = 1f;
            })
            .OnComplete(() =>
            {
                GameManager.Instance.canSkipTalking = true;
                objectChioceCG.gameObject.SetActive(false);
            })
            .SetUpdate(true);


        // Sub Cancel
        foreach (IDisposable iDis in iDisposables) { iDis.Dispose(); }

        // Get Reasoning Contents
        string contentID = DataManager.Instance.Get_ReasoningMaterial(_id);
        if (contentID != "")
        { ReasoningManager.Instance.reasoningContentIDs.Add(contentID); }

        // Get Stream Quarter
        string quarterID = DataManager.Instance.Get_SDialogByChoice(_id);
        if (quarterID != "")
        { StreamController.Instance.playSDialogIDs.Add(quarterID); }

        // Obj Description Play
        ObjDescOn(currentIO, DataManager.Instance.Get_NextDialogByChoice(_id));

        // Object Pooling
        foreach (IDBtn idBtn in choiceBtnList)
        { ObjectPooling.Instance.GetBackIDBtn(idBtn); }
        choiceBtnList.Clear();
    }
    private void ChoiceTab_Stream2D(string _id)
    {
        streamChioceCG.DOFade(0f, 0.2f).OnComplete(() => { streamChioceCG.gameObject.SetActive(false); });
        StreamController.Instance.haveChoiceDialogID = "";
        // Show Reaction
        string PC_Chatting = DataManager.Instance.Get_SChoiceText(_id);
        string PC_Name = DataManager.Instance.Get_ObjectName("1000");
        if (PC_Chatting != "")
        { 
            IDBtn idBtn = ObjectPooling.Instance.GetIDBtn();
            StreamController.Instance.GenSpeechBubble(idBtn, PC_Chatting, PC_Name, true);
        }

        //GenSpeechBubble
        streamChioceCG.gameObject.SetActive(false);
        StreamController.Instance.SetScenarioBase(DataManager.Instance.Get_NextSDialogBySChoice(_id));

        // Get Gage
        int incGage = Convert.ToInt32(DataManager.Instance.Get_GEPoint(_id));
        StreamController.Instance.goodOrEvilGage += incGage;
        Debug.Log(StreamController.Instance.goodOrEvilGage);

        // iDisposables
        foreach (IDisposable iDis in iDisposables) { iDis.Dispose(); }

        // Object Pooling
        foreach (IDBtn idBtn in choiceBtnList)
        { ObjectPooling.Instance.GetBackIDBtn(idBtn); }
        choiceBtnList.Clear();


        // Kill Tween
        if (DOTween.IsTweening(timeAttackFillImg))
        { DOTween.Kill(timeAttackFillImg); }
        timeAttackFillImg.fillAmount = 1f;
    }

    #endregion

    #region SetAbilityUI

    public void SetAbilityUI()
    {
        abilityTxts[0].text = GameManager.Instance.mainInfo.ObservationalAbility.ToString();
        abilityTxts[1].text = GameManager.Instance.mainInfo.PersuasiveAbility.ToString();
        abilityTxts[2].text = GameManager.Instance.mainInfo.MentalStrengthAbility.ToString();
    }

    #endregion

    #region Sprite

    private List<Sprite> GetAllCharacterSprite()
    {
        List<Sprite> allSprite = new List<Sprite>();
        foreach(SpriteModule SM in spriteModules)
        {
            foreach(Sprite sprite in SM.sprites)
            {
                allSprite.Add(sprite);
            }
        }
        return allSprite;
    }
    
    public Sprite GetCharacterSprite(string IllustID)
    {
        List<Sprite> allIllust = GetAllCharacterSprite();
        foreach(Sprite sprite in allIllust)
        {
            if(sprite.name == IllustID)
            {
                return sprite;
            }
        }
        return null;
    }


    #endregion

}

[System.Serializable]
public class SpriteModule
{
    public string nameID;
    public Texture2D texture;
    public Sprite[] sprites;
}
