using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    #region Value

    [Header("=== Font")]
    [SerializeField] public string languageID = "1";
    [SerializeField] List<TMP_FontAsset> fonts;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        TMP_Text[] allTmp_t = FindObjectsOfType<TMP_Text>();
        foreach(TMP_Text t in allTmp_t) { SetLanguageTxt(t); }
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

    public void SetLanguageTxt(TMP_Text tmpT)
    {
        tmpT.font = fonts[Convert.ToInt32(languageID)];
    }

    #endregion
}

[System.Serializable]
public class SameLanguageTxts
{
    public List<TMP_Text> LanguageTxts;
}