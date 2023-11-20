using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBtn : MonoBehaviour
{
    [Header("*Data")]
    [SerializeField] public Word word;

    [Header("*Button")]
    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform rect;
    
    [HideInInspector] public bool isButton;
    

    private void OnEnable()
    {
        WordBtnSetup();
    }

    void WordBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        text.text = word.Name;
    }
}
