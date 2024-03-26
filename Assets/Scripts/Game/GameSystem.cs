using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System;
using Spine;

public class GameSystem : MonoBehaviour
{
    #region Value

    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] PlayerController PlayerController;
    [SerializeField] public PlayerInputController PlayerInputController;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] Animator AnotherAnimator;

    [Header("*Popular UI")]
    [SerializeField] public CanvasGroup loading;

    [Header("*Obj Panel")]
    [SerializeField] public GameObject objPanel;
    [SerializeField] public Button objPanelBtn_toSkip;
    [SerializeField] public TMP_Text objText;
    [SerializeField] public Button objPanelExitBtn;

    [Header("*Npc Panel")]
    [SerializeField] public GameObject NpcPanel;
    [SerializeField] public Button NpcPanelBtn_toSkip;
    [SerializeField] public TMP_Text NpcName;
    [SerializeField] public TMP_Text NpcText;
    [SerializeField] public Image NpcImg;
    [SerializeField] public Button NpcPanelExitBtn;

    [Header("*Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    ConversationBase conversations;
    bool conversationTweeningNow = false;
    int currentOrder = 0;

    [Header("*Player")]
    public Transform playerPos;

    #endregion

    #region Main

    private void Start()
    {
        SetPlayerTransform();


        objPanelExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ObjectDescriptionOff();
            });
        objPanelBtn_toSkip.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ObjectDescriptionSkip();
            });


        NpcPanelExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                NpcDescriptionOff();
            });
        NpcPanelBtn_toSkip.OnClickAsObservable()
            .Subscribe(btn => 
            { 
                NpcDescriptionSkip();
            });

        
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && cutsceneImg.gameObject.activeSelf)
        {
            cutsceneSO.skipOrCompleteSeq(cutsceneImg);
            return;
        }
    }

    public void SetPlayerTransform()
    {
        playerPos.position = new Vector3(0, 0.2f, 0);
        playerPos.rotation = Quaternion.identity;
    }

    #endregion

    /*#region Loading

    public void TurnOnLoading()
    {
        loading.gameObject.SetActive(true);
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.OnStart(() =>
        {
            loading.DOFade(1f, 0.3f);
        })
        .SetDelay(1f)
        .Append(loading.DOFade(0f, 1f))
        .OnComplete(() => loading.gameObject.SetActive(false));
    }

    #endregion*/

    #region Obj Panel

    public void ObjectDescriptionOn(string text) // 오브젝트 설명 패널 켜기
    {
        PlayerController.resetAnime();

        PlayerInputController.CanMove = false;
        ObjectInteractionButtonGenerator.SetOnOffAllBtns(false);

        objText.text = "";
        objText.DOText(text, text.Length / 10).SetEase(Ease.Linear).SetId("Obj_Description");
        objPanel.SetActive(true);
    }
    
    public void ObjectDescriptionSkip()
    {
        DOTween.Complete("Obj_Description");
    }
    
    public void ObjectDescriptionOff() // 오브젝트 설명 패널 끄기
    {
        PlayerInputController.CanMove = true;
        DOTween.Complete("Obj_Description");
        objPanel.SetActive(false);
        objText.text = null;

        ObjectInteractionButtonGenerator.SetOnOffAllBtns(true);
    }

    #endregion

    #region Npc Panel

    public void NpcDescriptionOn(ConversationBase conversationBase, Animator animator) // NPC 설명 패널 켜기
    {
        PlayerController.resetAnime();
        PlayerController.setOnNpcInteractCamera(animator.gameObject);
        PlayerInputController.CanMove = false;
        PlayerController.isTalking = true;
        AnotherAnimator = animator;
        ObjectInteractionButtonGenerator.SetOnOffAllBtns(false);
        this.conversations = conversationBase;
        SetTween(0);

        NpcPanel.SetActive(true);
    }

    public void NpcDescriptionSkip()
    {
        if (conversationTweeningNow)
        {
            DOTween.Complete(NpcText);
        }
        else if (!conversationTweeningNow && (conversations.NpcConversations.Count > currentOrder))
        {
            if(!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
            {
                PlayerAnimator.SetTrigger("Return");
            }
            if (AnotherAnimator != null)
            {
                if (!AnotherAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    AnotherAnimator.SetTrigger("Return");
                }

            }

            DOTween.Kill(NpcText);
            NpcText.text = null;
            SetTween(currentOrder);
        }
    }

    public void NpcDescriptionOff() // NPC 설명 패널 끄기
    {
        PlayerInputController.CanMove = true;
        PlayerController.setOffNpcInteractCamera();
        PlayerController.isTalking = false;
        NpcPanel.SetActive(false);
        DOTween.Kill(NpcText);
        NpcText.text = null;
        conversationTweeningNow = false;
        currentOrder = 0;
        ObjectInteractionButtonGenerator.SetOnOffAllBtns(true);

        PlayerAnimator.SetTrigger("Return");
        AnotherAnimator.SetTrigger("Return");


        AnotherAnimator = PlayerAnimator;
    }

    private Tween SetTween(int i)
    {
        Tween tw = NpcText.DOText(conversations.NpcConversations[i].conversation, conversations.NpcConversations[i].conversationDurTime)
                     .SetEase(Ease.Linear)
                     .OnStart(() =>
                     {
                         conversationTweeningNow = true;
                         //NpcImg.sprite = conversations.NpcConversations[i].talkerSprite;
                         NpcName.text = conversations.NpcConversations[i].talkerName;
                         if(conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.player)
                         {
                             PlayerAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName);
                         }
                         else if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.another)
                         {
                             AnotherAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName);
                         }
                     })
                     .OnComplete(() =>
                     {
                         conversationTweeningNow = false;
                         currentOrder++;
                     });
        return tw;
    }

    #endregion
}
