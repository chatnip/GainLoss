using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class PhoneSoftware : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] GameManager GameManager;

    [Header("*Software")]
    [SerializeField] Button lockScreen;
    [SerializeField] public Button mapBtn;
    [SerializeField] GameObject map;
    [SerializeField] Button wordpadBtn;
    [SerializeField] GameObject wordpad;
    [SerializeField] Button backBtn;

    [Header("*Day")]
    [SerializeField] TMP_Text MapBtnDay;

    private void Awake()
    {
        lockScreen
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                lockScreen.gameObject.SetActive(false);
            });

        mapBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(true);
            });

        wordpadBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                wordpad.SetActive(true);
            });
        
        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(false);
                wordpad.SetActive(false);
            });       
    }
    public void ResetUI()
    {
        #region WindowUI

        lockScreen.gameObject.SetActive(true);
        mapBtn.interactable = true;
        map.SetActive(false);
        wordpadBtn.interactable = true;
        wordpad.SetActive(false);

        #endregion

        #region Day

        MapBtnDay.text = GameManager.currentMainInfo.day + " Day";

        #endregion

    }

    private void OnEnable()
    {
        ResetUI();
    }
}
