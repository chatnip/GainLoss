using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class TaskBar : MonoBehaviour
{
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] Button windowButton;
    [SerializeField] TMP_Text CurrentDay;

    private void Awake()
    {   
        windowButton.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ComputerInteract.StartCoroutine(ComputerInteract.ScreenZoomOut(false));
            });
    }
    public void SetCurrentDay(float Day)
    {
        CurrentDay.text = "DAY " + Day;
    }
}
