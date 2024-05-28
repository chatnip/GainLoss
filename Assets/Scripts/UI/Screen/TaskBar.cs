using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;

public class TaskBar : MonoBehaviour
{
    [SerializeField] GameManager GameManager;
    [SerializeField] ComputerInteract ComputerInteract;
    [Header("*UI")]
    [SerializeField] Button windowButton;
    [SerializeField] TMP_Text CurrentDayText;
    [SerializeField] TMP_Text CurrentDayOfWeekText;
    [SerializeField] Image BlackScreen;

    private void Awake()
    {   
        windowButton.OnClickAsObservable()
            .Subscribe(btn =>
            {
                BlackScreen.color = Color.black;
                BlackScreen.gameObject.SetActive(true);

                BlackScreen.DOFade(1, 1)
                    .OnComplete(() =>
                    {
                        BlackScreen.gameObject.SetActive(true);
                    });
                ComputerInteract.ScreenOff();
            });
    }
    private void OnEnable()
    {
        CurrentDayText.text = "DAY " + GameManager.mainInfo.Day;
        CurrentDayOfWeekText.text = GameManager.mainInfo.TodayOfTheWeek;


    }
}
