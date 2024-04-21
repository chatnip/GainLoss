using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ToggleInteractBtn : BasicInteractBtn
{
    Option Option;

    [Header("*UI")]
    [SerializeField] public Toggle thisToggle;
    [SerializeField] TMP_Text onOffTxt;
    string savedValue;

    public override void Awake()
    {
        base.Awake();
        thisToggle.onValueChanged.AddListener(SetToggleUI);
        if (GameObject.Find("OptionWindow").TryGetComponent(out Option option))
        { Option = option; }
    }
    private void OnDisable()
    {
        savedValue = null;
    }
    public void ResetUI(bool _b)
    {
        thisToggle.isOn = _b;
        SetValueTxt(_b);
        savedValue = onOffTxt.text;
    }
    

    public void SetToggleUI(bool _b)
    {
        SetValueTxt(_b);
        if (savedValue != onOffTxt.text && savedValue != null && savedValue != "")
        { Option.CanSave(true); }
    }

    private void SetValueTxt(bool _b)
    {
        if (_b)
        { onOffTxt.text = "ON"; }
        else
        { onOffTxt.text = "OFF"; }
    }

}
