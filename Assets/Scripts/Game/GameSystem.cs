//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main UI")]
    [SerializeField] public Button PauseBtn;

    [Header("=== Obj Panel")]
    [SerializeField] public Button objPanelBtn;
    [SerializeField] public TMP_Text objText;
    [SerializeField] public Button objPanelExitBtn;

    [Header("=== Npc Panel")]
    [SerializeField] public Button NpcPanelBtn;
    [SerializeField] public TMP_Text NpcName;
    [SerializeField] public TMP_Text NpcText;
    [SerializeField] public Image NpcImg;
    [SerializeField] public Button NpcPanelExitBtn;

    [Header("=== Animator")]
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] Animator AnotherAnimator;

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    // Dotween
    Tween objTextingTween;
    Tween npcTextingTween;

    // Other Value
    ConversationBase conversations;
    bool conversationTweeningNow = false;
    int currentOrder = 0;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        //Player

        playerPos.position = new Vector3(0f, 0f, 0f);
        playerPos.rotation = Quaternion.identity;

        //Btns

        PauseBtn.OnClickAsObservable()
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


        NpcPanelExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                NpcDescOff();
            });
        NpcPanelBtn.OnClickAsObservable()
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

    public void ObjDescOn(string text)
    {
        PlayerInputController.Instance.StopMove();
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);

        objText.text = "";
        objTextingTween = objText.DOText(text, text.Length / 10).SetEase(Ease.Linear).SetId("Obj_Description");
        objPanelBtn.gameObject.SetActive(true);
    }
    
    public void ObjDescSkip()
    {
        if (DOTween.IsTweening(objTextingTween)) { DOTween.Complete(objTextingTween); }
        else { ObjDescOff(); }
    }
    
    public void ObjDescOff()
    {
        PlayerInputController.Instance.CanMove = true;
        DOTween.Complete(objTextingTween);
        objPanelBtn.gameObject.SetActive(false);
        objText.text = null;

        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);
    }

    #endregion

    #region Npc Panel

    public void NpcDescOn(ConversationBase conversationBase, Animator animator)
    {
        PlayerController.Instance.setOnNpcInteractCamera(animator.gameObject);
        PlayerInputController.Instance.StopMove();
        PlayerController.Instance.isTalking = true;
        AnotherAnimator = animator;
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);
        this.conversations = conversationBase;
        npcTextingTween = SetTween(0);

        NpcPanelBtn.gameObject.SetActive(true);
    }

    public void NpcDescSkip()
    {
        if (conversationTweeningNow)
        {
            DOTween.Complete(npcTextingTween);
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

            DOTween.Kill(npcTextingTween);
            NpcText.text = null;
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
        NpcPanelBtn.gameObject.SetActive(false);
        DOTween.Kill(NpcText);
        NpcText.text = null;
        conversationTweeningNow = false;
        currentOrder = 0;
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);

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
                         Debug.Log("NPC 상호작용의 데이터 적용");
                         conversationTweeningNow = true;
                         NpcImg.sprite = conversations.NpcConversations[i].talkerSprite;
                         NpcName.text = conversations.NpcConversations[i].talkerName;
                        

                         if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.player)
                         { PlayerAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName); }
                         else if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.another)
                         { AnotherAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName); }
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
