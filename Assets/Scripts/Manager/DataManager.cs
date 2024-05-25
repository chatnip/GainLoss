//Refactoring v1.0
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region Value

    [Header("=== CSV File")]
    [SerializeField] TextAsset ChapterCSV;
    [SerializeField] TextAsset AbilityCSV;
    [SerializeField] TextAsset PlaceCSV;
    [SerializeField] TextAsset ObjectCSV;
    [SerializeField] TextAsset LanguageCSV;

    // Other Data
    public List<Dictionary<string, object>> ChapterCSVDatas = new();
    public List<Dictionary<string, object>> AbilityCSVDatas = new();
    public List<Dictionary<string, object>> PlaceCSVDatas = new();
    public List<Dictionary<string, object>> ObjectCSVDatas = new();
    public List<Dictionary<string, object>> LanguageCSVDatas = new();

    #endregion

    #region Framework & Base Set

    protected override void Awake()
    {
        Offset_ReadData_from_CSV();
        base.Awake();
    }

    public void Offset_ReadData_from_CSV()
    {
        // 0. Eng / 1. Kor / 2. Jpn / 3. Get Activitiy Each Day / 4. Start Day / 5. End Day
        // 6. Default Observational / 7. Default Persuasive / 8. Default MentalStength
        // 9. Visitable Place IDs / 10.Interactable Object IDs
        ChapterCSVDatas = CSVReader.Read(this.ChapterCSV);

        // 0. Eng / 1. Kor / 2. Jpn
        AbilityCSVDatas = CSVReader.Read(this.AbilityCSV);

        // 0. Eng / 1. Kor / 2. Jpn
        PlaceCSVDatas = CSVReader.Read(this.PlaceCSV);

        // 0. Eng / 1. Kor / 2. Jpn // 3. Default Desc
        ObjectCSVDatas = CSVReader.Read(this.ObjectCSV);

        // 0. Num
        LanguageCSVDatas = CSVReader.Read(this.LanguageCSV);
    }

    

    #endregion

}
