using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IDBtn : MonoBehaviour
{
    [Header("*Data")]
    [SerializeField] public ButtonValue buttonValue;
    [SerializeField] public string Rate;
    [SerializeField] Sprite basicImage;
    [SerializeField] Sprite folderImage;

    [Header("*Button")]
    [SerializeField] public Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text rate_text;
    [SerializeField] RectTransform rect;
    [SerializeField] public ButtonType buttonType;

    [Header("*RateColor")]
    [SerializeField] Color clr_Positive;
    [SerializeField] Color clr_Normal;
    [SerializeField] Color clr_Malicious;

    [Header("*Canno Label")]
    [SerializeField] GameObject CannotUseLabal;
    [SerializeField] TMP_Text reason;


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


    public void AddVisiableWordRate(string Rate)
    {
        rate_text.text = Rate;
        if (Rate == "Positive") { rate_text.color = clr_Positive; }
        else if (Rate == "Normal") { rate_text.color = clr_Normal; }
        else if (Rate == "Malicious") { rate_text.color = clr_Malicious; }
    }
    public void CannotUse(bool can, string Reason)
    {
        if(can)
        {
            button.interactable = true;
            CannotUseLabal.SetActive(false);
        }
        else
        {
            reason.text = Reason;
            button.interactable = false;
            CannotUseLabal.SetActive(true);
        }
        
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
