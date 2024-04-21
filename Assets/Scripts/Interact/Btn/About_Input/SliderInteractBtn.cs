using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class SliderInteractBtn : BasicInteractBtn
{
    Option Option;

    [Header("*UI")]
    [SerializeField] public Slider thisSlider;
    [SerializeField] TMP_Text valueTxt;
    string savedValue;

    public override void Awake()
    {
        base.Awake();
        thisSlider.onValueChanged.AddListener(SetSliderUI);
        if (GameObject.Find("OptionWindow").TryGetComponent(out Option option))
        { Option = option; }

    }
    public void ResetUI(int _i)
    {
        Debug.Log(_i);
        float silderValue = GetSilderValue_fromPercentValue(_i);
        Debug.Log(silderValue);
        SetValueTxt(silderValue);
        savedValue = valueTxt.text;
    }
    private void OnDisable()
    {
        savedValue = null;
    }
    public void SetSliderUI(float _f)
    {
        SetValueTxt(_f);
        if (savedValue != valueTxt.text && savedValue != null && savedValue != "")
        { Option.CanSave(true); }
    }
    private void SetValueTxt(float _f)
    {
        thisSlider.value = _f;
        string inputstring = GetPercentValue_fromSilderValue(_f).ToString() + "%";
        if(inputstring != valueTxt.text)
        { valueTxt.text = inputstring; }
    }
    private float GetSilderValue_fromPercentValue(int _i)
    {
        float percentValue = (float)_i;
        percentValue -= 50f;
        percentValue *= 0.01f;
        return percentValue;
    }
    public int GetPercentValue_fromSilderValue(float _f)
    {
        float sliderValue = _f;
        sliderValue *= 100f;
        sliderValue += 50f;
        return (int)Math.Ceiling(sliderValue);
    }
}
