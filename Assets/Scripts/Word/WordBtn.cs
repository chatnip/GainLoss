using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBtn : MonoBehaviour
{
    [Tooltip("컴퓨터 창에 사용하는 버튼일 경우 true / 핸드폰 단어장 버튼일 경우 false")]
    public bool isTODO;
    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform rect;
    public string wordBtnTextStr;

    private void OnEnable()
    {
        WordBtnSetup();
    }

    void WordBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isTODO;
        text.text = wordBtnTextStr;
    }
}
