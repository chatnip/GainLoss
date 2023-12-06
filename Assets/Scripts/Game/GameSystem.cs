using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameSystem : MonoBehaviour
{
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] CanvasGroup loading;

    [Header("*Player")]
    public Transform playerPos;

    private void Start()
    {
        GameStart();
    }

    private void GameStart()
    {
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
}
