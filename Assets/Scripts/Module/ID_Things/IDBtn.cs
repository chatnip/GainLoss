//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes.Test;

public class IDBtn : MonoBehaviour
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public string buttonID;
    [SerializeField] public ButtonType buttonType;

    [Header("=== Component")]
    [SerializeField] public RectTransform rect;
    [SerializeField] public Button button;
    [SerializeField] public TMP_Text buttonText;
    [SerializeField] public TMP_Text extraText;
    [SerializeField] public BasicInteractBtn BasicInteractBtn;

    [Header("=== Need Input")]
    [SerializeField] public Vector3 inputAnchorPos;
    [SerializeField] public Vector2 inputSizeDelta;
    [SerializeField] public Sprite inputBasicImage;
    [SerializeField] public bool inputIsRight;
    [SerializeField] public string inputText;
    [SerializeField] public string inputExtraText;


    #endregion

    #region Framework

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.ChoiceType_Object3D:
                IDBtnSetup_Base();
                IDBtnSetup_ChoiceType_Object3D();
                break;

            case ButtonType.ChoiceType_Stream2D:
                IDBtnSetup_Base();
                IDBtnSetup_ChoiceType_Stream2D();
                break;

            case ButtonType.SpeechBubble_Stream2D:
                IDBtnSetup_Base();
                IDBtnSetup_SpeechBubble_Stream2D();
                break;

            case ButtonType.ChoiceType_Reasoning2D:
                IDBtnSetup_Base();
                IDBtnSetup_ChoiceType_Reasoning2D();
                break;

        }
    }

    #endregion

    #region Set

    private void IDBtnSetup_Base()
    {
        buttonText.rectTransform.offsetMin = new Vector2(buttonText.rectTransform.offsetMin.x, 25);
        buttonText.rectTransform.offsetMax = new Vector2(buttonText.rectTransform.offsetMax.x, -25);

        extraText.rectTransform.offsetMax = new Vector2(extraText.rectTransform.offsetMax.x, 25);
        extraText.rectTransform.offsetMax = new Vector2(extraText.rectTransform.offsetMax.x, -25);

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        this.gameObject.transform.rotation = Quaternion.identity;

        rect.localScale = Vector3.one;

        buttonText.text = "";
        extraText.text = "";
        buttonText.color = Color.black;
        extraText.color = Color.black;
        buttonText.fontSizeMax = 50f;
        extraText.fontSizeMax = 35f;
        button.image.color = Color.white;   

        button.enabled = true;
        BasicInteractBtn.enabled = true;

        LanguageManager.Instance.SetLanguageTxt(buttonText);
        LanguageManager.Instance.SetLanguageTxt(extraText);
    }

    private void IDBtnSetup_ChoiceType_Object3D()
    {
        buttonText.text = DataManager.Instance.Get_ChoiceText(buttonID);
        IDBtnSetup_ChoiceType();
    }

    private void IDBtnSetup_ChoiceType_Stream2D()
    {
        buttonText.text = DataManager.Instance.Get_SChoiceText(buttonID);
        if (buttonText.text == "") { buttonText.text = ". . ."; }
        IDBtnSetup_ChoiceType();
    }

    private void IDBtnSetup_ChoiceType_Reasoning2D()
    {
        buttonText.text = DataManager.Instance.ReasoningContentCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        IDBtnSetup_ChoiceType();
    }

    private void IDBtnSetup_ChoiceType()
    {
        button.image.sprite = inputBasicImage;

        rect.anchoredPosition3D = inputAnchorPos;
        rect.sizeDelta = inputSizeDelta;

        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
    }

    private void IDBtnSetup_SpeechBubble_Stream2D()
    {

        if (!inputIsRight)
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;
            extraText.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            buttonText.alignment = TextAlignmentOptions.MidlineRight;
            extraText.alignment = TextAlignmentOptions.MidlineRight;
        }

        rect.anchoredPosition3D = new Vector3(0, StreamController.Instance.sb_IDBtns_Y[0], 0);
        rect.sizeDelta = inputSizeDelta;
        this.gameObject.transform.rotation = Quaternion.identity;

        button.image.sprite = inputBasicImage;

        buttonText.color = Color.black;
        extraText.color = Color.black;
        buttonText.text = inputText;
        extraText.text = inputExtraText;

        button.enabled = false;
        BasicInteractBtn.enabled = false;

        buttonText.fontSizeMax = 27f;

        buttonText.rectTransform.offsetMin = new Vector2(buttonText.rectTransform.offsetMin.x, 0);
        buttonText.rectTransform.offsetMax = new Vector2(buttonText.rectTransform.offsetMax.x, -rect.sizeDelta.y * 1f / 3f);

        extraText.rectTransform.offsetMin = new Vector2(extraText.rectTransform.offsetMin.x, rect.sizeDelta.y * 2f / 3f);
        extraText.rectTransform.offsetMax = new Vector2(extraText.rectTransform.offsetMax.x, 0);
    }

    
    #endregion
}

[System.Serializable]
public enum ButtonType
{
    IndividualType,
    ChoiceType_Object3D,
    ChoiceType_Stream2D,
    SpeechBubble_Stream2D,
    ChoiceType_Reasoning2D
}
