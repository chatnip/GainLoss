using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordBtn : MonoBehaviour
{
    [Tooltip("��ǻ�� â�� ����ϴ� ��ư�� ��� true / �ڵ��� �ܾ��� ��ư�� ��� false")]
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
