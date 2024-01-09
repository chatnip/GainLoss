using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System;

public class GameSystem : MonoBehaviour
{
    [SerializeField] PhoneHardware PhoneHardware;

    [Header("*Popular UI")]
    [SerializeField] public CanvasGroup loading;

    [Header("*Obj Panel")]
    [SerializeField] public GameObject objPanel;
    [SerializeField] public TMP_Text objText;
    [SerializeField] public Button objPanelExitBtn;

    [Header("*Npc Panel")]
    [SerializeField] public GameObject NpcPanel;
    [SerializeField] public TMP_Text NpcText;
    [SerializeField] public Image NpcImg;
    [SerializeField] public Button NpcPanelExitBtn;

    [Header("*Player")]
    public Transform playerPos;

    private void Start()
    {
        GameStart();

        objPanelExitBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                ObjectDescriptionOff();
            });
    }

    public void GameStart()
    {
        playerPos.position = new Vector3(0, 0.2f, 0);
        playerPos.rotation = Quaternion.identity;
        PhoneHardware.PhoneOn();
    }

    #region Loading

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

    #endregion

    #region Obj Panel

    public void ObjectDescriptionOn(string text) // 오브젝트 설명 패널 켜기
    {
        objText.text = "";
        objText.DOText(text, text.Length / 10).SetEase(Ease.Linear).SetId("Obj_Description");
        objPanel.SetActive(true);
    }
    
    public void ObjectDescriptionOff() // 오브젝트 설명 패널 끄기
    {
        DOTween.Complete("Obj_Description");
        objPanel.SetActive(false);
        objText.text = null;
    }

    #endregion

    #region Npc Panel

    public void NpcDescriptionOn(string text) // NPC 설명 패널 켜기
    {
        NpcText.text = "";
        NpcText.DOText(text, text.Length / 10).SetEase(Ease.Linear).SetId("Npc_Description");
        NpcPanel.SetActive(true);
    }

    public void NpcDescriptionOff() // 오브젝트 설명 패널 끄기
    {
        DOTween.Complete("Npc_Description");
        NpcPanel.SetActive(false);
        NpcText.text = null;
    }

    #endregion
}
