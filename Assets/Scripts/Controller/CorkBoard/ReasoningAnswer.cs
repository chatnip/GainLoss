//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class ReasoningAnswer : ReasoningModule
{
    #region Value

    [Header("=== Data")]
    [SerializeField] string thisSelectedMaterialID = "";
    [SerializeField] List<string> thisMaterialIDs = new List<string>();
    [SerializeField] ReasoningArrow relationRA;
    [SerializeField] ReasoningPhoto relationPT;

    [Header("=== Component")]
    [SerializeField] TMP_Dropdown thisDropdown;


    #endregion

    #region OnEnable

    public override void SetEachTime(float time)
    {
        // Set Visible
        if (relationRA != null && !this.gameObject.activeSelf && relationRA.isActive)
        { base.isActive = true; }
        else if (relationPT != null && !this.gameObject.activeSelf && relationPT.isActive)
        { base.isActive = true; }

        base.SetEachTime(time);
    }

    public void SetDropDownOption(List<string> reasoningMaterialIDs)
    {
        // Set Base
        thisDropdown.onValueChanged.RemoveAllListeners();
        thisMaterialIDs.Clear();

        // 적용 가능한 추리 소재 ID 모두 찾기
        foreach (string id in DataManager.Instance.Get_MaterialIDs(thisID))
        {
            if (reasoningMaterialIDs.Contains(id))
            { thisMaterialIDs.Add(id); }
        }

        // DropDown 옵션 추가
        thisDropdown.ClearOptions();
        thisDropdown.AddOptions(thisMaterialIDs);
        for(int i = 0; i < thisDropdown.options.Count; i++)
        {
            if (thisDropdown.options[i].text == thisSelectedMaterialID)
            {
                thisDropdown.value = i;
            }
        }

        // 처음 소재가 생길시
        if (thisSelectedMaterialID == "" && thisDropdown.options.Count > 0) 
        {
            thisDropdown.value = 0;
            setSelectedID();
        }

        // Set DropDown
        thisDropdown.onValueChanged.AddListener(delegate { setSelectedID(); });
    }

    private void setSelectedID()
    {
        thisSelectedMaterialID = thisDropdown.options[thisDropdown.value].text;
    }


    #endregion
}
