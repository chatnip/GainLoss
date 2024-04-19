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

    public void SetToggleUI(bool _bool)
    {
        if (_bool)
        { onOffTxt.text = "ON"; }
        else
        { onOffTxt.text = "OFF"; }
    }

}
