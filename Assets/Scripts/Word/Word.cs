using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Word : MonoBehaviour
{
    [Tooltip("��ǻ�� â�� ����ϴ� ��ư�� ��� true / �ڵ��� �ܾ��� ��ư�� ��� false")]
    public bool isTODO;
    [SerializeField] Button wordBtn;
    [SerializeField] TMP_Text wordText;
    [SerializeField] RectTransform wordRect;
    public WordBase wordBase;

    private void OnEnable()
    {
        wordRect.localPosition = Vector3.zero;
        wordRect.localScale = Vector3.one;
        wordBtn.enabled = isTODO;
        wordText.text = wordBase.wordName;
    }
}
