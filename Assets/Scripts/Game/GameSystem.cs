//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using System;
using UniRx.Triggers;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main UI")]
    [SerializeField] Button pauseBtn;
    [SerializeField] List<TMP_Text> abilityTxts;

    [Header("=== Obj Panel")]
    [SerializeField] public Button objPanelBtn;
    [SerializeField] TMP_Text objText;
    [SerializeField] Button objPanelExitBtn;

    [Header("=== Npc Panel")]
    [SerializeField] public Button npcPanelBtn;
    [SerializeField] TMP_Text npcName;
    [SerializeField] TMP_Text npcText;
    [SerializeField] Image npcImg;
    [SerializeField] Button npcPanelExitBtn;

    [Header("=== Choice UI")]
    [SerializeField] CanvasGroup chioceCG;
    [SerializeField] RectTransform btnsRT;
    [SerializeField] List<IDBtn> choiceBtnList = new List<IDBtn>();
    [SerializeField] TMP_Text needAbilityTxt;
    [SerializeField] Sprite btnSprite;

    [Header("=== Animator")]
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator anotherAnimator;

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    // Dotween
    Tween objTextingTween;
    Tween npcTextingTween;
    InteractObject currentIO;
    
    // Other Value
    ConversationBase conversations;
    bool conversationTweeningNow = false;
    int currentOrder = 0;
    List<IDisposable> iDisposables = new List<IDisposable>();
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
        List<TMP_Text> systemTmpT = new List<TMP_Text> { objText, npcName, npcText, needAbilityTxt, cutsceneTxt };
        LanguageManager.Instance.SetLanguageTxts(systemTmpT);
        

        // Choice
        chioceCG.gameObject.SetActive(false);
        chioceCG.alpha = 0f;

        // Btns
        pauseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                PlayerInputController.Instance.OnOffPause();
            });

        objPanelExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ObjDescOff();
            });
        objPanelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ObjDescSkip();
            });


        npcPanelExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                NpcDescOff();
            });
        npcPanelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                NpcDescSkip();
            });

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

    public void ObjDescOn(InteractObject IO)
    {
        currentIO = IO;
        GameManager.Instance.canInput = false;
        GameManager.Instance.canInteractObject = false;
        PlayerInputController.Instance.StopMove();

        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);


        objText.text = "";
        string writeText = "";
        if (currentIO.IsInteracted) 
        { 
            writeText = DataManager.Instance.ObjectCSVDatas[3 + LanguageManager.Instance.languageNum][currentIO.ID].ToString();
            objPanelExitBtn.interactable = true;
        }
        else 
        {
            writeText = DataManager.Instance.ObjectCSVDatas[6 + LanguageManager.Instance.languageNum][currentIO.ID].ToString();
            objPanelExitBtn.interactable = false; 
        }
        objTextingTween = objText.DOText(writeText, writeText.Length / 10).SetEase(Ease.Linear);
        objPanelBtn.gameObject.SetActive(true);

    }
    
    public void ObjDescSkip()
    {
        if (DOTween.IsTweening(objTextingTween)) 
        { DOTween.Complete(objTextingTween); }
        else 
        {
            if (currentIO.IsInteracted)
            { ObjDescOff(); }
            else
            { ShowChioceWindow(0.5f); }
        }
    }


    public void ObjDescOff()
    {
        GameManager.Instance.canInput = true;
        GameManager.Instance.canInteractObject = true;
        PlayerInputController.Instance.CanMove = true;
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);

        DOTween.Complete(objTextingTween);
        objPanelBtn.gameObject.SetActive(false);
        objText.text = null;
        currentIO = null;
    }

    #endregion

    #region Npc Panel

    public void NpcDescOn(ConversationBase conversationBase, Animator animator)
    {
        PlayerController.Instance.setOnNpcInteractCamera(animator.gameObject);
        PlayerInputController.Instance.StopMove();
        PlayerController.Instance.isTalking = true;
        anotherAnimator = animator;
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);
        this.conversations = conversationBase;
        npcTextingTween = SetTween(0);

        npcPanelBtn.gameObject.SetActive(true);
    }

    public void NpcDescSkip()
    {
        if (conversationTweeningNow)
        {
            DOTween.Complete(npcTextingTween);
        }
        else if (!conversationTweeningNow && (conversations.NpcConversations.Count > currentOrder))
        {
            if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
            {
                playerAnimator.SetTrigger("Return");
            }
            if (anotherAnimator != null)
            {
                if (!anotherAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    anotherAnimator.SetTrigger("Return");
                }

            }

            DOTween.Kill(npcTextingTween);
            npcText.text = null;
            npcTextingTween = SetTween(currentOrder);
        }
        else if (!conversationTweeningNow)
        {
            NpcDescOff();
        }
    }

    public void NpcDescOff()
    {
        PlayerInputController.Instance.CanMove = true;
        PlayerController.Instance.setOffNpcInteractCamera();
        PlayerController.Instance.isTalking = false;
        npcPanelBtn.gameObject.SetActive(false);
        DOTween.Kill(npcText);
        npcText.text = null;
        conversationTweeningNow = false;
        currentOrder = 0;
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);

        playerAnimator.SetTrigger("Return");
        anotherAnimator.SetTrigger("Return");


        anotherAnimator = playerAnimator;
    }

    private Tween SetTween(int i)
    {
        Tween tw = npcText.DOText(conversations.NpcConversations[i].conversation, conversations.NpcConversations[i].conversationDurTime)
                     .SetEase(Ease.Linear)
                     .OnStart(() =>
                     {
                         Debug.Log("NPC 상호작용의 데이터 적용");
                         conversationTweeningNow = true;
                         npcImg.sprite = conversations.NpcConversations[i].talkerSprite;
                         npcName.text = conversations.NpcConversations[i].talkerName;
                        

                         if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.player)
                         { playerAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName); }
                         else if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.another)
                         { anotherAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName); }
                     })
                     .OnComplete(() =>
                     {
                         conversationTweeningNow = false;
                         currentOrder++;
                     });
        return tw;
    }

    #endregion

    #region Choice

    private void ShowChioceWindow(float time)
    {
        // Set On
        chioceCG.gameObject.SetActive(true);
        chioceCG.DOFade(1f, time)
            .OnStart(() => { GameManager.Instance.canInput = false; })
            .OnComplete(() => { GameManager.Instance.canInput = true; });

        // Set Btn By ID
        List<string> IDs = new List<string>();
        foreach(KeyValuePair<string, object> dictDatas in DataManager.Instance.ChoiceCSVDatas[0])
        {
            if (dictDatas.Key != "ID" && dictDatas.Key.Substring(0, 4) == currentIO.ID)
            { IDs.Add(dictDatas.Key); }
        }

        // Set UI
        for (int i = 0; i < IDs.Count; i++)
        {
            IDBtn Choice_IDBtn = ObjectPooling.Instance.GetIDBtn();
            Choice_IDBtn.buttonType = ButtonType.ChoiceType;
            Choice_IDBtn.transform.SetParent(btnsRT);
            Choice_IDBtn.buttonID = IDs[i];
            Choice_IDBtn.basicImage = btnSprite;
            Choice_IDBtn.sizeDelta = new Vector2(1000f, 100f);
            LanguageManager.Instance.SetLanguageTxt(Choice_IDBtn.buttonText);

            Choice_IDBtn.anchorPos = new Vector3(0f, i * -150f, 0f);
            if (IDs.Count > 1) { Choice_IDBtn.anchorPos += Vector3.up * 75f * (IDs.Count - 1); }

            Choice_IDBtn.gameObject.SetActive(true);
            choiceBtnList.Add(Choice_IDBtn);
        }

        // Set Btn Subscribe
        foreach (IDBtn idBtn in choiceBtnList)
        {
            IDisposable iDisEnter = idBtn.button.OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    string _id = idBtn.buttonID;
                    if (_id.Substring(4, 3) == "C99") { return; }
                    needAbilityTxt.transform.parent.gameObject.SetActive(true);
                    if(needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(1f, 0.25f);
                    }
                    needAbilityTxt.text =
                        DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 0][_id].ToString() + " / " +
                        DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 1][_id].ToString() + " / " +
                        DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 2][_id].ToString();
                    needAbilityTxt.color = Color.white;
                });
            IDisposable iDisExit = idBtn.button.OnPointerExitAsObservable()
                .Subscribe(_ =>
                {
                    if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(0f, 0.25f)
                        .OnComplete(() => { needAbilityTxt.transform.parent.gameObject.SetActive(false); });
                    }
                });
            IDisposable iDisClick = idBtn.button.OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    // 능력치 충분한지 체크
                    string _id = idBtn.buttonID;
                    int need_obse = Convert.ToInt32(DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 0][_id]);
                    int need_pers = Convert.ToInt32(DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 1][_id]);
                    int need_ment = Convert.ToInt32(DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 2][_id]);
                    if (GameManager.Instance.mainInfo.IsEnoughAbility(need_obse, need_pers, need_ment)) 
                    { 

                    }
                    else
                    {
                        needAbilityTxt.color = Color.red;
                    }
                });
            iDisposables.Add(iDisEnter); iDisposables.Add(iDisExit); iDisposables.Add(iDisClick);
        }
        
    }

    private void ChoiceTab()
    {
        currentIO.IsInteracted = true;
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
}
