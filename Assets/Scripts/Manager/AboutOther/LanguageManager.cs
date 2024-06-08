using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public string languageID;
    [SerializeField] public int languageTypeAmount;
    [HideInInspector] public int languageNum;

    [Header("=== Font")]
    [SerializeField] List<TMP_FontAsset> fonts;

    [Header("=== Scene Component")]
    [SerializeField] List<SameLanguageTxts> MultipleLanguageTxts;


    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Get Lanaguage ID
        languageNum = GetLanguageNum(languageID);
    
        // Set Text by Lanaguage
        for(int i = 0; i < MultipleLanguageTxts.Count; i++)
        { 
            // Set Text
            int numLength = GetNumOfDigits(i);
            string id = "ST";
            for (int j = 0; j < 3 - numLength; j++) { id += "0"; }
            id += i.ToString();

            foreach (TMP_Text tmpTs in MultipleLanguageTxts[i].LanguageTxts)
            {
                tmpTs.font = fonts[languageNum];
                //tmpTs.text = DataManager.Instance.StaticTextCSVDatas[languageNum][id].ToString();
            } 
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Other

    public int GetNumOfDigits(int n)
    {
        if( n == 0) return 1;   
        return (int)Math.Floor(Math.Log10(n)) + 1;
    }

    #endregion

    #region Set Lanaguage

    private int GetLanguageNum(string languageID)
    {
        return Convert.ToInt32(languageID.Substring(1, 2));
    }

    public void SetLanguageTxt(TMP_Text tmpT)
    {
        tmpT.font = fonts[Convert.ToInt32(GameManager.Instance.languageID)];
    }

    public void SetLanguageTxts(List<TMP_Text> tmpTs)
    {
        foreach(TMP_Text tmpT in tmpTs)
        {
            SetLanguageTxt(tmpT);
        }
    }

    #endregion
}

[System.Serializable]
public class SameLanguageTxts
{
    public List<TMP_Text> LanguageTxts;
}