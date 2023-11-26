using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IDBtn : MonoBehaviour
{
    [Header("*Data")]
    [SerializeField] public ButtonValue word;

    [Header("*Button")]
    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform rect;
    
    [HideInInspector] public bool isButton;
    

    private void OnEnable()
    {
        IDBtnSetup();
    }

    void IDBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        text.text = word.Name;
    }
}
