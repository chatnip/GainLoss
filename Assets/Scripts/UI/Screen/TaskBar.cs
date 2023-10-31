using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TaskBar : MonoBehaviour
{
    [SerializeField] ScreenObject ScreenObject;
    [SerializeField] Button windowButton;

    private void Awake()
    {
        windowButton
            .OnClickAsObservable()
            .Subscribe(x =>
            {
                ScreenObject.InteractCancel();
            });
    }
}
