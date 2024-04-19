using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleInteractBtn : BasicInteractBtn
{
    [Header("*UI")]
    [SerializeField] public Toggle thisToggle;
    [SerializeField] TMP_Text onOffTxt;

    public override void Awake()
    {
        base.Awake();
        thisToggle.onValueChanged.AddListener(SetToggleUI);
    }
    public void ResetUI(bool _b)
    {
        thisToggle.isOn = _b;
        SetToggleUI(_b);
    }

    public void SetToggleUI(bool _b)
    {
        if (_b)
        { onOffTxt.text = "ON"; }
        else
        { onOffTxt.text = "OFF"; }
    }

}
