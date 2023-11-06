using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordAction : MonoBehaviour
{
    [SerializeField] public Button wordActionBtn;
    [SerializeField] TMP_Text wordActionText;
    public string wordActionName;

    private void OnEnable()
    {
        wordActionText.text = wordActionName;
    }
}
