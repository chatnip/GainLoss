using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class GameSystem : MonoBehaviour
{
    #region Value

    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] public PlayerController PlayerController;
    [SerializeField] public PlayerInputController PlayerInputController;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] Animator AnotherAnimator;
    [SerializeField] Pause Pause;

    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;

    [Header("*Popular UI")]
    [SerializeField] public CanvasGroup loading;
    [SerializeField] public Button PauseBtn;

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
    [SerializeField] public GameObject NpcInteractCamera;

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

        PauseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                PlayerInputController.OnOffPause();
            });

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
            cutsceneSO.skipOrCompleteSeq(cutsceneImg, cutsceneTxt);
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

        PlayerInputController.StopMove();
        ObjectInteractionButtonGenerator.SetOnOffAllBtns(false);

        objText.text = "";
        objText.DOText(text, text.Length / 10).SetEase(Ease.Linear).SetId("Obj_Description");
        objPanel.SetActive(true);
    }
    
    public void ObjectDescriptionSkip()
    {
        if (DOTween.IsTweening("Obj_Description"))
        { DOTween.Complete("Obj_Description"); }
        else 
        { ObjectDescriptionOff(); }
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
        PlayerInputController.StopMove();
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
        else if (!conversationTweeningNow)
        {
            NpcDescriptionOff();
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
                         Debug.Log("NPC 상호작용의 데이터 적용");
                         conversationTweeningNow = true;
                         NpcImg.sprite = conversations.NpcConversations[i].talkerSprite;
                         NpcName.text = conversations.NpcConversations[i].talkerName;
                        

                         if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.player)
                         {
                             PlayerAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName);
                             NpcInteractCamera.transform.position = conversations.playerCameraPos;
                             NpcInteractCamera.transform.LookAt(PlayerController.gameObject.transform.position + new Vector3(0, conversations.playerHeight, 0));
                         }
                         else if (conversations.NpcConversations[i].targetGO == ConversationBase.targetGO.another)
                         {
                             AnotherAnimator.SetTrigger(conversations.NpcConversations[i].AnimationTriggerName);
                             NpcInteractCamera.transform.position = conversations.anotherCameraPos;
                             NpcInteractCamera.transform.LookAt(AnotherAnimator.gameObject.transform.position + new Vector3(0, conversations.anotherHeight, 0));
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

    #region Get
    public string ft_setTextGetData(string ID)
    {
        string name = "";
        if (ID.Substring(0, 2) == "WA")
        {
            name = ft_setEachTextGetData(DataManager.WordActionDatas[3], WordManager.currentWordActionIDList, ID, ".EXE");
        }
        else if (ID.Substring(0, 1) == "W")
        {
            name = ft_setEachTextGetData(DataManager.WordDatas[5], WordManager.currentWordIDList, ID, ".AIL");
        }
        else if (ID.Substring(0, 1) == "P")
        {
            name = ft_setEachTextGetData(DataManager.PlaceDatas[1], PlaceManager.currentPlaceID_Dict.Keys.ToList(), ID, "(Place)");
        }
        return name;

        string ft_setEachTextGetData(Dictionary<string, object> Data, List<string> haveThings, string id, string type)
        {
            string nameTemp = "";
            if (!haveThings.Contains(id))
            {
                nameTemp = Data[id].ToString() + type + "\n★ 획득 ★";
                haveThings.Add(id);
            }
            else
            {
                nameTemp = "※ 이미 가지고 있는 요소 ※";
            }
            return nameTemp;
        }
    }
    #endregion
}
