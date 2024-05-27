using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class IDBtn : MonoBehaviour
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public string buttonID;

    [Header("=== Component")]
    [SerializeField] public Sprite basicImage;
    [SerializeField] public Button button;
    [SerializeField] public TMP_Text buttonText;
    [SerializeField] RectTransform rect;
    [SerializeField] ButtonType buttonType;

    #endregion

    #region Main

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.WordType:
                break;

            case ButtonType.WordActionType:
                break;

            case ButtonType.PlaceType:
                IDBtnSetup_PlaceType();
                break;
            case ButtonType.ChoiceType:
                IDBtnSetup_ChoiceType();
                break;
        }
    }

    #endregion

    #region Set

    private void IDBtnSetup_ChoiceType()
    {
        buttonText.text = DataManager.Instance.ChoiceCSVDatas[LanguageManager.Instance.languageNum][this.buttonID].ToString();
        button.image.sprite = basicImage;
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
    WordType,
    WordActionType,

    PlaceType,
    ChoiceType
}
