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

        // ���� ������ �߸� ���� ID ��� ã��
        foreach (string id in DataManager.Instance.Get_MaterialIDs(thisID))
        {
            if (reasoningMaterialIDs.Contains(id))
            { thisMaterialIDs.Add(id); }
        }

        // DropDown �ɼ� �߰�
        thisDropdown.ClearOptions();
        thisDropdown.AddOptions(thisMaterialIDs);
        for(int i = 0; i < thisDropdown.options.Count; i++)
        {
            if (thisDropdown.options[i].text == thisSelectedMaterialID)
            {
                thisDropdown.value = i;
            }
        }

        // ó�� ���簡 �����
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
