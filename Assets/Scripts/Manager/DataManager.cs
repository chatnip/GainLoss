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
    [SerializeField] TextAsset NpcCSV;
    [SerializeField] TextAsset ChoiceCSV;
    public List<Dictionary<string, object>> PlaceCSVDatas = new();
    public List<Dictionary<string, object>> ObjectCSVDatas = new();
    public List<Dictionary<string, object>> NpcCSVDatas = new();
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

        // 0. Eng / 1. Kor / 2. Jpn / 3. Get Activitiy Each Day / 4. Start Day / 5. End Day
        // 6. Default Observational / 7. Default Persuasive / 8. Default MentalStength
        // 9. Visitable Place IDs / 10. Interactable Object IDs / 11.Interactable NPC IDs
        // 12. VisitReason Eng / 13. VisitReason Kor / 14. VisitReason Jpn
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
        NpcCSVDatas = CSVReader.Read(this.NpcCSV);

        // 0. Eng / 1. Kor / 2. Jpn
        // 3. AnswerDesc Eng / 4. AnswerDesc Kor / 5.  AnswerDesc Jpn
        // 6. NeedObservational / 7. NeedPersuasive / 8. NeedMentalStrength
        // 9. GetPositiveAndNegative
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
