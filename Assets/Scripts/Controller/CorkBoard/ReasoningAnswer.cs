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

        // 적용 가능한 추리 소재 ID 모두 찾기
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

        // DropDown 옵션 추가
        thisDropdown.AddOptions(thisMaterailNames);

        // 이미 선택했던 옵션이 있다면, 그 옵션이 기본값
        for (int i = 0; i < thisDropdown.options.Count; i++)
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
        // 선택된 ID 값 저장
        thisSelectedMaterialID = IDbyNameDict[thisDropdown.options[thisDropdown.value].text];
    }


    #endregion
}
