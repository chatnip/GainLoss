using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LeTai.TrueShadow;
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

    [Header("=== Need Input")]
    [SerializeField] public Vector3 anchorPos;
    [SerializeField] public Vector2 sizeDelta;
    [SerializeField] public Sprite basicImage;


    #endregion

    #region Main

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.ChoiceType:
                IDBtnSetup_ChoiceType();
                break;

            case ButtonType.PlaceType:
                IDBtnSetup_PlaceType();
                break;
        }
    }

    #endregion

    #region Set

    private void IDBtnSetup_ChoiceType()
    {
        buttonText.text = DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString(); rect.anchoredPosition3D = anchorPos;
        buttonText.color = Color.white;
        button.image.sprite = basicImage;
        rect.localScale = Vector3.one;
        rect.sizeDelta = sizeDelta;
        button.enabled = true;
        this.gameObject.transform.rotation = Camera.main.transform.rotation;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.alignment = TextAlignmentOptions.Midline;
    }

    private void IDBtnSetup_PlaceType()
    {
        buttonText.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        rect.localScale = Vector3.one;
        button.enabled = true;
        button.image.sprite = basicImage;
    }


    #endregion
}

[System.Serializable]
public enum ButtonType
{
    PlaceType,
    ChoiceType,
    DesktopAppType
}
