using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class IDBtn : MonoBehaviour
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public ButtonValue buttonValue;

    [Header("=== Component")]
    [SerializeField] Sprite basicImage;
    [SerializeField] public Button button;
    [SerializeField] public TMP_Text buttonText;
    [SerializeField] RectTransform rect;
    [SerializeField] public ButtonType buttonType;

    [HideInInspector] public bool isButton;

    #endregion

    #region Main

    private void OnEnable()
    {
        switch (buttonType)
        {
            case ButtonType.WordType:
                IDBtnSetup();
                break;

            case ButtonType.WordActionType:
                IDBtnSetup();
                break;

            case ButtonType.PlaceType:
                IDBtnSetup();
                break;
        }
    }

    #endregion

    #region Set

    void IDBtnSetup()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        button.enabled = isButton;
        buttonText.text = buttonValue.Name;
        buttonText.rectTransform.localPosition = Vector3.zero;
        button.image.sprite = basicImage;
    }

    #endregion
}

[System.Serializable]
public enum ButtonType
{
    WordType,
    WordActionType,
    PlaceType
}
