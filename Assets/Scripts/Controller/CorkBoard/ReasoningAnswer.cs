//Refactoring v1.0
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReasoningAnswer : ReasoningModule
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public string thisSelectedMaterialID = "";
    [SerializeField] List<string> thisMaterialIDs = new List<string>();
    [SerializeField] List<string> thisMaterailNames = new List<string>();
    [SerializeField] ReasoningArrow relationRA;
    [SerializeField] ReasoningPhoto relationPT;

    [Header("=== Component")]
    [SerializeField] TMP_Dropdown thisDropdown;

    //OtherValue
    Dictionary<string, string> IDbyNameDict = new Dictionary<string, string>();


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
        thisDropdown.ClearOptions();
        thisMaterialIDs.Clear();
        thisMaterailNames.Clear();
        IDbyNameDict.Clear();

        // ���� ������ �߸� ���� ID ��� ã��
        foreach (string id in DataManager.Instance.Get_MaterialIDs(thisID))
        {
            if (reasoningMaterialIDs.Contains(id))
            {
                string _id = id;
                string _name = DataManager.Instance.Get_MaterialName(id);

                thisMaterialIDs.Add(_id);
                thisMaterailNames.Add(_name);

                IDbyNameDict.Add(_name, _id);
            }
        }

        // DropDown �ɼ� �߰�
        thisDropdown.AddOptions(thisMaterailNames);

        // �̹� �����ߴ� �ɼ��� �ִٸ�, �� �ɼ��� �⺻��
        for (int i = 0; i < thisDropdown.options.Count; i++)
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
        // ���õ� ID �� ����
        thisSelectedMaterialID = IDbyNameDict[thisDropdown.options[thisDropdown.value].text];
    }


    #endregion
}
