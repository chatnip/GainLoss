using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;

public class GameSystem : MonoBehaviour
{
    [SerializeField] PhoneHardware PhoneHardware;

    [Header("*UI")]
    [SerializeField] public CanvasGroup loading;
    [SerializeField] public GameObject objPanel;
    [SerializeField] public TMP_Text objText;
    [SerializeField] public Button objPanelExitBtn;

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

    public void ObjectDescriptionOn(string text) // 오브젝트 설명 패널 켜기
    {
        objText.DOText(text, text.Length / 10).SetEase(Ease.Linear);
        objPanel.SetActive(true);
    }

    public void ObjectDescriptionOff() // 오브젝트 설명 패널 끄기
    {
        objPanel.SetActive(false);
        objText.text = null;
    }
}
