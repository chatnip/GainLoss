using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class IDBtn : MonoBehaviour
{
    #region Value
    [Header("*Data")]
    [SerializeField] public ButtonValue buttonValue;
    [SerializeField] public string Rate;
    [SerializeField] Sprite basicImage;
    [SerializeField] Sprite folderImage;

    [Header("*Button")]
    [SerializeField] public Button button;
    [SerializeField] public TMP_Text text;
    [SerializeField] public TMP_Text rate_text;
    [SerializeField] RectTransform rect;
    [SerializeField] public ButtonType buttonType;

    [Header("*RateColor")]
    [SerializeField] Color clr_Positive;
    [SerializeField] Color clr_Normal;
    [SerializeField] Color clr_Malicious;

    [Header("*Canno Label")]
    [SerializeField] public GameObject CannotUseLabal;
    [SerializeField] public TMP_Text reason;


    [HideInInspector] public bool isButton;
    #endregion

    #region Main

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
                WordPadTypeAdd();
                break;

            case ButtonType.WordActionType:
                IDBtnSetup();
                WordActionTypeAdd(); 
                break;
            case ButtonType.WordActionPadType:
                IDBtnSetup();
                WordActionPadTypeAdd();
                break;

            case ButtonType.PlaceType:
                //IDBtnSetup();
                break;
            case ButtonType.PlacePadType:
                IDBtnSetup();
                PlacePadTypeAdd();
                break;
            case ButtonType.BehaviorActionType:
                IDBtnSetup();
                break;

            case ButtonType.ClueSpotType:
                break;
        }
    }

    #endregion

    #region Tpye Things

    // Streaming AIL
    private void WordTypeAdd()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 150);
        text.text += ".AIL";
        setRect(text, 20, 0, 0, 0, 45);
        setRect(rate_text, 0, 0, 20, 90, 30);
        button.image.sprite = folderImage;
    }
    // Streaming EXE
    private void WordActionTypeAdd()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 150);
        text.text += ".EXE";
        setRect(text, 20, 0, 0, 0, 45);
        setRect(rate_text, 0, 0, 20, 90, 30);
        button.image.sprite = folderImage;
    }


    // Pad AIL
    private void WordPadTypeAdd()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 300);
        text.text += ".AIL";
        setRect(text, 150, 0, 0, 0, 100);
        setRect(rate_text, 0, 0, 150, 0, 75);
    }

    // Pad EXE
    private void WordActionPadTypeAdd()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 300);
        text.text += ".EXE";
        setRect(text, 150, 0, 0, 0, 100);
        setRect(rate_text, 0, 0, 150, 0, 75);
    }

    // Pad Place
    private void PlacePadTypeAdd()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 300);
        text.text += "(Place)";
        setRect(text, 150, 0, 0, 0, 100);
        setRect(rate_text, 0, 0, 150, 0, 75);
    }

    private void setRect(TMP_Text targetTMP, float left, float bottom, float right, float top, float fontSize)
    {
        targetTMP.rectTransform.offsetMin = new Vector2(left, bottom);
        targetTMP.rectTransform.offsetMax = new Vector2(-right, -top);
        targetTMP.fontSize = fontSize;
    }

    #endregion

    #region Preliminary Survey
    public void ClueSpotTypeInput()
    {
        TryGetComponent(out Image image);
        image.DOFillAmount(1.0f, 0.2f)
            .SetEase(Ease.InCubic);
    }
    #endregion

    #region Condition

    public void AddVisiableWordRate(string Rate)
    {
        rate_text.text = Rate;
        if (Rate == "Positive") { rate_text.color = clr_Positive; }
        else if (Rate == "Normal") { rate_text.color = clr_Normal; }
        else if (Rate == "Malicious") { rate_text.color = clr_Malicious; }
        else { rate_text.text = ""; }
    }
    public void CannotUse(bool can, string Reason)
    {
        if(can)
        {
            CannotUseLabal.SetActive(false);
        }
        else
        {
            reason.text = Reason;
            CannotUseLabal.SetActive(true);
        }
    }
    public void CheckCannotUse()
    {
        if (CannotUseLabal.activeSelf)
        { button.interactable = false; }
        else
        { button.interactable = true; }
    }
    #endregion

    #region Set
    void IDBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        text.text = buttonValue.Name;
        text.rectTransform.localPosition = Vector3.zero;
        button.image.sprite = basicImage;
    }
    #endregion
}

[System.Serializable]
public enum ButtonType
{
    WordType,
    WordPadType,
    WordActionType,
    WordActionPadType,
    PlaceType,
    PlacePadType,
    BehaviorActionType,
    ClueSpotType
}
