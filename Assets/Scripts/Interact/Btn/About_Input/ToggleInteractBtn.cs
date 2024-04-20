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
    string SavedValue;

    public override void Awake()
    {
        base.Awake();
        thisToggle.onValueChanged.AddListener(SetToggleUI);
        if (GameObject.Find("OptionWindow").TryGetComponent(out Option option))
        { Option = option; }
    }
    private void OnDisable()
    {
        SavedValue = null;
    }
    public void ResetUI(bool _b)
    {
        thisToggle.isOn = _b; 
        if (_b)
        { onOffTxt.text = "ON"; }
        else
        { onOffTxt.text = "OFF"; }

        SavedValue = onOffTxt.text;
    }
    

    public void SetToggleUI(bool _b)
    {
        if (_b)
        { onOffTxt.text = "ON"; }
        else
        { onOffTxt.text = "OFF"; }

        if(SavedValue != onOffTxt.text && SavedValue != null && SavedValue != "")
        { Option.CanSave(true); }
    }

}
