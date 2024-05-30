//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] public BasicInteractBtn BasicInteractBtn;

    [Header("=== Need Input")]
    [SerializeField] public Vector3 inputAnchorPos;
    [SerializeField] public Vector2 inputSizeDelta;
    [SerializeField] public Sprite inputBasicImage;
    [SerializeField] public bool inputIsRight;
    [SerializeField] public string inputText;


    #endregion

    #region Framework

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.ChoiceType_Object3D:
                IDBtnSetup_ChoiceType_Object3D();
                break;
            case ButtonType.ChoiceType_Stream2D:
                IDBtnSetup_ChoiceType_Stream2D();
                break;

            case ButtonType.PlaceType:
                IDBtnSetup_PlaceType();
                break;

            case ButtonType.SpeechBubble_Stream2D:
                IDBtnSetup_SpeechBubble_Stream2D(inputIsRight);
                break;
        }
    }

    #endregion

    #region Set

    private void IDBtnSetup_ChoiceType_Object3D()
    {
        buttonText.text = DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        IDBtnSetup_ChoiceType();
    }
    private void IDBtnSetup_ChoiceType_Stream2D()
    {
        buttonText.text = DataManager.Instance.StreamModuleCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        IDBtnSetup_ChoiceType();
    }
    private void IDBtnSetup_ChoiceType()
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        button.image.sprite = inputBasicImage;

        rect.anchoredPosition3D = inputAnchorPos;
        rect.sizeDelta = inputSizeDelta;
        this.gameObject.transform.rotation = Camera.main.transform.rotation;

        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        rect.localScale = Vector3.one;

        button.enabled = true;
        BasicInteractBtn.enabled = true;
    }

    private void IDBtnSetup_PlaceType()
    {
        buttonText.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        rect.localScale = Vector3.one;
        button.enabled = true;
        button.image.sprite = inputBasicImage;
    }

    private void IDBtnSetup_SpeechBubble_Stream2D(bool isRight)
    {
        if(!isRight)
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            buttonText.alignment = TextAlignmentOptions.MidlineRight;
        }

        rect.anchoredPosition3D = new Vector3(0, StreamController.Instance.sb_IDBtns_Y[0], 0);
        rect.sizeDelta = inputSizeDelta;

        button.image.sprite = inputBasicImage;

        buttonText.color = Color.black;
        buttonText.text = inputText;

        rect.localScale = Vector3.one;

        button.enabled = false;
        BasicInteractBtn.enabled = false;
    }

    #endregion
}

[System.Serializable]
public enum ButtonType
{
    PlaceType,
    ChoiceType_Object3D,
    ChoiceType_Stream2D,
    DesktopAppType,
    SpeechBubble_Stream2D
}
