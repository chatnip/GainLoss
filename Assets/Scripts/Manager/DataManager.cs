//Refactoring v1.0
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region Value

    [Header("=== CSV File")]
    [Header("-- About Entire Play")]
    [SerializeField] TextAsset ChapterCSV;
    [SerializeField] TextAsset AbilityCSV;
    public List<Dictionary<string, object>> ChapterCSVDatas = new();
    public List<Dictionary<string, object>> AbilityCSVDatas = new();


    [Header("-- About Play")]
    [SerializeField] TextAsset PlaceCSV;
    [SerializeField] TextAsset ObjectCSV;
    [SerializeField] TextAsset ChoiceCSV;
    public List<Dictionary<string, object>> PlaceCSVDatas = new();
    public List<Dictionary<string, object>> ObjectCSVDatas = new();
    public List<Dictionary<string, object>> ChoiceCSVDatas = new();


    [Header("-- About Language")]
    [SerializeField] TextAsset LanguageCSV;
    [SerializeField] TextAsset StaticTextCSV; 
    public List<Dictionary<string, object>> LanguageCSVDatas = new();
    public List<Dictionary<string, object>> StaticTextCSVDatas = new();

    // Other Data




    #endregion

    #region Framework & Base Set

    protected override void Awake()
    {
        Offset_ReadData_from_CSV();
        base.Awake();
    }

    public void Offset_ReadData_from_CSV()
    {
        #region Entire Play

        // 0. Eng / 1. Kor / 2. Jpn 
        // 3. VisitReason Eng / 4. VisitReason Kor / 5. VisitReason Jpn

        // (a = LanguageAmount*2)
        // a+0. Get Activitiy Each Day / a+1. Start Day / a+2. End Day
        // a+3. Default Observational / a+4. Default Persuasive / a+5. Default MentalStength
        // a+6. Visitable Place IDs / a+7. Interactable Object IDs

        ChapterCSVDatas = CSVReader.Read(this.ChapterCSV);


        // 0. Eng / 1. Kor / 2. Jpn

        AbilityCSVDatas = CSVReader.Read(this.AbilityCSV);

        #endregion

        #region Play

        // 0. Eng / 1. Kor / 2. Jpn

        PlaceCSVDatas = CSVReader.Read(this.PlaceCSV);


        // 0. Eng / 1. Kor / 2. Jpn
        // 3. Defualt Text Eng / 4. Defualt Text Kor / 5. Defualt Text Jpn
        // 6. Extra Text Eng / 7. Extra Text Kor / 8. Extra Text Jpn 

        ObjectCSVDatas = CSVReader.Read(this.ObjectCSV);


        // 0. Eng / 1. Kor / 2. Jpn
        // 3. AnswerDesc Eng / 4. AnswerDesc Kor / 5.  AnswerDesc Jpn

        // (a = LanguageAmount*2)
        // a+0. NeedObservational / a+1. NeedPersuasive / a+2. NeedMentalStrength
        // a+3. GetContents

        ChoiceCSVDatas = CSVReader.Read(this.ChoiceCSV);

        #endregion

        #region Language

        // 0. Num

        LanguageCSVDatas = CSVReader.Read(this.LanguageCSV);


        // 0 -> Length (It have one's turn)

        StaticTextCSVDatas = CSVReader.Read(this.StaticTextCSV);

        #endregion

    }



    #endregion

}
