//Refactoring v1.0
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TutorialController : MonoBehaviour
{
    #region Value

    [Header("=== this")]
    [SerializeField] public string thisID;
    [SerializeField] public TMP_Text nameTxt;

    [Header("=== Child")]
    [SerializeField] List<TutorialCategory> categorys;

    #endregion

    #region Framework

    public void Offset()
    {
        nameTxt.text = DataManager.Instance.Get_TutorialName(thisID);
        for (int i = 0; i < categorys.Count; i++)
        {
            string _name = DataManager.Instance.Get_TutorialName(categorys[i].thisID);
            categorys[i].nameTxt.text = _name;
            string _desc = DataManager.Instance.Get_TutorialDesc(categorys[i].thisID);
            _desc = _desc.Replace("\\n", "\n");
            categorys[i].descTxt.text = _desc;
        }
        this.gameObject.SetActive(false);
    }


    #endregion
}

[System.Serializable]
public class TutorialCategory
{
    public string thisID;
    public TMP_Text nameTxt;
    public TMP_Text descTxt;
}
