using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Word : MonoBehaviour
{
    [Tooltip("컴퓨터 창에 사용하는 버튼일 경우 true / 핸드폰 단어장 버튼일 경우 false")]
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
