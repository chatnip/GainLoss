using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IDBtn : MonoBehaviour
{
    [Header("*Data")]
    [SerializeField] public ButtonValue buttonValue;
    [SerializeField] Sprite basicImage;
    [SerializeField] Sprite folderImage;

    [Header("*Button")]
    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform rect;
    [SerializeField] public ButtonType buttonType;

    [HideInInspector] public bool isButton;

    

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.WordType:
                IDBtnSetup();
                WordTypeAdd();
                break;
            case ButtonType.WordPadType:
                IDBtnSetup();
                break;
            case ButtonType.WordActionType:
                IDBtnSetup();
                break;
            case ButtonType.PlaceType:
                break;
            case ButtonType.BehaviorActionType:
                IDBtnSetup();
                break;
        }
    }

    void WordTypeAdd()
    {
        text.text += ".ail";
        text.rectTransform.localPosition = new Vector3(0, -10, 0);
        button.image.sprite = folderImage;
    }


    void IDBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        text.text = buttonValue.Name;
        text.rectTransform.localPosition = Vector3.zero;
        button.image.sprite = basicImage;
    }
}

[System.Serializable]
public enum ButtonType
{
    WordType,
    WordPadType,
    WordActionType,
    PlaceType,
    BehaviorActionType
}
