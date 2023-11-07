using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBtn : MonoBehaviour
{

    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform rect;

    public bool isButton;
    public string wordBtnTextStr;

    private void OnEnable()
    {
        WordBtnSetup();
    }

    void WordBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        text.text = wordBtnTextStr;
    }
}
