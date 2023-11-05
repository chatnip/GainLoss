using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordAction : MonoBehaviour
{
    [SerializeField] public Button wordActionBtn;
    [SerializeField] TMP_Text wordActionText;
    public WordBase wordBase;

    private void OnEnable()
    {
        wordActionText.text = wordBase.wordName;
    }
}
